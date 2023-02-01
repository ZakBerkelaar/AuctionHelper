using DSharpPlus;
using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using DSharpPlus.Entities;
using System.Net;

namespace AuctionHelper
{
    public static class Program
    {
        //private static Regex regex = new Regex(@"https?:\/\/rideau\.auctioneersoftware.com\/auctions\/\d+\/lot\/\d+-.+");
        private static Regex regex = new Regex(@"(?<!!)https?:\/\/rideau\.auctioneersoftware\.com\/auctions\/\d+\/lot\/\d+-.+");

        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "MTA3MDIxMDUzODc0OTA1NTAzNg.GCWbLv.9BKGzd-yp6UsflGm_DMWxa_V9aeW-c4_EeDTUc", // DEV
                //Token = "OTg2MDgzMTU2OTM3NTY0MjMw.Gye9rZ.3Dv2xDJes2RHGmZXbMy6GJhIi3GqZEJCE6Fn7c", // REAL
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            HtmlWeb web = new HtmlWeb();

            discord.MessageCreated += async (s, e) =>
            {
                Match match = regex.Match(e.Message.Content);
                if(match.Success)
                {
                    static string FormatRemaining(TimeSpan span)
                    {
                        return $"{span.Days} days {span.Hours} hours";
                    }

                    //HtmlDocument doc = new HtmlDocument();
                    //doc.Load(@"D:\Libraries\Downloads\Plaid scarf - Rideau Auctions.html");
                    HtmlDocument doc = web.Load(match.Value);
                    AuctionItem item = GetAuctionItem(doc);
                    await e.Message.RespondAsync(new DiscordMessageBuilder()
                        .WithEmbed(new DiscordEmbedBuilder()
                            .AddField("Remaining", FormatRemaining(item.EndTime - DateTime.Now), true)
                            .AddField("Start", item.StartTime.ToString(), true)
                            .AddField("End", item.EndTime.ToString(), true)
                            .AddField("Current Bid", item.CurrentBid.ToString("C"), true)
                            .AddField("Bid Count", item.BidCount.ToString(), true)
                            .AddField("Watching", item.Watching.ToString(), true)
                            .AddField("Starting Bid", item.StartingBid.ToString("C"), false)
                            .AddField("Bid Increment", item.BidIncrement.ToString("C"), false)
                            .WithTitle($"{item.Name} - {item.LotNumber}")
                            .WithFooter(item.Description)
                            .WithColor(DiscordColor.Azure)
                            .WithImageUrl(item.ImageUri)));
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static AuctionItem GetAuctionItem(HtmlDocument doc)
        {
            string name = doc.DocumentNode.SelectNodes("//div[@class='title']").Single().ChildNodes[0].InnerText;
            string lotNumber = doc.DocumentNode.SelectNodes("//div[@class='lotNumber']").Single().ChildNodes[1].InnerText;
            //Uri imageUri = new Uri($@"https://auctioneersoftware.s3.amazonaws.com/rid/2021/2/medium/{urlRegex.Match(doc.DocumentNode.SelectNodes("//div[@class='image-gallery-slides']").Single().FirstChild.FirstChild.FirstChild.Attributes[0].Value).Groups[1].Value}");
            Uri imageUri = new Uri(doc.DocumentNode.SelectNodes("//div[@class='image-gallery-slides']").Single().FirstChild.FirstChild.FirstChild.Attributes[0].Value);
            decimal curBid = decimal.Parse(doc.DocumentNode.SelectNodes("//div[@class='currentBid']").Single().ChildNodes[1].InnerText[1..^3]);
            DateTime startTime = DateTime.Parse(doc.DocumentNode.SelectNodes("//div[@class='startTime']").Single().ChildNodes[1].InnerText);
            DateTime endTime = DateTime.Parse(doc.DocumentNode.SelectNodes("//div[@class='endTime']").Single().ChildNodes[1].InnerText);
            int bidCount = int.Parse(doc.DocumentNode.SelectNodes("//div[@class='bidCount']").Single().ChildNodes[1].InnerText);
            int watching = int.Parse(doc.DocumentNode.SelectNodes("//div[@class='watchCount']").Single().ChildNodes[1].InnerText);
            decimal startingBid = decimal.Parse(doc.DocumentNode.SelectNodes("//div[@class='startingBid']").Single().ChildNodes[1].InnerText[1..]);
            decimal bidIncrement = decimal.Parse(doc.DocumentNode.SelectNodes("//div[@class='bidIncrement scale']").Single().ChildNodes[2].InnerText[1..]);
            string description = doc.DocumentNode.SelectNodes("//div[@class='moreDetails']/div[1]/div[1]/div[1]/div[1]").Single().InnerText;

            return new AuctionItem()
            {
                Name = name,
                LotNumber = lotNumber,
                ImageUri = imageUri,
                CurrentBid = curBid,
                StartTime = startTime,
                EndTime = endTime,
                BidCount = bidCount,
                Watching = watching,
                StartingBid = startingBid,
                BidIncrement = bidIncrement,
                Description = description
            };
        }
    }
}