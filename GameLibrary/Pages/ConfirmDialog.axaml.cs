using Avalonia.Controls;

namespace GameLibrary.Pages
{
    public partial class ConfirmDialog : Window
    {
        public bool Confirmed { get; private set; }

        public ConfirmDialog()
        {
            InitializeComponent();
        }

        public ConfirmDialog(string message) : this()
        {
            MessageText.Text = message;
        }

        private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }

        private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }
    }
}
