// ==========================================================================
// Node.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    /// <summary>
    /// Abstract base class for all nodes of the diagram.
    /// </summary>
    public abstract class NodeBase : ISelectable, INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// Occurs when the current item has been selected or unselected by the user.
        /// </summary>
        public event EventHandler SelectionChanged;
        /// <summary>
        /// Raises the <see cref="E:SelectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            EventHandler eventHandler = SelectionChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        /// <summary>
        /// Occurs when an property that is managed by the undo redo system has been changed.
        /// </summary>
        public event EventHandler<UndoRedoPropertyChangedEventArgs> UndoRedoPropertyChanged;
        /// <summary>
        /// Raises the <see cref="E:UndoRedoPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="UndoRedoPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUndoRedoPropertyChanged(UndoRedoPropertyChangedEventArgs e)
        {
            EventHandler<UndoRedoPropertyChangedEventArgs> eventHandler = UndoRedoPropertyChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler eventHandler = PropertyChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the unique id of the node.
        /// </summary>
        /// <value>
        /// The id of the node.
        /// </value>
        [XmlAttribute]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the tag value to attach temporary data.
        /// </summary>
        /// <value>
        /// A property to attach temporary data.
        /// </value>
        [XmlIgnore]
        public object Tag { get; set; }

        /// <summary>
        /// Gets the document where this node belongs to.
        /// </summary>
        /// <value>
        /// The document where this node belongs to.
        /// </value>
        [XmlIgnore]
        public Document Document { get; internal set; }

        /// <summary>
        /// Gets the parent of the current node. Can be null.
        /// </summary>
        /// <value>
        /// The parent of the current node.
        /// </value>
        [XmlIgnore]
        public NodeBase Parent { get; internal set; }

        private int color;
        /// <summary>
        /// Gets or sets the color of the node as simple integer, where all channels have four bytes.
        /// </summary>
        /// <value>
        /// The color of the node.
        /// </value>
        [DefaultValue(0)]
        [XmlAttribute]
        public int Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color != value)
                {
                    var oldValue = color;

                    color = value;
                    OnPropertyChanged("Color");
                    OnUndoRedoPropertyChanged(new UndoRedoPropertyChangedEventArgs("Color", color, oldValue));
                }
            }
        }

        private string text;
        /// <summary>
        /// Gets or sets the text of the node.
        /// </summary>
        /// <value>
        /// The text of the node.
        /// </value>
        [DefaultValue(null)]
        [XmlElement]
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text != value)
                {
                    var oldValue = text;

                    text = value;
                    OnPropertyChanged("Text");
                    OnUndoRedoPropertyChanged(new UndoRedoPropertyChangedEventArgs("Text", text, oldValue));
                }
            }
        }

        private IconSize iconSize;
        /// <summary>
        /// Gets or sets the size of the icon, that must be specified using the <see cref="P:IconKey"/> property.
        /// </summary>
        /// <value>
        /// The size of the icon of this node.
        /// </value>
        [DefaultValue(IconSize.Small)]
        [XmlAttribute]
        public IconSize IconSize
        {
            get
            {
                return iconSize;
            }
            set
            {
                if (iconSize != value)
                {
                    var oldValue = iconSize;

                    iconSize = value;
                    OnPropertyChanged("IconSize");
                    OnUndoRedoPropertyChanged(new UndoRedoPropertyChangedEventArgs("IconSize", iconSize, oldValue));
                }
            }
        }

        private string iconKey;
        /// <summary>
        /// Gets or sets the key to identify the icon image. Can be null if no icon should be used.
        /// </summary>
        /// <value>
        /// The key to identify the icon image.
        /// </value>
        [DefaultValue(null)]
        [XmlAttribute]
        public string IconKey
        {
            get
            {
                return iconKey;
            }
            set
            {
                if (iconKey != value)
                {
                    var oldValue = iconKey;

                    iconKey = value;
                    OnPropertyChanged("IconKey");
                    OnUndoRedoPropertyChanged(new UndoRedoPropertyChangedEventArgs("IconKey", iconKey, oldValue));
                }
            }
        }

        private bool isSelected;
        /// <summary>
        /// Gets or sets a value indicating whether the item is selected by the user
        /// or by the application code.
        /// </summary>
        /// <value>
        /// <c>true</c> if this item is selected by the user; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnSelectionChanged(EventArgs.Empty);
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeBase"/> class.
        /// </summary>
        protected NodeBase()
        {
            Id = Guid.NewGuid();
        }

        #endregion
    }
}
