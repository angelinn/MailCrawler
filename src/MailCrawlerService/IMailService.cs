using System;
using System.Collections.Generic;
using System.Text;

namespace MailCrawlerService
{
    public interface IMailService
    {
        IEnumerable<string> CrawlMail();
        IEnumerable<string> GetAccounts();
        int Failed { get; }
    }
}
