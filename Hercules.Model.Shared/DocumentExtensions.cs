// ==========================================================================
// DocumentExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Hercules.Model
{
    public static class DocumentExtensions
    {
        public static void ToggleCheckableTransactional(this Document document)
        {
            var tansactionName = LocalizationManager.GetString("TransactionName_ToggleCheckable");

            document.MakeTransaction(tansactionName, commands =>
            {
                commands.Apply(new ToggleCheckableCommand(document));
            });
        }
    }
}
