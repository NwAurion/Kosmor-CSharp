using System;
using System.Collections.Generic;
using System.IO;
using KosmorMapTool.Model;

namespace KosmorMapTool.Output
{
    class CsvCreator
    {
        static List<Planet> planetList;
        static List<Warplanet> warplanetList;
        static StringWriter writer;

        public void WriteCSV(DateTime date,
                List<Planet> warplanetList, List<Planet> planetList)
        {
            string kosmorDate = KosmorMapTool.getKosmorDate();
            string dateString = KosmorMapTool.dateStringBuilder(kosmorDate, date);
            // Uses the kosmorDate to build the dateString
            FileInfo csvDir = new FileInfo("csv");
            if (!csvDir.Exists)
            {
                csvDir.Create();
            }
            String fileName = csvDir + "/" + dateString + (" - csv -.csv");
            // Uses the dateString to create the fileName
            kosmorDate = kosmorDate.Replace("_", "-");
            writer = new StringWriter();
            writer.Write("# csv for kosmor com day =" + kosmorDate + "\n");
            foreach (Warplanet warplanet in warplanetList)
            {
                writer.Write(warplanet.ToString());
                writer.Write("\n");
            }
            foreach (Planet planet in planetList)
            {
                writer.Write(planet.ToString());
                writer.Write("\n");
            }
            writer.Close();
        }
    }
}
