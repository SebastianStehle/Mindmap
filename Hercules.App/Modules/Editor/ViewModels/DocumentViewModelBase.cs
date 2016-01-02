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
        public NodeBase SelectedNode { get; private set; }

        [NotifyUI]
        public Document Document { get; set; }

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

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == "Document")
            {
                OnDocumentChanged(before as Document, after as Document);

                OnDocumentChangedInternal(before as Document, after as Document);
            }
            else if (propertyName == "SelectedNode")
            {
                OnSelectedNodeChanged(before as NodeBase, after as NodeBase);
            }

            RaisePropertyChanged(propertyName);
        }

        private void OnDocumentChangedInternal(Document oldDocument, Document newDocument)
        {
            if (oldDocument != null)
            {
                oldDocument.NodeSelected -= Document_NodeSelected;
            }
            if (newDocument != null)
            {
                newDocument.NodeSelected += Document_NodeSelected;

                SelectedNode = newDocument.SelectedNode;
            }
        }

        protected virtual void OnDocumentChanged(Document oldDocument, Document newDocument)
        {
        }

        protected virtual void OnSelectedNodeChanged(NodeBase oldNode, NodeBase newNode)
        {
        }

        private void Document_NodeSelected(object sender, NodeEventArgs e)
        {
            SelectedNode = e.Node;
        }
    }
}
