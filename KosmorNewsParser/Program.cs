using System;
using System.Net;
using System.Linq;
using NodaTime;
using HtmlAgilityPack;
using System.Collections.Specialized;

namespace KosmorNewsParser
{
    class Program
    {
        static string kosmorDate;
        static long MILLISECONDS_IN_A_DAY = 86400000;
        static string baseTopicURL = "http://www.kosmor.de/forum/viewtopic.php?t=";

        static void Main(string[] args)
        {
            Instant now = SystemClock.Instance.Now;
            ZonedDateTime currentDate = now.InUtc();
            CalculateKosmorDate(currentDate);

            WebClient client = new WebClient();

            string result = client.DownloadString("http://www.kosmor.de/forum/viewforum.php?f=-8");
            string id = FindCurrentId(result);

            GetNewsPost(id);

            Console.ReadKey();

        }

        public static string FindCurrentId(string newsForumHTML)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(newsForumHTML);
            HtmlNode table = doc.DocumentNode.Descendants().Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value =="forumline").ElementAt(0);
            HtmlNode tr = table.ChildNodes[3];
            HtmlNode td = tr.ChildNodes[3];
            HtmlNode span = td.FirstChild;
            string link = span.InnerHtml;
            int start = link.IndexOf("?")+3;
            int end = link.IndexOf("&");
            string id = link.Substring(start, end - start);
            return id;
        }

        public static void GetNewsPost(string id)
        {
            WebClient client = new WebClient();
            string news = client.DownloadString(baseTopicURL + id);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(news);
            Console.Write(news);
        }

        public static void CalculateKosmorDate(ZonedDateTime currentDate)
        {

            LocalDateTime localTime = new LocalDateTime(2004, 2, 1, 0, 0);
            ZonedDateTime startDate = new ZonedDateTime(localTime, DateTimeZone.Utc, Offset.Zero);

            int kosmorDay = 6;
            int kosmorYear = 3500;

            PeriodBuilder pb = new PeriodBuilder();
            pb.Milliseconds = MILLISECONDS_IN_A_DAY;
            Period period = pb.Build();

            ZonedDateTime date = currentDate.Minus(period.ToDuration());

            while (startDate < currentDate)
            {
                startDate = startDate.Plus(period.ToDuration());
                if (currentDate.Calendar.IsLeapYear(currentDate.Year))
                {
                    if (kosmorDay > 366)
                    {
                        kosmorDay = 1;
                        kosmorYear++;
                    }
                    else
                    {
                        kosmorDay++;
                    }
                }
                else if (kosmorDay > 365)
                {
                    kosmorDay = 1;
                    kosmorYear++;
                }
                else
                {
                    kosmorDay++;
                }
            }

            String formattedKosmorDay = kosmorDay.ToString("#000");
            kosmorDate = kosmorYear + "_" + formattedKosmorDay;

        }
    }
}
