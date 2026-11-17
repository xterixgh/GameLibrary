using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using GameLibrary.Helpers;
using GameLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Pages
{
    public partial class Library : UserControl
    {
        private GameCardItem? _selected;

        public Library()
        {
            InitializeComponent();
            Loaded += Library_Loaded;
        }

        private async void Library_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ThemeButton.Content = ThemeHelper.ButtonText();
            StatusFilterBox.ItemsSource = await App.DbContext.PlayStatuses.ToListAsync();
            await LoadGameAsync();
        }

        private void ThemeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ThemeHelper.Toggle();
            ThemeButton.Content = ThemeHelper.ButtonText();
        }

        private async Task LoadGameAsync()
        {
            UserInfoText.Text = "Вы вошли как: " + App.CurrentUser.Login;
            StatusText.Text = "Загрузка игр...";
            _selected = null;

            var query = App.DbContext.Games
                .Include(g => g.PlayStatuses)
                .Where(g => g.User_Id == App.CurrentUser.Id);

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                string text = SearchBox.Text.Trim().ToLower();
                query = query.Where(g => g.Title.ToLower().Contains(text));
            }

            if (StatusFilterBox.SelectedItem is PlayStatuses status)
            {
                query = query.Where(g => g.PlayStatuses_Id == status.Id);
            }

            if (WishlistFilterBox.IsChecked == true)
            {
                query = query.Where(g => g.IsWishlist);
            }

            List<Games> games = await query
                .OrderBy(g => g.Title)
                .ToListAsync();

            List<GameCardItem> cards = new List<GameCardItem>();
            foreach (Games g in games)
            {
                var cover = CoverHelper.Load(g.CoverPath);
                string letter = "?";
                if (!string.IsNullOrWhiteSpace(g.Title))
                    letter = g.Title.Substring(0, 1).ToUpper();

                string statusName = g.PlayStatuses != null ? g.PlayStatuses.Name : "";
                if (g.IsWishlist)
                    statusName = statusName + " · wishlist";

                cards.Add(new GameCardItem
                {
                    Game = g,
                    Cover = cover,
                    HasCover = cover != null,
                    ShowPlaceholder = cover == null,
                    Letter = letter,
                    StatusName = statusName,
                    Meta = g.ReleaseYear + " · ★ " + g.Rating + " · " + g.PlaytimeMinutes + " мин"
                });
            }

            GamesList.ItemsSource = cards;
            StatusText.Text = "Игр: " + cards.Count;
        }

        private void Card_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is not Border border || border.Tag is not GameCardItem item)
                return;

            _selected = item;

            foreach (var child in GamesList.GetVisualDescendants().OfType<Border>())
            {
                if (child.Classes.Contains("gamecard"))
                    child.Classes.Remove("selected");
            }

            border.Classes.Add("selected");
        }

        private void Card_DoubleTapped(object? sender, TappedEventArgs e)
        {
            if (_selected == null)
                return;

            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new GameDetails(_selected.Game);
        }

        private Games? GetSelectedGame()
        {
            if (_selected == null)
            {
                StatusText.Text = "Сначала выбери карточку";
                return null;
            }
            return _selected.Game;
        }

        private async void RefreshButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await LoadGameAsync();
        }

        private async void FilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await LoadGameAsync();
        }

        private async void ResetFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SearchBox.Text = "";
            StatusFilterBox.SelectedItem = null;
            WishlistFilterBox.IsChecked = false;
            await LoadGameAsync();
        }

        private void LogoutButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            App.CurrentUser = null;
            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new Login();
        }

        private void StatsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new Statistics();
        }

        private void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new GameEdit();
        }

        private void EditButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Games? game = GetSelectedGame();
            if (game == null)
                return;

            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new GameEdit(game);
        }

        private void DetailsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Games? game = GetSelectedGame();
            if (game == null)
                return;

            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new GameDetails(game);
        }

        private async void PlayButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Games? game = GetSelectedGame();
            if (game == null)
                return;

            StatusText.Text = "Игра запущена... Закрой игру, чтобы сохранить сессию.";
            var result = await GameLauncher.LaunchAndTrackAsync(game);
            StatusText.Text = result.Message;
            if (result.Ok && result.Minutes > 0)
                await LoadGameAsync();
        }

        private async void DeleteButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Games? game = GetSelectedGame();
            if (game == null)
                return;

            var dialog = new ConfirmDialog("Удалить игру \"" + game.Title + "\"?");
            if (TopLevel.GetTopLevel(this) is Window owner)
                await dialog.ShowDialog(owner);
            else
                dialog.Show();

            if (!dialog.Confirmed)
                return;

            App.DbContext.Games.Remove(game);
            await App.DbContext.SaveChangesAsync();
            await LoadGameAsync();
        }
    }
}
