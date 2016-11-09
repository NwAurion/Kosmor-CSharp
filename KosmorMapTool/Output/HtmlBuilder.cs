
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using KosmorMapTool.Input;
using KosmorMapTool.Model;
using KosmorMapTool.Processing;

namespace KosmorMapTool.Output
{
    class HtmlBuilder
    {
        static List<Planet> planetList;
        static List<Planet> neutralPlanetList;
        static List<Planet> warplanetList;

        public static void initializeHTML(String dateString, StreamWriter fileOut)
        {
            fileOut.WriteLine("<!doctype html>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<html>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<head>");
            importCSSFile(fileOut);
            fileOut.WriteLine("</head>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<body>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<form name=\"form\">");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine(dateString);
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<br>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<br>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<span id='distance12'> </span><br />");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<span id='distance13'> </span><br />");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<span id='distance23'> </span>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<br>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<br>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<table id=\"table1\" class=\"sortable\">");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<thead>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<tr>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<th></th>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<th>Name</th>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<th>XCoord</th>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<th>YCoord</th>");
            fileOut.WriteLine(fileOut.NewLine);
            if (KosmorMapTool.fetchInfo)
            {
                fileOut.WriteLine("<th>Player</a></th>");
                fileOut.WriteLine(fileOut.NewLine);
                fileOut.WriteLine("<th>House</a></th>");
                fileOut.WriteLine(fileOut.NewLine);
            }
            fileOut.WriteLine("<th>Type</th>");
            fileOut.WriteLine(fileOut.NewLine);
            if (KosmorMapTool.fetchInfo)
            {
                fileOut.WriteLine("<th>Combat power</th>");
            }
            else
            {
                fileOut.WriteLine("<th>Ships</th>");
            }
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<th>Moved</th>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<th id='selectedname'>a=</th>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("<th>Dist from a=</th>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("</tr>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("</thead>");
            fileOut.WriteLine(fileOut.NewLine);
        }

