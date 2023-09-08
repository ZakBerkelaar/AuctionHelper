using DSharpPlus.EventArgs;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using DSharpPlus.Entities;

namespace AuctionHelper.Websites
{
    internal class FBMarketplace : Website
    {
        private class FBItem
        {
            public string Title { get; init; }
            public string Description { get; init; }
            public decimal Price { get; init; }
            public Uri ImageUri { get; init; }
        }

        private static Regex regex = new Regex(@"(?<!!)https?:\/\/www\.facebook\.com\/marketplace\/item\/\d+\/?.*");

        public override bool CheckURL(string url)
        {
            return regex.IsMatch(url);
        }

        public override async Task Respond(string url, HtmlWeb web, MessageCreateEventArgs e)
        {
            HtmlDocument doc = web.Load(url);
            FBItem item = GetItem(doc);

            await e.Message.RespondAsync(new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder()
                    .AddField("Price", item.Price.ToString())
                    .WithTitle(item.Title)
                    .WithFooter(item.Description)
                    .WithColor(DiscordColor.MidnightBlue)
                    .WithImageUrl(item.ImageUri)));
        }

        private static FBItem GetItem(HtmlDocument doc)
        {
            string name = System.Web.HttpUtility.HtmlDecode(doc.DocumentNode.SelectNodes("//meta[@name='DC.title']").Single().Attributes[1].Value);
            string description = System.Web.HttpUtility.HtmlDecode(doc.DocumentNode.SelectNodes("//meta[@name='DC.description']").Single().Attributes[1].Value);
            string priceJson = doc.DocumentNode.SelectNodes("//script[@type='application/ld+json']")[1].InnerHtml;
            var priceNode = JsonNode.Parse(priceJson)?.AsObject();
            decimal price = decimal.Parse(priceNode?["offers"]?["price"]?.ToString() ?? "-1");
            string imageString = priceNode?["image"]?.ToString() ?? "";

            return new FBItem
            {
                Title = name,
                Description = description,
                Price = price,
                ImageUri = new Uri(imageString),
            };
        }
    }
}
