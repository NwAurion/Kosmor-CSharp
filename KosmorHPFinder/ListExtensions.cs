using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KosmorHPFinder
{
    static class ListExtensions
    {

        public static int Unshift<T>(this LinkedList<T> list, params T[] param)
        {
            LinkedList<T> temp = new LinkedList<T>();
            foreach (var elem in param)
            {
                temp.AddFirst(elem);
            }
            foreach (var elem in temp)
            {
                list.AddFirst(elem);
            }
            return list.Count;
        }

        public static T Shift<T>(this LinkedList<T> list)
        {
            var elem = list.First.Value;
            list.RemoveFirst();
            return elem;
        }

        public static T[] ElementAsArray<T>(this LinkedList<T> list, int index){
            var first = list.ElementAt(index);
            var second = list.ElementAt(index + 1);
            T[] elem = new T[]{first, second};
            return elem;
        }

        public static int Push<T>(this LinkedList<T> list, params T[] param)
        {
            foreach(var elem in param){
                list.AddLast(elem);
            }
            return list.Count;
        }
    }
}
