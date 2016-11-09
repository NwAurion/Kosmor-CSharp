using System;
using System.Collections.Generic;
using KosmorMapTool.Input;
using KosmorMapTool.Model;

namespace KosmorMapTool.Processing
{
    class PlanetListBuilder
    {

        static int XCOORD_CORRECTION; // Correction factor for the x coordinate of planets, see actual calculation
        static int YCOORD_CORRECTION; // Correction factor for the y coordinate of planets, see actual calculation
        static int XCOORD_CORRECTION_WARPLANET; // Correction factor for the x coordinate of warplanets, see actual calculation
        static int YCOORD_CORRECTION_WARPLANET; // Correction factor for the y coordinate of warplanets, see actual calculation
        static bool is_warplanet;
        static bool didJump;

        static int xPos1, xPos2, yPos1, yPos2;

        static string xCoord1AsString, xCoord2AsString, yCoord1AsString, yCoord2AsString;
        private static List<Object> listOfPlanetsAsXMLNode;
        private static List<Object> listOfLinesAsXMLNode;
        private static int totalPlanets;
        private static int totalLines;

        static List<Warplanet> warplanetList = new List<Warplanet>();
        static List<Planet> neutralPlanetList = new List<Planet>();
        static List<Object> allPlanetLists = new List<Object>();
        static List<Planet> planetList = new List<Planet>();


        public static List<Object> BuildPlanetList(String kosmorDate, DateTime date, Boolean fetchInfo)
        {
            Object[] returnedNodeLists = SVGParser.parseSVG(kosmorDate, date);

            listOfPlanetsAsXMLNode = (List<Object>)returnedNodeLists[0];
            totalPlanets = listOfPlanetsAsXMLNode.Count - 10;
            listOfLinesAsXMLNode = (List<Object>)returnedNodeLists[1];
            totalLines = listOfLinesAsXMLNode.Count - 10;


            List<Object> warplanetAndPlanetList = GenerateAllPlanets(fetchInfo);

            return warplanetAndPlanetList;
        }

        private static void InitializePlanet()
        {

            int currentPosition = 0;
            String tempName = listOfPlanetsAsXMLNode[currentPosition].ToString();
            String name = tempName.Substring(1, listOfPlanetsAsXMLNode[currentPosition].ToString().Length - 1); //.getTextContent().length() - 1);


            // If Lymgat is found, use it to calculate the coordinates. If not, ask for another planet
            if (name.Equals("Lymgat"))
            {
                CalculateCoordinates(currentPosition);
            }
            else
            {
                String somePlanetName = "";

                for (int i = 0; i < listOfPlanetsAsXMLNode.Count; i++)
                {
                    tempName = listOfPlanetsAsXMLNode[i].ToString();
                    name = tempName.Substring(1, listOfPlanetsAsXMLNode[i].ToString().Length - 1);
                    if (name.Equals(somePlanetName))
                    {
                        CalculateCoordinates(i);
                    }
                }
            }
        }

        private static void CalculateCoordinates(int currentPosition)
        {
            String tempXCoord = listOfPlanetsAsXMLNode[currentPosition].ToString(); //.getAttributes().item(2).toString();
            String tempYCoord = listOfPlanetsAsXMLNode[currentPosition].ToString(); //.getAttributes().item(3).toString();

            xCoord2AsString = tempXCoord.Substring(3, tempXCoord.Length - 1);
            yCoord2AsString = tempYCoord.Substring(3, tempYCoord.Length - 1);

            /*
             * The coordinates in the svg are not the actual coordinates, as the
             * .svg uses it's own coordinate system of size 10000*10000, with
             * 0/0 being the top left. Therefore, it has to be adjusted.
             */

            xPos2 = int.Parse(xCoord2AsString);
            yPos2 = int.Parse(yCoord2AsString);

            XCOORD_CORRECTION = xPos2 - 8;
            YCOORD_CORRECTION = yPos2 + 1;

            XCOORD_CORRECTION_WARPLANET = XCOORD_CORRECTION - 1;
            YCOORD_CORRECTION_WARPLANET = YCOORD_CORRECTION + 1;
        }

        private static List<Object> GenerateAllPlanets(bool fetchHouse)
        {
            InitializePlanet();

            int planetCount = 0;

            for (int currentPosition = 0; currentPosition <= totalPlanets; currentPosition++)
            {
                String tempWP = listOfPlanetsAsXMLNode[currentPosition].ToString(); //.getAttributes().item(0).toString();
                String wp = tempWP.Substring(7, tempWP.Length - 1);


                if (wp.Contains("wpname"))
                {
                    planetCount--;
                }
                if (wp.Contains("name"))
                {
                    planetCount++;
                }
            }

            for (int currentPosition = planetCount; currentPosition <= totalPlanets; currentPosition++)
            {
                String tempWP = listOfPlanetsAsXMLNode[currentPosition].ToString(); //.getAttributes().item(0).toString();
                String ships = tempWP.Substring(7, tempWP.Length - 1);
                is_warplanet = true;
                Warplanet warplanet = GenerateSingleWarplanet(currentPosition, ships);
                warplanetList.Add(warplanet);
            }

            for (int currentPosition = 0; currentPosition < planetCount; currentPosition++)
            {
                String tempPlanet = listOfPlanetsAsXMLNode[currentPosition].ToString(); // .getAttributes().item(0).toString();
                String ships = tempPlanet.Substring(7, tempPlanet.Length - 1);
                is_warplanet = false;
                Planet planet = GenerateSinglePlanet(currentPosition, ships);

                string color = planet.Color;
                if (color.Equals("b4b4b4") || color.Equals("dcdcdc") || color.Equals("787878"))
                {
                    planet.House = "neutral";
                    neutralPlanetList.Add(planet);
                }
                else
                {
                    planetList.Add(planet);
                }
            }

            allPlanetLists.Add(planetList);
            allPlanetLists.Add(warplanetList);
            allPlanetLists.Add(neutralPlanetList);
            KosmorMapTool.planetLister.planetList = planetList;
            return allPlanetLists;

        }

