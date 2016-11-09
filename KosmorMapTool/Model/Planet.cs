using System;
using System.Collections.Generic;

namespace KosmorMapTool.Model
{
    class Planet
    {
        public int Id;
        public int CombatPower;
        public double XPos;
        public double YPos;
        public String Name;
        public String Color;
        public String Owner = "";
        public String House = "unknown";
        public String Ships;
        public String CsvPlanetType;
        public String Type;


        public static List<String> player_id_list; // = HTTPClientForKosmor.player_id_list;
        public static List<String> player_name_list; // = HTTPClientForKosmor.player_name_list;
        public static List<String> house_id_list; // = HTTPClientForKosmor.house_id_list;
        public static List<String> house_name_list; // = HTTPClientForKosmor.house_name_list;


        public Planet(String name, double x, double y, String color, Boolean ships, String type, String csvPlanetType)
        {
            this.Name = name;
            XPos = x;
            YPos = y;
            this.Color = color;
            if (ships == true)
            {
                this.Ships = "ships";
            }
            else { this.Ships = ""; }
            this.Type = type;
            this.CsvPlanetType = csvPlanetType;
        }

        public String toHTMLTable(String link)
        {
            String output;
            output = "<tr>" + "\n";
            output += "<td><input type='radio' name='p1' value='" + Name + "' onclick=\"DoDistance();\"/>" + "\n" +
                    "<input type='radio' name='p2' value='" + Name + "' onclick=\"DoDistance();\"/>" + "\n" +
                    "<input type='radio' name='p3' value='" + Name + "' onclick=\"DoDistance();\"/>" + "\n";
            output += "<td id='" + Name + "'>" + link + "\n" +
                    "<td id='" + Name + "~x'>" + (int)XPos + "\n" +
                    "<td id='" + Name + "~y'>" + (int)YPos + "\n";
            if (KosmorMapTool.fetchInfo)
            {
                if (Planet.player_name_list.Contains(Owner))
                {
                    output += "<td>"
                            + "<a href=\""
                            + Planet.player_id_list[Planet.player_name_list
                                    .IndexOf(Owner)] + "\">" + Owner + "</a>"
                                    + "\n";
                }
                else
                {
                    output += "<td>" + Owner + "\n";
                }
                if (Planet.house_name_list.Contains(House))
                {
                    output += "<td>" + "<a href=\"" + Planet.house_id_list[Planet.house_name_list.IndexOf(House)] + "\">" + House + "</a>" + "\n";
                }
                else
                {
                    output += "<td>" + House + "\n";
                }
                output += "<td>" + "<font color=#" + Color + ">" + Type + "</font>" + "\n";
                if (CombatPower > 0)
                {
                    output += "<td>" + CombatPower + "\n";
                }
                else
                {
                    output += "<td>" + "" + "\n";
                }
            }
            else
            {
                output +=
                 "<td>" + "<font color=#" + Color + ">" + Type + "</font>" + "\n" +
                         "<td>" + Ships + "\n";
            }
            output += "<td>" + "\n";
            output += "<td><input type='radio' name='dist' value='" + Name + "' onchange=\"DistTool2();\" />" + "\n";
            output += "<td id='" + Name + "~d'>" + "\n";
            output += "</tr>";
            return output;
        }
    }
}
