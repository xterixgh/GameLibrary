using Avalonia.Controls;
using Avalonia.Platform.Storage;
using GameLibrary.Helpers;
using GameLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Pages
{
    public partial class GameEdit : UserControl
    {
        private Games? _game;
        private string? _coverPath;
        private string? _newCoverSource;
        private bool _removeCover;

        public GameEdit() : this(null)
        {
        }

        public GameEdit(Games? game = null)
        {
            InitializeComponent();
            _game = game;
            Loaded += GameEdit_Loaded;
        }

        private async void GameEdit_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            StatusBox.ItemsSource = await App.DbContext.PlayStatuses.ToListAsync();

            if (_game == null)
            {
                TitleText.Text = "Новая игра";
                YearBox.Text = "2026";
                RatingBox.Text = "5";
                MinutesBox.Text = "0";
                ShowCover(null);
            }
            else
            {
                TitleText.Text = "Изменить игру";
                NameBox.Text = _game.Title;
                YearBox.Text = _game.ReleaseYear.ToString();
                RatingBox.Text = _game.Rating.ToString();
                MinutesBox.Text = _game.PlaytimeMinutes.ToString();
                WishlistBox.IsChecked = _game.IsWishlist;
                DescBox.Text = _game.Description;
                LaunchPathBox.Text = _game.LaunchPath;
                _coverPath = _game.CoverPath;
                ShowCover(_coverPath);

                foreach (PlayStatuses s in StatusBox.ItemsSource!)
                {
                    if (s.Id == _game.PlayStatuses_Id)
                    {
                        StatusBox.SelectedItem = s;
                        break;
                    }
                }
            }
        }

        private void ShowCover(string? path)
        {
            var bmp = CoverHelper.Load(path);
            if (bmp == null)
            {
                CoverPreview.Source = null;
                CoverPreview.IsVisible = false;
                CoverLetter.IsVisible = true;
                string title = NameBox.Text;
                if (!string.IsNullOrWhiteSpace(title))
                    CoverLetter.Text = title.Substring(0, 1).ToUpper();
                else
                    CoverLetter.Text = "?";
            }
            else
            {
                CoverPreview.Source = bmp;
                CoverPreview.IsVisible = true;
                CoverLetter.IsVisible = false;
            }
        }

        private async void PickCover_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var top = TopLevel.GetTopLevel(this);
            if (top == null)
                return;

            var files = await top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Выбери обложку",
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
            });

            if (files == null || files.Count == 0)
                return;

            string? localPath = files[0].TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(localPath))
            {
                ErrorText.Text = "Не удалось открыть файл";
                ErrorText.IsVisible = true;
                return;
            }

            _newCoverSource = localPath;
            _removeCover = false;
            ShowCover(localPath);
        }

        private void ClearCover_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _newCoverSource = null;
            _removeCover = true;
            _coverPath = null;
            ShowCover(null);
        }

        private async void PickExe_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var top = TopLevel.GetTopLevel(this);
            if (top == null)
                return;

            var files = await top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Выбери файл игры",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Исполняемые файлы")
                    {
                        Patterns = new[] { "*.exe" }
                    }
                }
            });

            if (files == null || files.Count == 0)
                return;

            string? path = files[0].TryGetLocalPath();
            if (!string.IsNullOrWhiteSpace(path))
                LaunchPathBox.Text = path;
        }

        private async void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ErrorText.IsVisible = false;

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                ErrorText.Text = "Введи название";
                ErrorText.IsVisible = true;
                return;
            }

            if (StatusBox.SelectedItem is not PlayStatuses status)
            {
                ErrorText.Text = "Выбери статус";
                ErrorText.IsVisible = true;
                return;
            }

            if (!int.TryParse(YearBox.Text, out int year) ||
                !int.TryParse(RatingBox.Text, out int rating) ||
                !int.TryParse(MinutesBox.Text, out int minutes))
            {
                ErrorText.Text = "Год, оценка и минуты — числа";
                ErrorText.IsVisible = true;
                return;
            }

            if (_game == null)
            {
                _game = new Games();
                _game.User_Id = App.CurrentUser!.Id;
                _game.CreatedAt = DateTime.UtcNow;
                App.DbContext.Games.Add(_game);
            }

            _game.Title = NameBox.Text.Trim();
            _game.ReleaseYear = year;
            _game.Rating = rating;
            _game.PlaytimeMinutes = minutes;
            _game.IsWishlist = WishlistBox.IsChecked == true;
            _game.Description = DescBox.Text ?? "";
            _game.LaunchPath = string.IsNullOrWhiteSpace(LaunchPathBox.Text)
                ? null
                : LaunchPathBox.Text.Trim();
            _game.PlayStatuses_Id = status.Id;
            _game.UpdateAt = DateTime.UtcNow;

            if (_removeCover)
            {
                _game.CoverPath = null;
            }
            else if (!string.IsNullOrWhiteSpace(_newCoverSource))
            {
                _game.CoverPath = CoverHelper.SaveCopy(_newCoverSource, App.CurrentUser!.Id);
            }
            else
            {
                _game.CoverPath = _coverPath;
            }

            await App.DbContext.SaveChangesAsync();
            GoBack();
        }

        private void BackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            GoBack();
        }

        private void GoBack()
        {
            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new Library();
        }
    }
}
