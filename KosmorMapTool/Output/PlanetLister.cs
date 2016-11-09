using System;
using System.Collections.Generic;
using KosmorMapTool.Model;

namespace KosmorMapTool.Output
{
    class PlanetLister
    {
        public List<Planet> planetList = new List<Planet>();
        public List<Planet> neutralPlanetList = new List<Planet>();

        String kosmorDate;
        DateTime date;

        public PlanetLister(DateTime date, String kosmorDate)
        {
            this.date = date;
            this.kosmorDate = kosmorDate;
        }
    }
}
