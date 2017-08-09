using MailCrawlerService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MailCrawler.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private IMailService mailService;
        public List<string> MailProviders { get; set; } = new List<string>()
        {
            "Outlook",
            "Thunderbird",
            "Postbox"
        };

        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                NotifyPropertyChanged();
            }
        }

        private string status;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                NotifyPropertyChanged();
            }
        }

        private string links;
        public string Links
        {
            get
            {
                return links;
            }
            set
            {
                links = value;
                NotifyPropertyChanged();
            }
        }

        private string selectedMail;
        public string SelectedMail
        {
            get
            {
                return selectedMail;
            }
            set
            {
                selectedMail = value;
                NotifyPropertyChanged();

                try
                {
                    switch (selectedMail)
                    {
                        case "Outlook":
                            mailService = new OutlookMailService();
                            break;
                        case "Thunderbird":
                        case "PostBox":
                            mailService = new ThunderbirdMailService();
                            break;
                        default:
                            break;
                    }
                    IEnumerable<string> accounts = mailService.GetAccounts();
                    Accounts = $"Found {accounts.Count()} account(s). ";
                    HasAccounts = true;
                }
                catch (Exception e)
                {
                    Accounts = "No accounts found.";
                    HasAccounts = false;
                }
            }
        }

        private bool hasAccounts;
        public bool HasAccounts
        {
            get
            {
                return hasAccounts;
            }
            set
            {
                hasAccounts = value;
                NotifyPropertyChanged();
            }
        }
        
        private string accounts;
        public string Accounts
        {
            get
            {
                return accounts;
            }
            set
            {
                accounts = value;
                NotifyPropertyChanged();
            }
        }

        public List<string> FetchedLinks { get; set; } = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            SelectedMail = MailProviders[1];
            DataContext = this;
        }

        private CancellationTokenSource token;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            token = new CancellationTokenSource();
            svScroller.ScrollToEnd();
            Task.Run(async () =>
            {
                int total = 0;
                IsLoading = true;
                foreach (string link in mailService.CrawlMail())
                {
                    if (token.IsCancellationRequested)
                        break;

                    Links += $"{link}\n";
                    Status = $"{mailService.Failed} failed.";
                    ++total;
                    if (total > 100)
                    {
                        Links = String.Empty;
                        total = 0;
                    }
                    FetchedLinks.Add(link);
                    await Task.Delay(50);
                }
                IsLoading = false;
            }, token.Token);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            token.Cancel();
        }
    }
}
