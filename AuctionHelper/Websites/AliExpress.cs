﻿using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuctionHelper.Websites
{
    internal class AliExpress : Website
    {
        private class Item
        {

        }

        private static Regex regex = new Regex(@"(?<!!)https?:\/\/(?:www\.)?aliexpress\.com\/item\/.*");

        public override bool CheckURL(string url)
        {
            return regex.IsMatch(url);
        }

        public override async Task Respond(string url, HtmlWeb web, MessageCreateEventArgs e)
        {
            HtmlDocument doc = web.Load(url);
            Item item = GetItem(doc);

            static string FormatRemaining(TimeSpan span)
            {
                return $"{span.Days} days {span.Hours} hours";
            }

            //await e.Message.RespondAsync(new DiscordMessageBuilder()
            //    .WithEmbed(new DiscordEmbedBuilder()
            //        .AddField("Remaining", FormatRemaining(item.EndTime - DateTime.Now), true)
            //        .AddField("Start", item.StartTime.ToString(), true)
            //        .AddField("End", item.EndTime.ToString(), true)
            //        .AddField("Current Bid", item.CurrentBid.ToString("C"), true)
            //        .AddField("Bid Count", item.BidCount.ToString(), true)
            //        .AddField("Watching", item.Watching.ToString(), true)
            //        .AddField("Starting Bid", item.StartingBid.ToString("C"), false)
            //        .AddField("Bid Increment", item.BidIncrement.ToString("C"), false)
            //        .WithTitle($"{item.Name} - {item.LotNumber}")
            //        .WithFooter(item.Description)
            //        .WithColor(DiscordColor.Azure)
            //        .WithImageUrl(item.ImageUri)));
        }

        private static Item GetItem(HtmlDocument doc)
        {
            //string name = doc.DocumentNode.SelectNodes("//div[@class='title']").Single().ChildNodes[0].InnerText;
            //string lotNumber = doc.DocumentNode.SelectNodes("//div[@class='lotNumber']").Single().ChildNodes[1].InnerText;
            //Uri imageUri = new Uri(doc.DocumentNode.SelectNodes("//div[@class='image-gallery-slides']").Single().FirstChild.FirstChild.FirstChild.Attributes[0].Value);
            //decimal curBid = decimal.Parse(doc.DocumentNode.SelectNodes("//div[@class='currentBid']").Single().ChildNodes[1].InnerText[1..^3]);
            //DateTime startTime = DateTime.Parse(doc.DocumentNode.SelectNodes("//div[@class='startTime']").Single().ChildNodes[1].InnerText);
            //DateTime endTime = DateTime.Parse(doc.DocumentNode.SelectNodes("//div[@class='endTime']").Single().ChildNodes[1].InnerText);
            //int bidCount = int.Parse(doc.DocumentNode.SelectNodes("//div[@class='bidCount']").Single().ChildNodes[1].InnerText);
            //int watching = int.Parse(doc.DocumentNode.SelectNodes("//div[@class='watchCount']").Single().ChildNodes[1].InnerText);
            //decimal startingBid = decimal.Parse(doc.DocumentNode.SelectNodes("//div[@class='startingBid']").Single().ChildNodes[1].InnerText[1..]);
            //decimal bidIncrement = decimal.Parse(doc.DocumentNode.SelectNodes("//div[@class='bidIncrement scale']").Single().ChildNodes[2].InnerText[1..]);
            //string description = doc.DocumentNode.SelectNodes("//div[@class='moreDetails']/div[1]/div[1]/div[1]/div[1]").Single().InnerText;

            string name = doc.DocumentNode.SelectNodes("//h1[@class='product-title-text']").Single().InnerText;
            Uri imageUri = new Uri(doc.DocumentNode.SelectNodes("//img[@class='magnifier-image']").First().Attributes[2].Value);
            string priceStr = doc.DocumentNode.SelectNodes("//span[@class='product-price-value']").Single().InnerText;
            string shippingStr = doc.DocumentNode.SelectNodes("//div[@class='dynamic-shipping-line']").Single().FirstChild.FirstChild.FirstChild.InnerText;

            return new Item();
        }
    }
}
