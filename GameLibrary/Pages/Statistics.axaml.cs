using Avalonia.Controls;
using GameLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Pages
{
    public partial class Statistics : UserControl
    {
        public Statistics()
        {
            InitializeComponent();
            Loaded += Statistics_Loaded;
        }

        private async void Statistics_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
                return;

            int userId = App.CurrentUser.Id;
            List<Games> games = await App.DbContext.Games
                .Include(g => g.PlayStatuses)
                .Where(g => g.User_Id == userId)
                .ToListAsync();

            TotalText.Text = games.Count.ToString();
            WishlistText.Text = games.Count(g => g.IsWishlist).ToString();

            List<Games> rated = games.Where(g => g.Rating > 0).ToList();
            if (rated.Count == 0)
            {
                AverageText.Text = "—";
                AverageHint.Text = "Оценок пока нет";
            }
            else
            {
                double average = rated.Average(g => g.Rating);
                AverageText.Text = average.ToString("0.0");
                AverageHint.Text = "По " + rated.Count + " из " + games.Count;
            }

            int minutes = games.Sum(g => g.PlaytimeMinutes);
            TimeText.Text = FormatMinutes(minutes);
            TimeHint.Text = minutes + " мин";

            List<StatRow> statuses = games
                .GroupBy(g => g.PlayStatuses != null ? g.PlayStatuses.Name : "Без статуса")
                .Select(g => new StatRow { Name = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ThenBy(g => g.Name)
                .ToList();
            StatusList.ItemsSource = statuses;
            StatusEmpty.IsVisible = statuses.Count == 0;

            List<int> gameIds = games.Select(g => g.Id).ToList();
            if (gameIds.Count == 0)
            {
                PlatformEmpty.IsVisible = true;
                GenreEmpty.IsVisible = true;
                return;
            }

            List<GamePlatforms> platforms = await App.DbContext.GamePlatforms
                .Include(x => x.Platforms)
                .Where(x => gameIds.Contains(x.Game_Id))
                .ToListAsync();
            List<StatRow> platformRows = platforms
                .GroupBy(x => x.Platforms != null ? x.Platforms.Name : "Без названия")
                .Select(g => new StatRow { Name = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ThenBy(g => g.Name)
                .ToList();
            int withoutPlatform = games.Count - platforms.Select(x => x.Game_Id).Distinct().Count();
            if (withoutPlatform > 0)
                platformRows.Add(new StatRow { Name = "Без платформы", Count = withoutPlatform });
            PlatformList.ItemsSource = platformRows;
            PlatformEmpty.IsVisible = platformRows.Count == 0;

            List<GameGenres> genres = await App.DbContext.GameGenres
                .Include(x => x.Genres)
                .Where(x => gameIds.Contains(x.Game_Id))
                .ToListAsync();
            List<StatRow> genreRows = genres
                .GroupBy(x => x.Genres != null ? x.Genres.Name : "Без названия")
                .Select(g => new StatRow { Name = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ThenBy(g => g.Name)
                .ToList();
            int withoutGenre = games.Count - genres.Select(x => x.Game_Id).Distinct().Count();
            if (withoutGenre > 0)
                genreRows.Add(new StatRow { Name = "Без жанра", Count = withoutGenre });
            GenreList.ItemsSource = genreRows;
            GenreEmpty.IsVisible = genreRows.Count == 0;
        }

        private static string FormatMinutes(int minutes)
        {
            if (minutes < 60)
                return minutes + " мин";
            int hours = minutes / 60;
            int rest = minutes % 60;
            if (rest == 0)
                return hours + " ч";
            return hours + " ч " + rest + " мин";
        }

        private void BackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new Library();
        }

        private class StatRow
        {
            public string Name { get; set; } = "";
            public int Count { get; set; }
        }
    }
}
