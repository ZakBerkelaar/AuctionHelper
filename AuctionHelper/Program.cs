using DSharpPlus;
using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using DSharpPlus.Entities;
using System.Net;
using AuctionHelper.Websites;

namespace AuctionHelper
{
    public static class Program
    { 
        private static List<Website> websites = new List<Website>();

        public static void Main(string[] args)
        {
            websites.Add(new RideauActions());
            //websites.Add(new AliExpress());
            websites.Add(new FBMarketplace());

            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            //var fetcher = new AuctionFetcher();
            //await fetcher.GetAuctionItemsAsync();
            //return;

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                //Token = "MTA3MDIxMDUzODc0OTA1NTAzNg.GCWbLv.9BKGzd-yp6UsflGm_DMWxa_V9aeW-c4_EeDTUc", // DEV
                //Token = "OTg2MDgzMTU2OTM3NTY0MjMw.Gye9rZ.3Dv2xDJes2RHGmZXbMy6GJhIi3GqZEJCE6Fn7c", // REAL
                Token = Environment.GetEnvironmentVariable("TOKEN") ?? "INVALID_TOKEN",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            HtmlWeb web = new HtmlWeb();

            discord.MessageCreated += async (s, e) =>
            {
                foreach (Website website in websites)
                {
                    if(website.CheckURL(e.Message.Content))
                    {
                        await website.Respond(e.Message.Content, web, e);
                    }
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}