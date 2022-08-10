using Discord.Gateway;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace SteamWorker
{
    internal class Bot : BackgroundService
    {
        private readonly GameClaimerFactory factory;
        private readonly ILogger<Bot> logger;
        private readonly Settings settings;
        private readonly DiscordSocketClient client;

        public Bot(GameClaimerFactory factory, IOptions<Settings> options, ILogger<Bot> logger)
        {
            settings = options.Value;
            client = new DiscordSocketClient();
            client.OnLoggedIn += Client_OnLoggedIn;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.Login(settings.Token);
            client.SetStatus(UserStatus.Invisible);
            this.factory = factory;
            this.logger = logger;
        }

        private void Client_OnLoggedIn(DiscordSocketClient client, LoginEventArgs args)
        {
            logger.LogInformation("Logged in");
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(-1, stoppingToken);
        }

        private void Client_OnMessageReceived(DiscordSocketClient client, MessageEventArgs args)
        {
            //Console.WriteLine(args.Message.Content);

            var match = Regex.Matches(args.Message.Content, settings.Regex);

            if (match.FirstOrDefault() != null)
            {
                foreach (var key in match)
                {
                    client.OnMessageReceived -= Client_OnMessageReceived;
                    ISteamReedemer reedemer = factory.CreateSteamRedemer();
                    reedemer.RedeemGame(key.ToString());
                    logger.LogInformation("Added key {key}",key.ToString());
                    Thread.Sleep(Convert.ToInt32(settings.Dely));
                    client.OnMessageReceived += Client_OnMessageReceived;
                }

                try
                {
                }
                catch (DiscordHttpException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

}