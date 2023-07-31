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
    internal class Kijiji : Website
    {
        private class KjItem
        {
            public string Title { get; init; }
            public string Description { get; init; }
            public decimal Price { get; init; }
            public Uri ImageUri { get; init; }
        }

        private static Regex regex = new Regex(@"(?<!!)https?:\/\/www\.kijiji\.ca\/v-.*");

        public override bool CheckURL(string url)
        {
            return regex.IsMatch(url);
        }

        public override async Task Respond(string url, HtmlWeb web, MessageCreateEventArgs e)
        {
            HtmlDocument doc = web.Load(url);
            KjItem item = GetItem(doc);

            await e.Message.RespondAsync(new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder()
                    .AddField("Price", item.Price.ToString())
                    .WithTitle(item.Title)
                    .WithFooter(item.Description)
                    .WithColor(DiscordColor.Purple)
                    .WithImageUrl(item.ImageUri)));
        }

        private static KjItem GetItem(HtmlDocument doc)
        {
            string name = doc.DocumentNode.SelectNodes("//h1[@class='title-2323565163']").Single().InnerText;
            string description = doc.DocumentNode.SelectNodes("//meta[@property='og:description']").Single().Attributes[1].Value;
            string imageString = doc.DocumentNode.SelectNodes("//meta[@property='og:image']").Single().Attributes[1].Value ?? "";
            string priceString = doc.DocumentNode.SelectNodes("//div[@class='priceContainer-1419890179']")[0].InnerText.TrimStart('$');
            decimal price = decimal.Parse(priceString ?? "-1");

            return new KjItem
            {
                Title = name,
                Description = description,
                Price = price,
                ImageUri = new Uri(imageString),
            };
        }
    }
}