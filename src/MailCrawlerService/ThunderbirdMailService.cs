using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MimeKit;
using System.Text.RegularExpressions;

namespace MailCrawlerService
{
    public class ThunderbirdMailService
    {
        private static readonly string EMAILS_PATH = Path.Combine(Environment.GetEnvironmentVariable("appdata"), "Thunderbird", "Profiles");
        private const string URL_REGEX = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)";

        public string[] GetProfiles()
        {
            return Directory.GetDirectories(EMAILS_PATH);
        }

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
        
        public IEnumerable<string> CrawlMail()
        {
            List<string> urls = new List<string>();
            int failed = 0;
            Regex regex = new Regex(URL_REGEX);
            foreach (string file in GetMailFiles())
            {
                try
                {
                    foreach (MimeMessage message in MboxReader.Read(file))
                    {
                        foreach (Match match in regex.Matches(URL_REGEX))
                            urls.Add(match.Value);
                    }
                }
                catch (Exception e)
                {
                    ++failed;
                    continue;
                }
            }

            return urls;
        }
    }
}
