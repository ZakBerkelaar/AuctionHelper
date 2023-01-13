using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHelper
{
    public record class AuctionItem
    {
        public string Name { get; init; }
        public string LotNumber { get; init; }
        public Uri ImageUri { get; init; }

        public decimal CurrentBid { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public int BidCount { get; init; }
        public int Watching { get; init; }
        public decimal StartingBid { get; init; }
        public decimal BidIncrement { get; init; }

        public string Description { get; init; }
    }
}
