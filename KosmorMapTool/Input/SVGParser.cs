using System;
using System.IO;

namespace KosmorMapTool.Input
{
    class SVGParser
    {
        public static Object[] parseSVG(String kosmorDate, DateTime date)
        {

            /* As it has to return two things, it can not return them 
             * directly but has to encapsulate it in another object.
             */
            Object[] bothNodeLists = new Object[2];

            //DocumentBuilderFactory docBuilderFactory = DocumentBuilderFactory.newInstance();
            //DocumentBuilder docBuilder = docBuilderFactory.newDocumentBuilder();

            String SVGFileName = "./" + KosmorMapTool.SVGFileName;
            FileInfo svgMap = new FileInfo(SVGFileName);



            //Document doc = docBuilder.parse(SVGFileName);


            // "text" is the actual planet/warplanet
            Object[] listOfPlanetsAsXMLNode = null; // = doc.getElementsByTagName("text");

            // "line" is the line drawn by a warplanet. Used for calculation of warplanet coordinates.
            Object[] listOfLinesAsXMLNode = null; // = doc.getElementsByTagName("line");
            bothNodeLists[0] = listOfPlanetsAsXMLNode;
            bothNodeLists[1] = listOfLinesAsXMLNode;

            // normalize text representation
            //doc.getDocumentElement().normalize();

            // Build the fileName the .svg should be renamed to, using the dateStringBuilder
            String newSVGFileName = KosmorMapTool.dateStringBuilder(kosmorDate, date);
            newSVGFileName = String.Concat(newSVGFileName, " - Map - kosmor.com - .svg");



            // Making sure the directory does exist, if not, it is created
            DirectoryInfo svgDir = new DirectoryInfo("svg");
            if (!svgDir.Exists)
            {
                svgDir.Create();
            }
            File.Move(svgMap.FullName, Path.Combine(svgDir + "/" + newSVGFileName));


            return bothNodeLists;

        }
    }
}
