using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHelper
{
    internal abstract class Website
    {
        public abstract bool CheckURL(string url);
        public abstract Task Respond(string url, HtmlWeb web, DSharpPlus.EventArgs.MessageCreateEventArgs e);
    }
}
