using System;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

[assembly: FunctionsStartup(typeof(Food4U.Startup))]
namespace Food4U
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            ITelegramBotClient telegramBot = new TelegramBotClient(Environment.GetEnvironmentVariable("TelegramApiKey"));
            telegramBot.SetWebhookAsync(Environment.GetEnvironmentVariable("TelegramWebHookAddress"));

            TopicCredentials credentials = new TopicCredentials(Environment.GetEnvironmentVariable("EventGridTopicKey"));
            EventGridClient eventGridClient = new EventGridClient(credentials);

            builder.Services.AddSingleton(telegramBot);           
            builder.Services.AddSingleton(eventGridClient);
        }
    }
}