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

namespace AuctionHelper.Websites
{
    internal class FBMarketplace : Website
    {
        private class FBItem
        {
            public string Title { get; init; }
            public string Description { get; init; }
            public decimal Price { get; init; }
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
            
        }

        private static FBItem GetItem(HtmlDocument doc)
        {
            string name = doc.DocumentNode.SelectNodes("//meta[@name='DC.title']").Single().Attributes[1].Value;
            string description = doc.DocumentNode.SelectNodes("//meta[@name='DC.description']").Single().Attributes[1].Value;
            var priceNode = JsonNode.Parse(doc.DocumentNode.SelectNodes("//meta[@name='DC.description']").Single().Attributes[1].Value)?.AsObject();
            decimal price = decimal.Parse(priceNode?["offers"]?["price"]?.ToString() ?? "-1");

            return new FBItem
            {
                Title = name,
                Description = description,
                Price = price,
            };
        }
    }
}
