// ==========================================================================
// Node.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    /// <summary>
    /// Defines a normal node, that is not a root node.
    /// </summary>
    public class Node : NodeBase, IOrdered
    {
        #region Fields

        private readonly NodeCollection children;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the ordering has been changed.
        /// </summary>
        public event EventHandler<OrderedEventArgs> OrderChanged;
        /// <summary>
        /// Raises the <see cref="E:OrderChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnOrderChanged(OrderedEventArgs e)
        {
            EventHandler<OrderedEventArgs> eventHandler = OrderChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the children of the node.
        /// </summary>
        /// <value>
        /// The children of the current node.
        /// </value>
        public NodeCollection Children
        {
            get { return children; }
        }

        private NodeSide side;
        /// <summary>
        /// Gets a value indicating at which side this node is positioned.
        /// </summary>
        /// <value>
        /// A value indicating at which side this node is positioned.
        /// </value>
        [XmlIgnore]
        public NodeSide Side
        {
            get
            {
                return side;
            }
            set
            {
                if (side != value)
                {
                    side = value;
                    OnPropertyChanged("NodeSide");
                }
            }
        }

        private int orderIndex;
        /// <summary>
        /// Gets or sets the order index.
        /// </summary>
        /// <value>
        /// The order index.
        /// </value>
        [XmlAttribute]
        public int OrderIndex
        {
            get
            {
                return orderIndex;
            }
            set
            {
                if (orderIndex != value)
                {
                    var oldValue = orderIndex;

                    orderIndex = value;
                    OnPropertyChanged("OrderIndex");
                    OnOrderChanged(new OrderedEventArgs(orderIndex, oldValue));
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
        {
            children = new NodeCollection(this, () => Side);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Notifies the ordered object that it has been reordered by the user.
        /// </summary>
        public void NotifyReordered(int oldIndex)
        {
            OnUndoRedoPropertyChanged(new UndoRedoPropertyChangedEventArgs("OrderIndex", orderIndex, oldIndex));
        }

        #endregion
    }
}
