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
        public static void Swap<T>(this IList<T> array, int pos1, int pos2)
        {
            T temp = array[pos1];
            array[pos1] = array[pos2];
            array[pos2] = temp;
        }

        public static List<T> Shuffle<T>(this IEnumerable<T> array, int startWith = 0)
        {
            List<T> list = new(array);

            Random rnd = new();
            int n = list.Count;
            while (n > startWith + 1)
            {
                int k = rnd.Next(startWith, n--);
                list.Swap(n, k);
            }

            return list;
        }

        /// <summary>
        /// Перемещает выбранный элемент на первую позицию и не затрагивает его при перемешке.
        /// </summary>
        public static List<T> ShuffleWithoutFirstElement<T>(this IEnumerable<T> array, int firstElement = 0)
        {
            List<T> list = new(array);

            if (firstElement != 0)
                list.Swap(0, firstElement);

            Random rnd = new();
            int n = list.Count;
            while (n > 2)
            {
                int k = rnd.Next(1, n--);
                list.Swap(n, k);
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
    
    public sealed class BoolExtension : MarkupExtension
    {
        public BoolExtension(bool value) => Value = value;
        public bool Value { get; set; }
        public override Object ProvideValue(IServiceProvider sp) => Value;
    };
}