        private static Planet GenerateSinglePlanet(int currentPosition, String ships)
        {
            Boolean hasShips = false;

            String tempColor = listOfPlanetsAsXMLNode[currentPosition].ToString(); //getAttributes().item(1).toString();
            String color = tempColor.Substring(13, tempColor.Length - 2);

            String tempName = listOfPlanetsAsXMLNode[currentPosition].ToString(); //.getTextContent();
            String name = tempName.Substring(1, listOfPlanetsAsXMLNode[currentPosition].ToString().Length - 1); //.getTextContent().length() - 1);

            String tempXCoord = listOfPlanetsAsXMLNode[currentPosition].ToString(); //.getAttributes().item(2).toString();
            String tempYCoord = listOfPlanetsAsXMLNode[currentPosition].ToString(); //.getAttributes().item(3).toString();

            xCoord2AsString = tempXCoord.Substring(3, tempXCoord.Length - 1);
            yCoord2AsString = tempYCoord.Substring(3, tempYCoord.Length - 1);

            xPos2 = int.Parse(xCoord2AsString) - XCOORD_CORRECTION;
            yPos2 = int.Parse(yCoord2AsString) - YCOORD_CORRECTION;

            if (ships.Contains("ships"))
            {
                hasShips = true;
            }

            String type = "Planet";
            String csvPlanetType = "p";
            Planet planet = new Planet(name, xPos2, yPos2, color, hasShips, type, csvPlanetType);
            return planet;
        }

        private static Warplanet GenerateSingleWarplanet(int currentPosition, String ships)
        {
            Boolean hasShips = false;
            didJump = false;

            String tempColor = listOfPlanetsAsXMLNode[currentPosition].ToString(); //.getAttributes().item(1).toString();
            String color = tempColor.Substring(13, tempColor.Length - 2);

            String tempName = listOfPlanetsAsXMLNode[currentPosition].ToString(); //.getTextContent();
            String name = tempName.Substring(1, listOfPlanetsAsXMLNode[currentPosition].ToString().Length - 1); //.getTextContent().length() - 1);

            String tempXCoord1 = listOfLinesAsXMLNode[currentPosition - (totalPlanets - totalLines)].ToString(); //.getAttributes().item(1).toString();
            String tempXCoord2 = listOfLinesAsXMLNode[currentPosition - (totalPlanets - totalLines)].ToString(); //.getAttributes().item(2).toString();

            String tempYCoord1 = listOfLinesAsXMLNode[currentPosition - (totalPlanets - totalLines)].ToString(); //getAttributes().item(3).toString();
            String tempYCoord2 = listOfLinesAsXMLNode[currentPosition - (totalPlanets - totalLines)].ToString(); //getAttributes().item(4).toString();

            xCoord1AsString = tempXCoord1.Substring(4, tempXCoord1.Length - 1);
            yCoord1AsString = tempYCoord1.Substring(4, tempYCoord1.Length - 1);

            xCoord2AsString = tempXCoord2.Substring(4, tempXCoord2.Length - 1);
            yCoord2AsString = tempYCoord2.Substring(4, tempYCoord2.Length - 1);

            xPos1 = int.Parse(xCoord1AsString);
            yPos1 = int.Parse(yCoord1AsString);

            xPos2 = int.Parse(xCoord2AsString);
            yPos2 = int.Parse(yCoord2AsString);

            xPos1 = xPos1 - XCOORD_CORRECTION_WARPLANET;
            yPos1 = yPos1 - YCOORD_CORRECTION_WARPLANET;

            xPos2 = xPos2 - XCOORD_CORRECTION_WARPLANET;
            yPos2 = yPos2 - YCOORD_CORRECTION_WARPLANET;


            double a = (xPos1 - xPos2);
            double b = (yPos1 - yPos2);
            double distanceTraveled = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));

            if (distanceTraveled > 61)
            {
                didJump = true;
            }

            if (ships.Contains("ships"))
            {
                hasShips = true;
            }

            String type = "Warplanet";
            String csvPlanetType = "w";
            Warplanet warplanet = new Warplanet(name, xPos2, yPos2, color, hasShips, didJump, distanceTraveled, xPos1, yPos1, type, csvPlanetType);
            return warplanet;
        }
    }
}
