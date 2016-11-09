using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using KosmorMapTool.Model;

namespace KosmorMapTool.Input
{
    class HttpClientForKosmor
    {

        static CookieAwareWebClient webClient = new CookieAwareWebClient();

        public static void Login(NameValueCollection loginData)
        {
            Uri uri = new Uri("http://kosmor.com");
            string loginPageAddress = "http://kosmor.com/index.php?action=1";

            string loginString = "player_login="+loginData.GetKey(0) + "&player_password=" + loginData.GetValues(0)[0];
            string fullAddress = loginPageAddress + "&" + loginString;
            string response = webClient.UploadString(fullAddress, "post");

            //webClient.CookieContainer 
            
        }

        public static void DownloadMap()
        {
            string uri = "http://kosmor.com/";
            string file = "draw_map_svg.svg";
            webClient.DownloadFile(uri, file);
        }



        public static int CountShipsOnPlanet(String planetURL, Planet planet)
        {
            string response = webClient.DownloadString(planetURL);
            StringReader sr = new StringReader(response);
            String ln = "";
            string pattern = ".*(H[0-9]).*";

            while ((ln = sr.ReadLine()) != null)
            {
                if (ln.Contains("Ships on<br>") && ln.Contains(planet.Name))
                { // If there are ships on the planet/warplanet it is looking for

                    while ((ln = sr.ReadLine()) != null && Regex.IsMatch(ln, pattern))
                    { // Go through all ships, stop when something
                        for (int i = 0; i < 11; i++)
                        {
                            int ship = i + 1;
                            if (ln.Contains("(H" + ship + ")"))
                            {
                                KosmorMapTool.count[i]++;
                            }
                        }
                        sr.ReadLine(); // Ships use 4 lines in total (including <table> tag etc.
                        sr.ReadLine(); // Therefore, once a ship has been found, every fourth line
                        sr.ReadLine(); // does contain the next ship, if there are still ships left
                    }
                }
            }

            for (int j = 0; j < 11; j++)
            {
                KosmorMapTool.total[j] = KosmorMapTool.total[j] + KosmorMapTool.count[j];
                KosmorMapTool.count[j] = 0;
                KosmorMapTool.totalCombatPower[11] = 0;
                KosmorMapTool.count[11] = 0;
            }

            int totalCombatPower = KosmorMapTool.totalCombatPower.Sum();
            return totalCombatPower;
        }

        public static String[] ParsePlanets(String planetURL, Planet planet, bool isWarplanet)
        {
            string response = webClient.DownloadString(planetURL);
            StringReader sr = new StringReader(response);
            string ln = "";
            string house = "";
            string owner = "";
            while ((ln = sr.ReadLine()) != null)
            {
                if (ln.Contains("t_id"))
                {
                    int id = int.Parse(ln.Substring(ln.IndexOf("t_id") + 5, ln.IndexOf("&", ln.IndexOf("t_id"))));
                    planet.Id = id;
                }
                if (ln.Contains(planet.Name))
                { // If the planet is found..
                    int i = 0;
                    while ((ln = sr.ReadLine()) != null)
                    { // ..then look in the following lines for the rest of the data
                        i++;
                        if (ln.Contains("<td>House:</td>"))
                        { // First planet does use "house"
                            house = ln.Substring(ln.IndexOf("=NormalBold>") + 12, ln.IndexOf("</span>"));
                        }
                        else if (ln.Contains("<td>Haus:</td>"))
                        { // Second (possibly any successive planets) do use "Haus" and another formatting
                            house = ln.Substring(ln.IndexOf("<td>Haus:</td><td>") + 18, ln.IndexOf("</td></tr>"));
                        }
                        if (ln.Contains("<td>Owner:</td>"))
                        { // Getting the owner, need to find the line containing it.
                            try
                            {
                                owner = ln.Substring(ln.IndexOf("<span class=NormalBold>") + 23, ln.IndexOf("</span></td></tr>"));
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                owner = ln.Substring(ln.IndexOf("<b>") + 3, ln.IndexOf("</b></span>"));
                            }
                        }
                        if (ln.Contains("Set waypoint"))
                        { // Getting the real coordinates, instead of the rounded ones from the svg
                            planet.XPos = Double.Parse(ln.Substring(ln.IndexOf("&xpos") + 6, ln.IndexOf("&ypos")));
                            planet.YPos = Double.Parse(ln.Substring(ln.IndexOf("&ypos") + 6, ln.IndexOf("&t_typ")));
                        }
                        if (i > 5)
                        {
                            break;	// Stop after reading all the relevant info
                        }
                    }
                }
            }
            sr.Close();

            if (isWarplanet)
            {
                owner = planet.Name; // The line with the owner has another formatting
                // for warplanets, much easier this way
            }

            return new String[] { house, owner };
        }
    }
}
