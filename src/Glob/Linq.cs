#if NET20
namespace System.Runtime.CompilerServices
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>Indicates that a method is an extension method, or that a class or assembly contains extension methods.</summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.ExtensionAttribute" /> class. </summary>
        public ExtensionAttribute()
        {
        }
    }
}

namespace System
{
    using System;

    internal delegate TResult Func<in T, out TResult>(T item1);
}

namespace System.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal static class Enumerable
    {
        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (TSource element in source)
            {
                if (predicate(element))
                    return true;
            }
            return false;
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            ICollection<TSource> collection = source as ICollection<TSource>;
            if (collection != null)
                return collection.Contains(value);

            var comparer = EqualityComparer<TSource>.Default;
            foreach (TSource element in source)
                if (comparer.Equals(element, value))
                    return true;
            return false;
        }

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            return new Buffer<TSource>(source).ToArray();
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                int count = list.Count;
                if (count > 0)
                    return list[count - 1];
            }
            else
            {
                using (IEnumerator<TSource> e = source.GetEnumerator())
                {
                    if (e.MoveNext())
                    {
                        TSource result;
                        do
                        {
                            result = e.Current;
                        } while (e.MoveNext());
                        return result;
                    }
                }
            }
            return default(TSource);
        }

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            foreach (TSource element in first)
                yield return element;

            foreach (TSource element in second)
                yield return element;
        }

        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
        {
            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                while (count > 0 && e.MoveNext())
                    count--;

                if (count <= 0)
                {
                    while (e.MoveNext())
                        yield return e.Current;
                }
            }
        }

        struct Buffer<TElement>
        {
            internal TElement[] items;
            internal int count;

            internal Buffer(IEnumerable<TElement> source)
            {
                TElement[] items = null;
                int count = 0;
                ICollection<TElement> collection = source as ICollection<TElement>;
                if (collection != null)
                {
                    count = collection.Count;
                    if (count > 0)
                    {
                        items = new TElement[count];
                        collection.CopyTo(items, 0);
                    }
                }
                else
                {
                    foreach (TElement item in source)
                    {
                        if (items == null)
                        {
                            items = new TElement[4];
                        }
                        else if (items.Length == count)
                        {
                            TElement[] newItems = new TElement[checked(count * 2)];
                            Array.Copy(items, 0, newItems, 0, count);
                            items = newItems;
                        }
                        items[count] = item;
                        count++;
                    }
                }
                this.items = items;
                this.count = count;
            }

            internal TElement[] ToArray()
            {
                if (count == 0)
                    return new TElement[0];
                if (items.Length == count)
                    return items;
                TElement[] result = new TElement[count];
                Array.Copy(items, 0, result, 0, count);
                return result;
            }
        }
    }
}
#endif
