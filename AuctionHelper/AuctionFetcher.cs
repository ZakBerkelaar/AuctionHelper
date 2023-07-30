using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AuctionHelper
{
    internal class AuctionFetcher
    {
        private HttpClient httpClient;

        public AuctionFetcher()
        {
            httpClient = new HttpClient();
        }

        public async Task<ICollection<AuctionItem>> GetAuctionItemsAsync()
        {
            HttpContent content = new StringContent(File.ReadAllText("request.txt"), Encoding.UTF8, "application/json");
            var res = await httpClient.PostAsync(@"https://rideau.auctioneersoftware.com/api?crunch=1", content);
            string json = await res.Content.ReadAsStringAsync();

            JObject root = JObject.Parse(json);
            var dataList = root["data"].Children().ToList();
            int auctionRootIndex = dataList[^1]["auction"].Value<int>();
            int lotsObjIndex = dataList[auctionRootIndex]["lots"].Value<int>();
            int lotsIndex = dataList[lotsObjIndex]["lots"].Value<int>();
            var lotIndexList = dataList[lotsIndex].Children().Select(j => j.Value<int>()).ToList();

            JToken Lookup(JToken token, string key) => dataList[token[key]!.Value<int>()]!;

            List<AuctionItem> items = new List<AuctionItem>();
            foreach (int i in lotIndexList)
            {
                var obj = dataList[i];
                var item = new AuctionItem()
                {
                    Name = Lookup(obj, "title").Value<string>(),
                    LotNumber = Lookup(obj, "lot_number").Value<string>(),

                    ImageUri = null,

                    CurrentBid = Lookup(obj, "winning_bid_amount").Value<decimal?>() ?? 0,
                    StartTime = DateTime.Parse(Lookup(obj, "start_time").Value<string>()),
                    EndTime = DateTime.Parse(Lookup(obj, "end_time").Value<string>()),
                    BidCount = Lookup(obj, "bid_count").Value<int>(),
                    Watching = Lookup(obj, "watch_count").Value<int>(),
                    StartingBid = Lookup(obj, "starting_bid").Value<decimal>(),
                    BidIncrement = Lookup(obj, "bid_increment_amount").Value<decimal>(),

                    Description = null
                };

                items.Add(item);
            }

            return items;
        }
    }
}
