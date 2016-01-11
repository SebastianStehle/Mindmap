// ==========================================================================
// MockupModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.ComponentModel.DataAnnotations;

namespace Tests.Mockups
{
    public class MockupModel
    {
        [Required]
        public string Name { get; set; }
    }
}
