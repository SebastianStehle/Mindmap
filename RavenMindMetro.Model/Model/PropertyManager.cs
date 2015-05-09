// ==========================================================================
// PropertyManager.cs
// SE Requirements Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace RavenMind.Model
{
    internal sealed class PropertyManager
    {
        #region Fields

        private readonly Dictionary<object, Dictionary<string, ValuePair>> propertyValues = new Dictionary<object, Dictionary<string, ValuePair>>();

        #endregion

        #region Methods

        public IEnumerable<Tuple<object, UndoRedoPropertyChangedEventArgs>> CalculateChangedProperties()
        {
            List<Tuple<object, UndoRedoPropertyChangedEventArgs>> properties = new List<Tuple<object, UndoRedoPropertyChangedEventArgs>>();

            foreach (var objectProperties in propertyValues)
            {
                object changedObject = objectProperties.Key;

                foreach (var objectProperty in objectProperties.Value)
                {
                    UndoRedoPropertyChangedEventArgs e = new UndoRedoPropertyChangedEventArgs(objectProperty.Key, objectProperty.Value.NewValue, objectProperty.Value.OldValue);

                    properties.Add(new Tuple<object, UndoRedoPropertyChangedEventArgs>(changedObject, e));
                }
            }

            return properties;
        }

        public void Clear()
        {
            propertyValues.Clear();
        }

        public void Register(object changedObject, UndoRedoPropertyChangedEventArgs propertyInfo)
        {
            if (changedObject == null)
            {
                throw new ArgumentNullException("changedObject");
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            Dictionary<string, ValuePair> objectProperties = null;

            if (!propertyValues.TryGetValue(changedObject, out objectProperties))
            {
                objectProperties = new Dictionary<string, ValuePair>();

                propertyValues.Add(changedObject, objectProperties);
            }

            ValuePair propertyPair = null;

            if (!objectProperties.TryGetValue(propertyInfo.PropertyName, out propertyPair))
            {
                propertyPair = new ValuePair();
                propertyPair.OldValue = propertyInfo.OldValue;

                objectProperties.Add(propertyInfo.PropertyName, propertyPair);
            }

            propertyPair.NewValue = propertyInfo.NewValue;
        }

        #endregion
    }
}
