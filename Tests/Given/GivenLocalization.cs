// ==========================================================================
// GivenLocalization.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Tests.Given
{
    public abstract class GivenLocalization
    {
        protected GivenLocalization()
        {
            LocalizationManager.Provider = new NoopLocalizationProvider();
        }
    }
}
