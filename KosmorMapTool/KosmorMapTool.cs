
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using KosmorMapTool.Input;
using KosmorMapTool.Model;
using KosmorMapTool.Output;
namespace KosmorMapTool
{
    class KosmorMapTool
    {
        static double VERSION = 2.02;

        static long MILLISECONDS_IN_A_DAY = 86400000;

        public static int[] COMBAT_POWER = new int[] { 9, 13, 19, 41, 65, 116, 188, 310, 528, 916, 3306 };
        public static int[] totalCombatPower = new int[12];
        public static int[] count = new int[12];
        public static int[] total = new int[11];


        static long startTime = 0;
        static long endTime = 0;

        // Write warplanets to the html
        public static bool writeWarplanets = false;
        // Write planets with owners to the html
        public static bool writePlanets = false;
        // Write neutral planets to the html
        public static bool writeNeutral = false;

        public static bool fetchInfo = false;

        private static bool updateHTML = false;
        private static bool useCustomDate = false;
        private static DateTime date;
        private static bool writeCSV = false;

        public static string SVGFileName = "draw_map_svg.svg";

        static FileInfo settings = new FileInfo("settings.ini");
        static DateTimeFormatInfo dtfi = new DateTimeFormatInfo();

        //public static DateFormat dateFormat = new DateFormat("HH:mm:ss");
        static String kosmorDate;

        public static List<Planet> warplanetList;
        public static List<Planet> planetList;
        public static List<Planet> neutralPlanetList;

        public static PlanetLister planetLister;

        public KosmorMapTool()
        {
            dtfi.ShortTimePattern = "HH:mss:ss";
            dtfi.ShortDatePattern = "dd.mm.yyyy HH";
            TimeZoneInfo tz = TimeZoneInfo.Utc;
        }

        static void Main(string[] args)
        {
            String argsString = args.ToString();
            NameValueCollection loginData = new NameValueCollection();
            // If the settings file does exist, use it
            if (settings.Exists)
            {
                loginData = readSettings();
            }
            else
            {
                loginData = readArgumentList(argsString);
            }

   /*         if (KosmorMapTool.fetchInfo)
            {
               HttpClientForKosmor.readPlayersFromKosmorOnlineTools();
               HttpClientForKosmor.readHousesFromKosmorOnlineTools(0);
            }*/

            if (KosmorMapTool.updateHTML)
            {
                DirectoryInfo path = new DirectoryInfo(".");
                FileInfo[] files = path.GetFiles(); // Get all files
                foreach (FileInfo file in files)
                { // Go through all files
                    string pattern = ".*[0-9][0-9][.][0-9][0-9].[0-9].*";
                    if (Regex.IsMatch(file.FullName, pattern))
                    {
                        // Matches to date like "12.08.2011" Should probably add
                        // .svg extension?
                        SVGFileName = file.FullName; // Set the current file

                        // Parse the file name for the date
                        date = DateTime.Parse(file.FullName.Substring(9, 19));


                        // from the file name
                        //runMapTool(loginData);
                        // Run the map tool with every file once
                    }
                }
            }
            if (!KosmorMapTool.updateHTML)
            {
                KosmorMapTool.runMapTool(loginData);
                // If the user does not want to update, just run it normally
            }
        }

        public static void runMapTool(NameValueCollection loginData)
        {
            FileInfo svgMap = new FileInfo(SVGFileName);

            startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            // If the map does not exists, try to download it

            if (!svgMap.Exists)
            {
                HttpClientForKosmor.Login(loginData);
                HttpClientForKosmor.DownloadMap();
            }
            /*
             * If the -date parameter is used, the user has to input the date he
             * wishes to use in place of the current date
             */
            /*		if (useCustomDate) {
                            System.out.println("Please set the date");
                            BufferedReader reader = new BufferedReader(
                                    new InputStreamReader(System.in));
                            SimpleDateFormat df = new SimpleDateFormat("dd.MM.yyyy HH");
                            MapTool.date = df.parse(reader.readLine());
                            cal = MapTool.dateToGMTDate(MapTool.date);
                        }*/



            planetLister = new PlanetLister(date, getKosmorDate());
            HtmlBuilder.writeHTML(writeWarplanets, writePlanets,
                    writeNeutral, date,
                    loginData, fetchInfo);

            if (writeCSV)
            {
                CsvCreator csvCreator = new CsvCreator();
                csvCreator.WriteCSV(date, warplanetList, planetList);
            }

            //XMLParserWriter.serialize(MapTool.planetLister);

            endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long time = (endTime - startTime) / 1000;
        }



