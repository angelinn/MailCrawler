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
        public int Total { get; private set; }
        public IEnumerable<MimeMessage> Read(string mboxFile)
        {
            using (FileStream stream = new FileStream(mboxFile, FileMode.Open))
            {
                MimeParser parser = new MimeParser(stream, MimeFormat.Mbox);
                MimeMessage current = null;
                while (!parser.IsEndOfStream)
                {
                    try
                    {
                        current = parser.ParseMessage();
                    }
                    catch (Exception e)
                    {
                        ++Failed;
                        continue;
                    }
                    yield return current;
                }
            }
        }
    }
}
