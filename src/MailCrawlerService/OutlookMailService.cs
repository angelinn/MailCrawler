using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MailCrawlerService
{
    public class OutlookMailService : IMailService
    {
        private const string URL_REGEX = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)";
        public int Failed => 0;

        private Application outlookApplication = new Application();
        private NameSpace outlookNamespace;
        
        public OutlookMailService()
        {
            outlookNamespace = outlookApplication.GetNamespace("MAPI");
        }

        public IEnumerable<string> CrawlMail()
        {
            MAPIFolder inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
            Items mailItems = inboxFolder.Items;

            Regex regex = new Regex(URL_REGEX);
            foreach (var item in mailItems)
            {
                if (item is MailItem)
                {
                    MailItem mail = item as MailItem;
                    foreach (Match match in regex.Matches(mail.Body))
                        yield return match.Value;
                }
            }

        }

        public IEnumerable<string> GetAccounts()
        {
            Accounts accs = outlookNamespace.Accounts;
            foreach (Account item in accs)
            {
                yield return item.SmtpAddress;
            }
        }
    }
}