        private static NameValueCollection readSettings()
        {
            String player_login = "";
            String player_password = "";

            String line;
            StreamReader sr = new StreamReader(settings.FullName);
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("warplanets"))
                {
                    writeWarplanets = Boolean.Parse(line.Substring(line
                            .IndexOf("=") + 1));
                }
                if (line.StartsWith("planets"))
                {
                    writePlanets = Boolean.Parse(line.Substring(line
                            .IndexOf("=") + 1));
                }
                if (line.StartsWith("neutral"))
                {
                    writeNeutral = Boolean.Parse(line.Substring(line
                            .IndexOf("=") + 1));
                }
                if (line.StartsWith("login"))
                {
                    player_login = line.Substring(line.IndexOf("login") + 6);
                }
                if (line.StartsWith("password"))
                {
                    player_password = line
                            .Substring(line.IndexOf("password") + 9);
                }
                if (line.StartsWith("fetchinfo"))
                {
                    fetchInfo = Boolean.Parse(line.Substring(line
                            .IndexOf("=") + 1));
                }
                if (line.StartsWith("updatehtml"))
                {
                    updateHTML = Boolean.Parse(line.Substring(line
                            .IndexOf("=") + 1));
                }
                /*			if (line.startsWith("gui")) {
                                GUI = Boolean
                                        .valueOf(line.Substring(line.IndexOf("=") + 1));
                            }*/
                if (line.StartsWith("date"))
                {
                    useCustomDate = Boolean.Parse(line.Substring(line
                            .IndexOf("=") + 1));
                }
                /*			if (line.StartsWith("checkforupdate")) {
                                checkForUpdate = line.Substring(line
                                        .IndexOf("checkforupdate") + 15);
                            }*/
                if (line.StartsWith("csv"))
                {
                    writeCSV = Boolean
                            .Parse(line.Substring(line.IndexOf("=") + 1));
                }
            }
            sr.Close();
            
            NameValueCollection loginData = new NameValueCollection();
            loginData.Add(player_login, player_password);
            return loginData;
        }

        private static NameValueCollection readArgumentList(String argsString)
        {
            String player_login = "";
            String player_password = "";

            if (argsString.Contains("-warplanets"))
            {
                writeWarplanets = true;
            }
            if (argsString.Contains("-planets"))
            {
                writePlanets = true;
            }
            if (argsString.Contains("-neutral"))
            {
                writeNeutral = true;
            }
            if (argsString.Contains("-login"))
            {
                player_login = argsString.Substring(
                        argsString.IndexOf("-login") + 8,
                        argsString.IndexOf("-", argsString.IndexOf("-login") + 1));
                player_login = player_login.Substring(0, player_login.Length - 2);
            }
            if (argsString.Contains("-password"))
            {
                player_password = argsString.Substring(argsString
                        .IndexOf("-password") + 11, argsString.IndexOf("-",
                                argsString.IndexOf("-password") + 1));
                player_password = player_password.Substring(0,
                        player_password.Length - 2);
            }
            if (argsString.Contains("-fetchinfo"))
            {
                fetchInfo = true;
            }
            if (argsString.Contains("-updatehtml"))
            {
                updateHTML = true;
            }
            /*            if (argsString.Contains("-gui"))
                        {
                            GUI = true;
                        }*/
            if (argsString.Contains("-date"))
            {
                useCustomDate = true;
            }
            /*            if (argsString.Contains("-checkforupdate"))
                        {
                            checkForUpdate = argsString.Substring(argsString
                                    .IndexOf("-checkforupdate") + 16);
                        }*/
            NameValueCollection loginData = new NameValueCollection();
            loginData.Add(player_login, player_password);
            return loginData;
        }


        public static String dateStringBuilder(String kosmorDate, DateTime date)
        {
            String dayAsString;
            String monthAsString;

            int day = date.Day;
            int month = date.Month;
            int year = date.Year;

            dayAsString = date.Day.ToString();
            monthAsString = date.Month.ToString();

            if (dayAsString.Length < 2)
            {
                dayAsString = string.Concat("0", dayAsString);
            }
            if (monthAsString.Length < 2)
            {
                monthAsString = string.Concat("0", monthAsString);
            }

            String dateString = kosmorDate + "(" + dayAsString + "."
                    + monthAsString + "." + year + ")";
            return dateString;
        }

        public static void setKosmorDate(DateTime currentDate)
        {
            DateTime startDate = new DateTime(2004, 1, 2, 8, 0, 0, DateTimeKind.Utc);
            //Date startDate = startCalendar.getTime();

            /* 
             * TODO was 7 for most of the time (from the start until i ceased to use it in 2012).
             * Somehow, the day is off by one (it's behind the actual kosmor date) now (18.06.2014) 
             */
            int kosmorDay = 8;
            int kosmorYear = 3500;

            while (startDate.Ticks < currentDate.Ticks -
                    -KosmorMapTool.MILLISECONDS_IN_A_DAY)
            {
                startDate.AddMilliseconds(MILLISECONDS_IN_A_DAY);
                bool isLeapYear = DateTime.IsLeapYear(currentDate.Year - 1);
                if (isLeapYear)
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

            String formattedKosmorDay = String.Format("%03d", kosmorDay);
            String kosmorDate = kosmorYear + "_" + formattedKosmorDay;
        }

        public static String getKosmorDate()
        {
            if (kosmorDate == null)
            {
                setKosmorDate(new DateTime());
            }
            return kosmorDate;
        }

        public static String getTime()
        {
            DateTime date = DateTime.SpecifyKind(new DateTime(), DateTimeKind.Utc);
            return String.Format(dtfi.ShortTimePattern, date);
        }
    }
}
