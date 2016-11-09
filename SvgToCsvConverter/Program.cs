
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
namespace SvgToCsvConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            HashSet<Planet> planets = new HashSet<Planet>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader reader = XmlReader.Create(new FileStream("C:/Users/Aurion/Desktop/map.svg", FileMode.Open), settings);
           
            int x = 0;
            int y = 0;
            string name = "";
            string color = "";
            Planet planet;
            
            while (reader.Read())
            {
                // NodeType.Text is pure strings, while "text" is the name between the <> brackets. The rest is to filter out irrelevant elements that also starts with <text> like some UI
                if (reader.Name == "text" && reader.NodeType.Equals(XmlNodeType.Element) && reader.AttributeCount == 4 && reader.Depth == 1)
                {
                    x = int.Parse(reader.GetAttribute(0));
                    y = int.Parse(reader.GetAttribute(1));            

                    color = reader.GetAttribute(3).Substring(6);
                    color = color.Substring(0, color.Length - 1);
                }
                // some UI elements in Maelstroems are also text, but have a different depth, so we can easily filter them out
                if (reader.NodeType.Equals(XmlNodeType.Text) && reader.Depth == 2)
                {
                    name = reader.Value.Trim();
                    planet = new Planet(x, y, name, color);
                    planets.Add(planet);
                }
                
            }

            using (StreamWriter file = new StreamWriter(@"C:\Users\Aurion\Desktop\map.txt", false))
            {
                foreach (Planet planetInSet in planets)
                {
                    file.WriteLine(planetInSet.ToCSV());
                }
            }
        }
    }
}
    
    