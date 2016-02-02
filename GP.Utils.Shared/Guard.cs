// ==========================================================================
// Guard.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
// ReSharper disable InvertIf

namespace GP.Utils
{
    /// <summary>
    /// A static class with a lot of helper methods, which guards 
    /// a method agains invalid parameters.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Verifies that the specified value is between a lower and a upper value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is not between
        /// the lower and the upper value.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Between<TValue>(TValue target, TValue lower, TValue upper, string parameterName) where TValue : IComparable
        {
            if (!target.IsBetween(lower, upper))
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Value must be between {0} and {1}", lower, upper);

                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is greater than a lower value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GreaterThan<TValue>(TValue target, TValue lower, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(lower) <= 0)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Value must be greater than {0}", lower);

                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is greater or equals than a lower value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GreaterEquals<TValue>(TValue target, TValue lower, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(lower) < 0)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Value must be greater than {0}", lower);

                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is less than a upper value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessThan<TValue>(TValue target, TValue upper, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(upper) >= 0)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Value must be less than {0}", upper);

                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is less or equals than a upper value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessEquals<TValue>(TValue target, TValue upper, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(upper) > 0)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Value must be less than {0}", upper);

                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the collection method parameter with specified reference
        /// contains one or more elements and throws an exception
        /// if the object contains no elements.
        /// </summary>
        /// <typeparam name="TType">The type of the items in the collection.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentException"><paramref name="enumerable"/> is
        /// empty or contains only blanks.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEmpty<TType>(ICollection<TType> enumerable, string parameterName)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (enumerable.Count == 0)
            {
                throw new ArgumentException("Collection does not contain an item", parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the method parameter with specified object value and message  
        /// is not empty and throws an exception if the object is empty.
        /// </summary>
        /// <param name="target">The target object, which cannot be empty.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// empty.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEmpty(Guid target, string parameterName)
        {
            if (target == Guid.Empty)
            {
                throw new ArgumentException("Value cannot be empty.", parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the method parameter with specified object value and message  
        /// is not null and throws an exception if the object is null.
        /// </summary>
        /// <param name="target">The target object, which cannot be null.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the string method parameter with specified object value and message
        /// is not null, not empty and does not contain only blanks and throws an exception 
        /// if the object is null.
        /// </summary>
        /// <param name="target">The target string, which should be checked against being null or empty.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is
        /// empty or contains only blanks.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrEmpty(string target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                throw new ArgumentException("String parameter cannot be null or empty and cannot contain only blanks.", parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the string method parameter with specified object value and message
        /// is not null, not empty and does not contain only blanks and is a valid file name and throws an exception 
        /// if the object is null.
        /// </summary>
        /// <param name="target">The target string, which should be checked against being null or empty.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is
        /// empty or contains only blanks.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidFileName(string target, string parameterName)
        {
            NotNullOrEmpty(target, parameterName);

            if (target.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                throw new ArgumentException("Name contains an invalid character.", parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the model is a valid model.
        /// </summary>
        /// <param name="target">The target object, which must be a valid model.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        /// <exception cref="ValidationException"><paramref name="target"/> is
        /// not a valid model.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidModel(object target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            Validator.ValidateObject(target, new ValidationContext(target), true);
        }

        /// <summary>
        /// Verfifies that the specified target has the right type.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <param name="target">The target object, which cannot be null.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is not of the specified type.</exception>
        /// <exception cref="System.ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsType<T>(object target, string parameterName)
        {
            if (target != null && target.GetType() != typeof(T))
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Value must be of type {0}", typeof(T));

                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verfifies that the specified target has the right type.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="target">The target object, which cannot be null.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is not of the specified type.</exception>
        /// <exception cref="System.ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsType(object target, Type expectedType, string parameterName)
        {
            if (target != null && expectedType != null && target.GetType() != expectedType)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Value must be of type {0}", expectedType);

                throw new ArgumentException(message, parameterName);
            }
        }
    }
}
