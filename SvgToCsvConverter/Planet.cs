
namespace SvgToCsvConverter
{
    class Planet
    {
        int x, y;
        string name, color;

        public Planet(int x, int y, string name, string color)
        {
            this.x = x;
            this.y = y;
            this.name = name;
            this.color = color;
        }

        public string ToCSV()
        {
            return x + "," + y + "," + name + "," + color;
        }
    }
}
