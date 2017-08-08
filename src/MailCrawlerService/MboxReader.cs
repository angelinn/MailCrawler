using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MailCrawlerService
{
    public class MboxReader
    {
        public int Failed { get; private set; }
        public List<MimeMessage> Read(string mboxFile)
        {
            List<MimeMessage> messages = new List<MimeMessage>();
            FileStream stream = new FileStream(mboxFile, FileMode.Open);
            var parser = new MimeParser(stream, MimeFormat.Mbox);
            while (!parser.IsEndOfStream)
            {
                try
                {
                    messages.Add(parser.ParseMessage());
                }
                catch (Exception e)
                {
                    ++Failed;
                }
            }

            return messages;
        }
    }
}