        public static void writeHTML(bool doWriteWarplanets, bool doWritePlanets,
            bool writeNeutral, DateTime date, NameValueCollection loginData, bool fetchInfo)
        {
            KosmorMapTool.setKosmorDate(date);
            string kosmorDate = KosmorMapTool.getKosmorDate(); // Gets the kosmorDate

            List<Object> warplanetAndPlanetList = PlanetListBuilder.BuildPlanetList(kosmorDate, date, fetchInfo);
            planetList = (List<Planet>)warplanetAndPlanetList[0];
            warplanetList = (List<Planet>)warplanetAndPlanetList[1];
            neutralPlanetList = (List<Planet>)warplanetAndPlanetList[2];
            string dateString = KosmorMapTool.dateStringBuilder(kosmorDate, date);
            // Uses the kosmorDate to build the dateString

            //StatusMessageHandler.postStatusMessages(10);

            // Making sure the directory exists, if not, is is created
            DirectoryInfo htmldir = new DirectoryInfo("html");
            if (!htmldir.Exists)
            {
                htmldir.Create();
            }

            string fileName = htmldir + "/" + dateString
                    + (" - Planet & WP List - kosmor.com - .html");
            // Uses the dateString to create the fileName

            StreamWriter fileOut = new StreamWriter(fileName);

            //StatusMessageHandler.postStatusMessages(11);

            initializeHTML(dateString, fileOut);

            HttpClientForKosmor.Login(loginData);

            fileOut.WriteLine("<tbody>");
            fileOut.WriteLine(fileOut.NewLine);
            if (doWriteWarplanets)
            {
                writeWarplanets(fetchInfo, warplanetList, fileOut);
            }
            if (doWritePlanets)
            {
                writePlanets(fetchInfo, planetList, fileOut);
            }
            /*
             * if (writeNeutral) { HTMLBuilder.writeNeutral(fetchInfo,
             * HTMLBuilder.neutralPlanetList, out, httpclient); }
             */
            fileOut.WriteLine("</tbody>");
            fileOut.WriteLine(fileOut.NewLine);
            //httpclient.getConnectionManager().shutdown();
            fileOut.WriteLine("</table>");
            fileOut.WriteLine(fileOut.NewLine);
            importJavaScriptFile(fileOut);
            writeScriptInfo(fileOut);
            fileOut.WriteLine("</form>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("</body>");
            fileOut.WriteLine(fileOut.NewLine);
            fileOut.WriteLine("</html>");

            fileOut.Close();

            //StatusMessageHandler.postStatusMessages(15);

            KosmorMapTool.warplanetList = warplanetList;
            KosmorMapTool.planetList = planetList;
            KosmorMapTool.neutralPlanetList = neutralPlanetList;
        }

        public static void writeWarplanets(bool fetchInfo, List<Planet> warplanetList, StreamWriter sw)
        {
            String house;
            String player;
            int combatPower;

            int warplanetListSize = warplanetList.Count;
            String warplanetListSizeString = String.Format("%.0f", warplanetListSize);

            double value = 0;
            String percentage;

            for (int i = 0; i < warplanetListSize; i++)
            {
                Warplanet warplanet = (Warplanet)warplanetList[i];
                String url = createURL(warplanet);
                if (fetchInfo)
                {
                    value = (i / warplanetListSize) * 100;
                    percentage = String.Format("%.2f", value);

                    String[] info = HttpClientForKosmor.ParsePlanets(url, warplanet, true);
                    house = info[0];
                    player = info[1];
                    combatPower = 0;
                    if (warplanet.Ships.Contains("ships"))
                    {
                        combatPower = HttpClientForKosmor.CountShipsOnPlanet(
                                url, warplanet);
                    }
                    warplanet.House = house;
                    warplanet.Owner = player;
                    warplanet.CombatPower = combatPower;
                }
                url = createURL(warplanet);
                String link = "<a href=\"" + url + "\">" + warplanet.Name
                        + "</a>";
                String output = warplanet.toHTMLTable(link, fetchInfo);
                sw.WriteLine(output);
                sw.WriteLine(sw.NewLine);
                sw.WriteLine(sw.NewLine);
            }
            sw.WriteLine(sw.NewLine);
        }

        public static void writePlanets(bool fetchHouse, List<Planet> planetList, StreamWriter sw)
        {
            String house;
            String owner;
            int combatPower;
            int planetListSize = planetList.Count;
            String planetListSizeString = String.Format("%.0f", planetListSize);

            double value = 0;
            String percentage;
            for (int i = 0; i < planetListSize; i++)
            {
                Planet planet = planetList[i];
                String url = HtmlBuilder.createURL(planet);
                if (fetchHouse)
                {
                    value = (i / planetListSize) * 100;
                    percentage = String.Format("%.2f", value);


                    Object[] info = HttpClientForKosmor.ParsePlanets(url, planet, false);
                    house = (String)info[0];
                    owner = (String)info[1];
                    combatPower = 0;
                    if (planet.Ships.Contains("ships"))
                    {
                        combatPower = HttpClientForKosmor.CountShipsOnPlanet(url, planet);
                    }
                    planet.House = house;
                    planet.Owner = owner;
                    planet.CombatPower = combatPower;
                }
                url = HtmlBuilder.createURL(planet);
                String link = "<a href=\"" + url + "\">" + planet.Name + "</a>";
                String output = planet.toHTMLTable(link);
                sw.WriteLine(output);
                sw.WriteLine(sw.NewLine);
                sw.WriteLine(sw.NewLine);
            }

        }

        public static string createURL(Planet planet)
        {
            return "http://www.kosmor.com/index.php?action=3000&xpos=" + planet.XPos + "&ypos=" + planet.YPos;
        }


        private static void writeScriptInfo(StreamWriter sw)
        {
            sw.WriteLine(sw.NewLine);
            sw.WriteLine("<script language=\"javascript\" type=\"text/javascript\">");
            sw.WriteLine(sw.NewLine);
            sw.WriteLine("var table1_Props = { ");
            sw.WriteLine(sw.NewLine);
            sw.WriteLine("			bind_script:{ name:\"autocomplete\", target_fn: setAutoComplete },");
            sw.WriteLine(sw.NewLine);
            sw.WriteLine("			col_0: \"none\",");
            sw.WriteLine(sw.NewLine);
            if (KosmorMapTool.fetchInfo)
            {
                sw.WriteLine("			col_6: \"select\",");
                sw.WriteLine(sw.NewLine);
            }
            else
            {
                sw.WriteLine("			col_4: \"select\",");
                sw.WriteLine(sw.NewLine);
                sw.WriteLine("			col_5: \"select\",");
                sw.WriteLine(sw.NewLine);
            }
            sw.WriteLine(sw.NewLine);
            sw.WriteLine("			rows_counter: true, ");
            sw.WriteLine(sw.NewLine);
            sw.WriteLine("			display_all_text: \"Display all\"");
            sw.WriteLine(sw.NewLine);
            sw.WriteLine("		};");
            sw.WriteLine(sw.NewLine);
            sw.WriteLine("setFilterGrid( \"table1\",table1_Props );");
            sw.WriteLine(sw.NewLine);
            sw.WriteLine("setAlternateRows( \"table1\",table1_Props );");
            sw.WriteLine(sw.NewLine);
            sw.WriteLine("</script>");
            sw.WriteLine(sw.NewLine);
        }

        private static void importCSSFile(StreamWriter sw)
        {
            sw.WriteLine(sw.NewLine);
            sw.Write("<link rel=\"stylesheet\" type=\"text/css\" href=\"../scripts/maptool.css\">");
            sw.WriteLine(sw.NewLine);
        }

        public static void importJavaScriptFile(StreamWriter sw)
        {
            sw.WriteLine(sw.NewLine);
            sw.Write("<script type='text/javascript' src='../scripts/maptool.js'></script>");
            sw.WriteLine(sw.NewLine);
            sw.Write("<script type='text/javascript' src='../scripts/sorting.js'></script>");
            sw.WriteLine(sw.NewLine);
            sw.Write("<script type='text/javascript' src='../scripts/distance.js'></script>");
            sw.WriteLine(sw.NewLine);
            sw.Write("<script type='text/javascript' src='../scripts/dist2.js'></script>");
            sw.WriteLine(sw.NewLine);
        }
    }
}


