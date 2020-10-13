// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using System.Threading.Tasks;
using System.Threading;

namespace TelegramBot
{
    public class TB_OrderManagement
    {
        private readonly ITelegramBotClient _botClient;
        public TB_OrderManagement(ITelegramBotClient botClient){
            _botClient = botClient;
        }

        [FunctionName("TB_OrderManagement")]
        public  async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
           await _botClient.SendTextMessageAsync(eventGridEvent.Id, $"Your order of {eventGridEvent.Data} is in the kitchen");
           Thread.Sleep(5000);
           await _botClient.SendTextMessageAsync(eventGridEvent.Id, $"Your order of {eventGridEvent.Data} is on it's way");
        }
    }
}
