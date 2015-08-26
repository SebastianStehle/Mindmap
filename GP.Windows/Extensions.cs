// ==========================================================================
// Extensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using Windows.Foundation;

namespace GP.Windows
{
    /// <summary>
    /// A collection of simple helper extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Registers the specified collection changed event handler to the event source.
        /// </summary>
        /// <param name="eventSource">The event source. Cannot be null.</param>
        /// <param name="eventHandler">The event handler to register. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="eventSource"/> is null.
        ///     - or -
        ///     <paramref name="eventHandler"/> is null.
        /// </exception>
        public static void Register(this INotifyCollectionChanged eventSource, NotifyCollectionChangedEventHandler eventHandler)
        {
            Guard.NotNull(eventSource, "eventSource");
            Guard.NotNull(eventHandler, "eventHandler");

            eventSource.CollectionChanged += eventHandler;
        }

        /// <summary>
        /// Determines if two points are about equal.
        /// </summary>
        /// <param name="l">The first point.</param>
        /// <param name="r">The second point.</param>
        /// <returns>
        /// True, if two point values are more or less equal.
        /// </returns>
        public static bool AboutEqual(Point l, Point r)
        {
            return AboutEqual(l.X, r.X) && AboutEqual(l.Y, r.Y);
        }

        /// <summary>
        /// Determines if two double values are about equal.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// True, if two double values are more or less equal.
        /// </returns>
        public static bool AboutEqual(double x, double y)
        {
            double epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;

            return Math.Abs(x - y) <= epsilon;
        }

        /// <summary>
        /// Unregisters the specified collection changed event handler from the event source.
        /// </summary>
        /// <param name="eventSource">The event source. Cannot be null.</param>
        /// <param name="eventHandler">The event handler to unregister. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="eventSource"/> is null.
        ///     - or -
        ///     <paramref name="eventHandler"/> is null.
        /// </exception>
        public static void Unregister(this INotifyCollectionChanged eventSource, NotifyCollectionChangedEventHandler eventHandler)
        {
            Guard.NotNull(eventSource, "eventSource");
            Guard.NotNull(eventHandler, "eventHandler");

            eventSource.CollectionChanged -= eventHandler;
        }

        /// <summary>
        /// Writes the source stream to the target stream.
        /// </summary>
        /// <param name="target">The target stream where to write the source stream to. Cannot be null.</param>
        /// <param name="source">The source stream to write to the target stream. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="target"/> is null.
        ///     - or -
        ///     <paramref name="source"/> is null.
        /// </exception>
        public static void Write(this Stream target, Stream source)
        {
            Guard.NotNull(source, "source");
            Guard.NotNull(target, "target");

            byte[] buffer = new byte[32768];

            int bytesRead;

            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                target.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Returns the index of the element in the target list.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="target">The target list.</param>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The resulting index.
        /// </returns>
        public static int IndexOf<T>(this IReadOnlyList<T> target, T element)
        {
            for (int i = 0; i < target.Count; i++)
            {
                if (Equals(target[i], element))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Invokes the specified foreach item in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
        /// <param name="items">The enumerable that is iterated through this method. Cannot be null.</param>
        /// <param name="action">The action to invoke foreach item. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="items"/> is null.
        ///     - or -
        ///     <paramref name="action"/> is null.
        /// </exception>
        public static void Foreach<T>(this IEnumerable<T> items, Action<T> action)
        {
            Guard.NotNull(items, "items");
            Guard.NotNull(action, "action");

            foreach (T item in items)
            {
                if (item != null)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// Adds the the specified elements to the target collection object.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the source and target.</typeparam>
        /// <param name="target">The target, where the items should be inserted to. Cannot be null.</param>
        /// <param name="elements">The elements to add to the collection. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="target"/> is null.
        ///     - or -
        ///     <paramref name="elements"/> is null.
        /// </exception>
        public static void AddRange<TItem>(this Collection<TItem> target, IEnumerable<TItem> elements)
        {
            Guard.NotNull(target, "target");
            Guard.NotNull(elements, "elements");

            foreach (TItem item in elements)
            {
                target.Add(item);
            }
        }

        /// <summary>
        /// Gets a value from the dictionary with the specified key or creates a new instance 
        /// and inserts the instance to the dictionary, if a value with such a key does not exists.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary where the value should be get from.</param>
        /// <param name="key">The key of the value.</param>
        /// <returns>
        /// The value from the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
        public static TValue GetOrCreateDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            Guard.NotNull(dictionary, "dictionary");

            return GetOrCreateDefault(dictionary, key, () => default(TValue));
        }

        /// <summary>
        /// Gets a value from the dictionary with the specified key or creates a new instance by invoking the function
        /// and inserts the instance to the dictionary, if a value with such a key does not exists.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary where the value should be get from.</param>
        /// <param name="key">The key of the value.</param>
        /// <param name="function">The function for creating the instance. Cannot be null.</param>
        /// <returns>
        /// The value from the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dictionary"/> is null.
        ///     - or -
        ///     <paramref name="function"/> is null.
        /// </exception>
        public static TValue GetOrCreateDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> function)
        {
            Guard.NotNull(dictionary, "dictionary");
            Guard.NotNull(function, "function");

            TValue value;

            if (!dictionary.TryGetValue(key, out value))
            {
                dictionary[key] = value = function();
            }

            return value;
        }
    }
}
