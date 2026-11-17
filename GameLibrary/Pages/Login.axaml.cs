using Avalonia.Controls;
using GameLibrary;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.Pages
{
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }

      
        private async void LoginButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Text;
            ErrorText.IsVisible = true;
            if (string.IsNullOrEmpty(LoginBox.Text) || string.IsNullOrEmpty(PasswordBox.Text)) 
            {
                ErrorText.Text = "Неверный логин или пароль";
                ErrorText.IsVisible = true;
                return;
            }
           

            var loginUser = await App.DbContext.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (loginUser == null || loginUser.PasswordHash != password) 
            {
                ErrorText.Text = "Неверный логин или пароль";
                ErrorText.IsVisible = true;
                return;
            }
            App.CurrentUser = loginUser;
            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new Library();
        }

        private void RegisterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new Registration();
        }
    }
}