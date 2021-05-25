using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Player
{
    public static class Extensions
    {
        public static List<T> Shuffle<T>(this IEnumerable<T> array)
        {
            List<T> list = new(array);

            Random rnd = new();
            int n = list.Count;
            while (n > 1)
            {
                int k = rnd.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }

            return list;
        }
    }

    public sealed class Int32Extension : MarkupExtension
    {
        public Int32Extension(int value) => Value = value;
        public int Value { get; set; }
        public override Object ProvideValue(IServiceProvider sp) => Value;
    };
}
