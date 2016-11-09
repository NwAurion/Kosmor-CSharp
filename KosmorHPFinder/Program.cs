using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KosmorHPFinder
{
    class Program
    {
        static bool sorted = true;

        static int space_power = 12;
        static double square_size = Math.Pow(2, space_power);
        static double[] square_corner = { -1 * Math.Pow(2, space_power - 1), -1* Math.Pow(2, space_power - 1) };

        static LinkedList<double> stack = new LinkedList<double>();

        static LinkedList<string> fileoutput = new LinkedList<string>();

        static int goodfound;
        static int badfound;

        static double[] corner;
        static double gdistsq;
        static double bdistsq;

        static double[] goodxy;
        static double[] badxy;

        static LinkedList<double> yes;
        static LinkedList<double> goodxys = createLinkedList<double>(-383,104, -266,81, -194,149, -297,280, -127,238);
        static LinkedList<double> badxys =createLinkedList<double>(-53,192, -12,140, -101,271, 11,161, 360,141, -64,289, -10,236, 59,281, -48,327, -13,314, -33,221, 75,269, -45,309, 51,315, 62,128, -134,-643, 19,254);
        
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            stack.Unshift(square_corner[0], square_corner[1], square_size);

            //   for (int j = 0; j < goodxys.Count; j += 2)
            for(int j = goodxys.Count-2; j>= 0; j-=2)
            {
         
                if (sorted)
                {
                    if (j < goodxys.Count - 2)
                    {
                        badxys.Push(goodxys.ElementAsArray(j+2));
                    }
                }
                goodxy = goodxys.ElementAsArray(j);
             
                fileoutput.AddLast("<circle cx=" + goodxy[0] + " cy=" + goodxy[1] + " r=\"100\" fill=\"#FF0000\" />");
                for (int k = 0; k < badxys.Count; k += 2)
                {
                    badxy = badxys.ElementAsArray(k);
            
                    fileoutput.AddLast("<circle cx=" + badxy[0] + " cy=" + badxy[1] + " r=\"100\" fill=\"#00FF00\" />");

                    if (!(j == goodxys.Count-2 && k == 0))
                    {
                        stack = yes;
                    }
                    yes = new LinkedList<double>();

                    while (stack.Count > 0)
                    {
                        goodfound = 0;
                        badfound = 0;
                 
                        var tx = stack.Shift();
                        var ty = stack.Shift();
                        var size = stack.Shift();

                        LinkedList<double> corners = createLinkedList(tx, ty, tx + size, ty, tx, ty + size, tx + size, ty + size);
                        for (int i = 0; i < 8; i+=2)
                        {
                            corner = corners.ElementAsArray(i);

                            gdistsq = calc_dist(corner, goodxy);

                            bdistsq = calc_dist(corner, badxy);

                            if (bdistsq < gdistsq)
                            {
                                badfound++;
                            }
                            else
                            {
                                goodfound++;
                            }
                        }

                        if (goodfound == 0)
                        {
                            // whole region excluded
                        }
                        else if (badfound == 0)
                        {
                            // whole region accepted
                            yes.Push(tx, ty, size);
                        }
                        else
                        {
                            // region needs splitting
                            if (size > 2)
                            {
                                stack.Unshift(tx + size / 2, ty + size / 2, size / 2);
                                stack.Unshift(tx + size / 2, ty, size / 2);
                                stack.Unshift(tx, ty + size / 2, size / 2);
                                stack.Unshift(tx, ty, size / 2);
                            }
                            else
                            {
                                // print "cell case to be finished";
                            }
                        }
                    }
                }
            }
            DateTime end = DateTime.Now;
            long dur = (end - start).Ticks / TimeSpan.TicksPerMillisecond;
            Console.WriteLine(dur);
            using (StreamWriter file = new StreamWriter(@"C:\Users\Aurion\Desktop\WriteLines2.svg"))
            {
                file.WriteLine("<svg xmlns=\"http://www.w3.org/2000/svg\">");
                for(int index = 0; index<yes.Count; index+=3)
                {
                    file.WriteLine("<rect x=\""+yes.ElementAt(index)+"\" y=\""+yes.ElementAt(index+1)+"\" width=\""+yes.ElementAt(index+2)+"\" height=\""+yes.ElementAt(index+2)+"\" style=\"fill:darkred\" />");
                }
                file.WriteLine("</svg>");
            }

            Console.ReadKey();
        }

        private static double calc_dist(double[] corner, double[] xy)
        {
            double xdiff = corner[0] - xy[0];
            double ydiff = corner[1] - xy[1];
            return Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2);
        }

        private static LinkedList<T> createLinkedList<T>(params T[] elements)
        {
            LinkedList<T> newList = new LinkedList<T>();
            foreach (T el in elements)
            {
                newList.AddLast(el);
            }

            return newList;
        }
    }
}
