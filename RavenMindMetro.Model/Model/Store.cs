// ==========================================================================
// RootNode.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    /// <summary>
    /// Works as a wrapper for a object that must be initialized before using it.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped value.</typeparam>
    public struct Store<T> : IEquatable<Store<T>>
    {
        #region Properties

        /// <summary>
        /// Gets the value object that is handled by this object.
        /// </summary>
        /// <value>The value.</value>
        public T Value { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object has a valid value.
        /// </summary>
        /// <value><c>true</c> if this instance has a valid value; otherwise, <c>false</c>.</value>
        public bool HasValue { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Store&lt;T&gt;"/> struct with 
        /// the value that is handled by this object.
        /// </summary>
        /// <param name="value">The value.</param>
        public Store(T value)
            : this()
        {
            Value = value;

            HasValue = true;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Performs an explicit conversion from <see cref="RavenMind.Model.Store&lt;T&gt;"/> to the type of the value.
        /// </summary>
        /// <param name="value">The value which is the source of the convesion.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator T(Store<T> value)
        {
            return value.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from the value object to <see cref="RavenMind.Model.Store&lt;T&gt;"/>.
        /// </summary>
        /// <param name="value">The value which is the source of the convesion.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Store<T>(T value)
        {
            return new Store<T>(value);
        }

        #endregion

        #region IEquality<Initializable<T>> Members

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="lhs">The left operand.</param>
        /// <param name="rhs">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Store<T> lhs, Store<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="lhs">The left operand.</param>
        /// <param name="rhs">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Store<T> lhs, Store<T> rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool areEquals = false;

            if (obj is Store<T>)
            {
                areEquals = Equals((Store<T>)obj);
            }

            return areEquals;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object 
        /// of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(Store<T> other)
        {
            return object.Equals(Value, other.Value);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
