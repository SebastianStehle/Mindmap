// ==========================================================================
// IPrintService.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Threading.Tasks;
using Hercules.Model;
using Hercules.Model.Layouting;

namespace Hercules.App.Components
{
    public interface IPrintService
    {
        Task PrintAsync(Document document, IRenderer renderer);
    }
}
