using Avalonia.Controls;
using Avalonia.Interactivity;
using GameLibrary;
using GameLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GameLibrary.Pages;

public partial class Registration : UserControl
{
    public Registration()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is MainWindow window)
            window.MainControl.Content = new Login();
    }

    private async void RegisterButton_Click(object? sender, RoutedEventArgs e)
    {
        ErrorText.IsVisible = false;
        string login = LoginBox.Text;
        string password = PasswordBox.Text;
        string confirm = ConfirmBox.Text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm))
        {
            ErrorText.IsVisible =true;
            ErrorText.Text = "Заполните корректно данные";
            return;
        }

        if (login.Length < 3)
        {
            ErrorText.Text = "Логин должен быть не короче 3 символов.";
            ErrorText.IsVisible = true;
            return;
        }
        if (password.Length < 6)
        {
            ErrorText.Text = "Пароль должен быть не короче 6 символов.";
            ErrorText.IsVisible = true;
            return;
        }
        if (password != confirm)
        {
            ErrorText.Text = "Пароли не совпадают.";
            ErrorText.IsVisible = true;
            return;
        }

        bool taken = await App.DbContext.Users
       .AnyAsync(u => u.Login.ToLower() == login.ToLower());
        if (taken)
        {
            ErrorText.Text = "Этот логин уже занят";
            ErrorText.IsVisible = true;
            return;
        }

        var newUser = new User
        {
            Login = login,
            PasswordHash = password,
            CreateAt = DateTime.UtcNow
        };



        await App.DbContext.Users.AddAsync(newUser);
        await App.DbContext.SaveChangesAsync();

        if (TopLevel.GetTopLevel(this) is MainWindow window)
            window.MainControl.Content = new Login();

    }
}
