using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MimeKit;
using System.Text.RegularExpressions;

namespace MailCrawlerService
{
    public class ThunderbirdMailService : IMailService
    {
        private static readonly string EMAILS_PATH = Path.Combine(Environment.GetEnvironmentVariable("appdata"), "Thunderbird", "Profiles");
        private const string URL_REGEX = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)";
        private MboxReader reader = new MboxReader();
        public string[] GetProfiles()
        {
            return Directory.GetDirectories(EMAILS_PATH);
        }

        public int Failed => reader.Failed;

        public List<string> GetMailFiles()
        {
            List<string> mailFiles = new List<string>();

            string[] profiles = GetProfiles();
            foreach (string profile in profiles)
            {
                string[] imapDirs = Directory.GetDirectories(Path.Combine(profile, "ImapMail"));
                foreach (string dir in imapDirs)
                    mailFiles.AddRange(Directory.GetFiles(dir, "*."));

                string pop3Dir = Path.Combine(profile, "Mail", "Local Folders");
                mailFiles.AddRange(Directory.GetFiles(pop3Dir, "*."));
            }

            return mailFiles;
        }

        public IEnumerable<string> GetAccounts()
        {
            return GetProfiles();
        }

        public IEnumerable<string> CrawlMail()
        {
            reader = new MboxReader();
            
            Regex regex = new Regex(URL_REGEX);
            foreach (string file in GetMailFiles())
            {
                foreach (MimeMessage message in reader.Read(file))
                {
                    string body = (!String.IsNullOrEmpty(message.HtmlBody)) ? message.HtmlBody : message.Body.ToString();
                    foreach (Match match in regex.Matches(body))
                        yield return match.Value;
                }
            }
        }
    }
}
