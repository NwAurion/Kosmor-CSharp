using System;
using System.Globalization;

namespace KosmorMapTool.Model
{
    class Warplanet : Planet
    {
        Boolean DidJump;
        Double DistanceTraveled;
        double XPosPrevious;
        double YPosPrevious;

        public Warplanet(String name, int x, int y, String color, Boolean ships, Boolean didJump, double distanceTraveled, int xPrevious, int yPrevious, String type, String csvPlanetType)
            : base(name, x, y, color, ships, type, csvPlanetType)
        {
            this.DidJump = didJump;
            this.DistanceTraveled = distanceTraveled;
            this.XPosPrevious = xPrevious;
            this.YPosPrevious = yPrevious;
        }

        public String toHTMLTable(String link, bool fetchInfo)
        {
            String format;

            // Ensuring the separator is a dot (31.092 instead of 31,092)
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            if (fetchInfo)
            {
                format = "#0.0";

                // formatter = new DecimalFormat("#0.0"); // Using one decimal place
            }
            else
            {
                format = "#0";
                //formatter = new DecimalFormat("#0"); // SVG coords are integer to begin with, no point in displaying decimal places
            }

            String xPosAsString = XPos.ToString(format, nfi);
            String yPosAsString = YPos.ToString(format, nfi);
            String output;
            output = "<tr>" + "\n";
            output += "<td><input type='radio' name='p1' value='" + Name + "'"
                    + " onclick=\"DoDistance();\"/>" + "\n"
                    + "<input type='radio' name='p2' value='" + Name + "' "
                    + "onclick=\"DoDistance();\"/>" + "\n"
                    + "<input type='radio' name='p3' value='" + Name + "' "
                    + "onclick=\"DoDistance();\"/>" + "\n";
            output += "<td id='" + Name + "'>" + link + "\n" +
                    "<td id='" + Name + "~x'>" + xPosAsString + "\n" +
                    "<td id='" + Name + "~y'>" + yPosAsString + "\n";

            if (KosmorMapTool.fetchInfo)
            {
                if (Planet.player_name_list.Contains(this.Owner))
                {
                    output += "<td>" + "<a href=\"" + Planet.player_id_list[Planet.player_name_list.
                            IndexOf(this.Owner)] + "\">" + this.Owner + "</a>" + "\n";
                }
                else
                {
                    output += "<td>" + this.Owner + "\n";
                }
                if (Planet.house_name_list.Contains(this.House))
                {
                    output += "<td>" + "<a href=\"" + Planet.house_id_list[Planet.house_name_list.
                            IndexOf(this.House)] + "\">" + this.House + "</a>" + "\n";
                }
                else
                {
                    output += "<td>" + this.House + "\n";
                }
                output += "<td>" + "<font color=#" + this.Color + ">" + this.Type + "</font>" + "\n";
                output += "<td>" + this.CombatPower + "\n";
            }
            else
            {
                output +=
                        "<td>" + "<font color=#" + this.Color + ">" + this.Type + "</font>" + "\n" +
                                "<td>" + this.Ships + "\n";
            }
            output += "<td>" + Convert.ToInt32(this.DistanceTraveled) + "\n";
            output += "<td><input type='radio' name='dist' value='" + this.Name
                    + "' " + "onchange=\"DistTool2();\" />" + "\n";
            output += "<td id='" + this.Name + "~d'>" + "\n";
            output += "</tr>" + "\n";
            return output;
        }
    }
}
