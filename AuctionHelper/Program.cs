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

        public static int BuildNum;
        public static string CommitHash;
        public static string CommitHashShort;

        public static void Main(string[] args)
        {
            websites.Add(new RideauActions());
            //websites.Add(new AliExpress());
            websites.Add(new FBMarketplace());
            websites.Add(new Kijiji());

            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            //var fetcher = new AuctionFetcher();
            //await fetcher.GetAuctionItemsAsync();
            //return;

            // Parse build and hash
            if(File.Exists("ver.info"))
            {
                string[] verLines = File.ReadAllLines("ver.info");
                foreach (string s in verLines)
                {
                    string[] split = s.Split('=');

                    if (split.Length != 2)
                        continue;

                    string key = split[0];
                    string value = split[1];

                    if (key == "BUILD_NUM")
                    {
                        BuildNum = int.Parse(value);
                    }
                    else if (key == "COMMIT")
                    {
                        CommitHash = value;
                        CommitHashShort = value[..7];
                    }
                }
            }
            else
            {
                BuildNum = -1;
                CommitHash = "";
                CommitHashShort = "";
            }

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("TOKEN") ?? "INVALID_TOKEN",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents | DiscordIntents.GuildPresences
            });

            HtmlWeb web = new HtmlWeb();

            discord.Ready += async (s, e) =>
            {
                DiscordActivity activity = new DiscordActivity();
                activity.ActivityType = ActivityType.Competing;
                activity.Name = "the top auctions";
                await discord.UpdateStatusAsync(activity, UserStatus.Online);

                foreach (var guild in s.Guilds.Values)
                {
                    var member = await guild.GetMemberAsync(discord.CurrentUser.Id);
                    if (member.Nickname != null && (!Regex.IsMatch(member.Nickname, "Auction Helper \\(#-?\\d+\\)") && member.Nickname != "Auction Helper"))
                        continue; // Only continue if we have an update

                    await member.ModifyAsync(a =>
                    {
                        a.Nickname = $"Auction Helper (#{BuildNum})";
                        a.AuditLogReason = "Bot updated";
                    });
                }
            };

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