// ==========================================================================
// DocumentViewModelBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GalaSoft.MvvmLight;
using GP.Windows;
using Hercules.App.Components;
using Hercules.Model;
using Hercules.Win2D.Rendering;
using PropertyChanged;

namespace Hercules.App.Modules.Editor.ViewModels
{
    [ImplementPropertyChanged]
    public abstract class DocumentViewModelBase : ViewModelBase
    {
        private readonly IWin2DRendererProvider rendererProvider;
        private readonly IMindmapStore mindmapStore;
        private Document document;

        public IWin2DRendererProvider RendererProvider
        {
            get { return rendererProvider; }
        }

        public IMindmapStore MindmapStore
        {
            get { return mindmapStore; }
        }

        [NotifyUI]
        public Win2DRenderer Renderer { get; set; }

        [NotifyUI]
        public NodeBase SelectedNode { get; set; }

        public Document Document
        {
            get
            {
                return document;
            }
            set
            {
                if (document != value)
                {
                    if (document != null)
                    {
                        document.NodeSelected -= Document_NodeSelected;

                        OnDocumentUnset(document);
                    }

                    document = value;

                    RaisePropertyChanged();

                    if (document != null)
                    {
                        document.NodeSelected += Document_NodeSelected;

                        OnDocumentSet(document);

                        SelectedNode = document.SelectedNode;
                    }
                }
            }
        }

        private void Document_NodeSelected(object sender, NodeEventArgs e)
        {
            SelectedNode = e.Node;
        }

        protected DocumentViewModelBase()
        {
        }

        protected DocumentViewModelBase(IMindmapStore mindmapStore, IWin2DRendererProvider rendererProvider)
        {
            this.rendererProvider = rendererProvider;

            if (rendererProvider != null)
            {
                rendererProvider.RendererCreated += RendererProvider_RendererCreated;
            }

            this.mindmapStore = mindmapStore;

            if (mindmapStore != null)
            {
                mindmapStore.DocumentLoaded += MindmapStore_DocumentLoaded;
            }
        }

        private void RendererProvider_RendererCreated(object sender, EventArgs e)
        {
            Renderer = rendererProvider.Current;
        }

        private void MindmapStore_DocumentLoaded(object sender, DocumentLoadedEventArgs e)
        {
            Document = e.Document;
        }

        protected virtual void OnDocumentSet(Document newDocument)
        {
        }

        protected virtual void OnDocumentUnset(Document oldDocument)
        {
        }

        protected virtual void OnSelectedNodeChanged()
        {
        }
    }
}
