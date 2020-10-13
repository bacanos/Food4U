using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System.Collections.Generic;

namespace TelegramBot
{
    public  class TB_Conversational
    {
        private const string menu = "Hamburger\nTortilla\nPizza";
        private readonly ITelegramBotClient _botClient;
        private readonly EventGridClient _eventGridClient;
        

        public TB_Conversational(ITelegramBotClient botClient, EventGridClient eventGridClient){
            _botClient = botClient;
            _eventGridClient = eventGridClient;
        }

        [FunctionName("TB_Conversational")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {         

             Update update;
            string body = await new StreamReader(req.Body).ReadToEndAsync();

            try{
                update = JsonConvert.DeserializeObject<Update>(body);
            }catch(Exception ){
                return new BadRequestResult();
            }                      

            await HandleMessages(update);

            return new OkResult();
        }

        private async Task HandleMessages(Update update)
        {
            switch(update.Message.Text){
                case "/start": await _botClient.SendTextMessageAsync(update.Message.Chat.Id, 
                "To order any dish just type /order following with the dish from menu \n" + menu);
                break;

                case string s when s.Contains("/order"): await HandleOrdersAsync(update);
                break;

                default: await _botClient.SendTextMessageAsync(update.Message.Chat.Id, 
                "Sorry I didn't understand you");
                break;
            }
        }

        private async Task HandleOrdersAsync(Update update)
        {

            string order = update.Message.Text.Split("/order")[1];

            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, 
                $"Your order of {order} has been processed"); 

            string eventGridTopicHostName = new Uri(Environment.GetEnvironmentVariable("EventGridTopicHostName")).Host;   

            EventGridEvent eventGridEvent = new EventGridEvent(){
                Id = update.Message.Chat.Id.ToString(),
                EventType = "Bot.Order",
                EventTime = DateTime.UtcNow,
                Data = order,
                DataVersion = "v1",
                Subject = "Order"
            };    

            await _eventGridClient.PublishEventsAsync(eventGridTopicHostName,new List<EventGridEvent>(){eventGridEvent});

        }
    }
}
