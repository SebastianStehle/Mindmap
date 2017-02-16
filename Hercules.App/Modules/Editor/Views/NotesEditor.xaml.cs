// ==========================================================================
// NotesEditor.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Hercules.Model;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class NotesEditor
    {
        private readonly Flyout flyout;
        private readonly NodeBase node;

        public NotesEditor()
        {
            InitializeComponent();
        }

        public NotesEditor(Flyout flyout, NodeBase node)
        {
            this.node = node;

            this.flyout = flyout;
            this.flyout.Closed += Flyout_Closed;

            InitializeComponent();

            EditBox.Document.SetText(TextSetOptions.FormatRtf, node.Notes ?? string.Empty);
        }

        private void EditBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selection = EditBox.Document.Selection;

            if (selection == null)
            {
                return;
            }

            UpdateBoldButton(selection);
            UpdateItalicButton(selection);
            UpdateUnderlineButton(selection);
            UpdateListButton(selection);
        }

        private void Flyout_Closed(object sender, object e)
        {
            var text = string.Empty;

            var selection = EditBox.Document.Selection;

            selection.Expand(TextRangeUnit.Window);

            if (!string.IsNullOrWhiteSpace(selection.Text.Trim('\r', '\n')))
            {
                EditBox.Document.GetText(TextGetOptions.FormatRtf, out text);
            }

            node.ChangeNotesTransactional(text);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            flyout.Hide();
        }

        private void UpdateListButton(ITextRange selection)
        {
            ListButton.IsChecked = selection.ParagraphFormat.ListType == MarkerType.Bullet;
        }

        private void UpdateUnderlineButton(ITextRange selection)
        {
            UnderlineButton.IsChecked = selection.CharacterFormat.Underline == UnderlineType.Single;
        }

        private void UpdateBoldButton(ITextRange selection)
        {
            BoldButton.IsChecked = selection.CharacterFormat.Bold == FormatEffect.On;
        }

        private void UpdateItalicButton(ITextRange selection)
        {
            ItalicButton.IsChecked = selection.CharacterFormat.Italic == FormatEffect.On;
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            EditBox.Document.Selection.CharacterFormat.Bold = BoldButton.IsChecked == true ? FormatEffect.On : FormatEffect.Off;
            EditBox.Focus(FocusState.Programmatic);
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            EditBox.Document.Selection.CharacterFormat.Italic = ItalicButton.IsChecked == true ? FormatEffect.On : FormatEffect.Off;
            EditBox.Focus(FocusState.Programmatic);
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            EditBox.Document.Selection.CharacterFormat.Underline = UnderlineButton.IsChecked == true ? UnderlineType.Single : UnderlineType.None;
            EditBox.Focus(FocusState.Programmatic);
        }

        private void ListButton_Click(object sender, RoutedEventArgs e)
        {
            EditBox.Document.Selection.ParagraphFormat.ListType = ListButton.IsChecked == true ? MarkerType.Bullet : MarkerType.None;
            EditBox.Focus(FocusState.Programmatic);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EditBox.Focus(FocusState.Programmatic);
        }
    }
}
