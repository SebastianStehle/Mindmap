// ==========================================================================
// IMindmapPrintService.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Threading.Tasks;
using Hercules.Model;
using Hercules.Model.Rendering;

namespace Hercules.App.Components
{
    public interface IMindmapPrintService
    {
        Task PrintAsync(Document document, IRenderer renderer);
    }
}
