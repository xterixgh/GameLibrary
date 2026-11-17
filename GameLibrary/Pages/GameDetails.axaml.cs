using Avalonia.Controls;
using GameLibrary.Helpers;
using GameLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Pages
{
    public partial class GameDetails : UserControl
    {
        private Games? _game;

        public GameDetails()
        {
            InitializeComponent();
        }

        public GameDetails(Games game)
        {
            InitializeComponent();
            _game = game;
            Loaded += GameDetails_Loaded;
        }

        private async void GameDetails_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_game == null)
                return;

            TitleText.Text = _game.Title;
            HeroTitle.Text = _game.Title;
            InfoText.Text = "Год: " + _game.ReleaseYear +
                            " | Оценка: " + _game.Rating +
                            " | Минуты: " + _game.PlaytimeMinutes;
            HeroMeta.Text = InfoText.Text;
            DescText.Text = string.IsNullOrWhiteSpace(_game.Description)
                ? "Описания нет"
                : _game.Description;

            var cover = CoverHelper.Load(_game.CoverPath);
            if (cover == null)
            {
                CoverImage.IsVisible = false;
                CoverLetter.IsVisible = true;
                CoverLetter.Text = string.IsNullOrWhiteSpace(_game.Title)
                    ? "?"
                    : _game.Title.Substring(0, 1).ToUpper();
            }
            else
            {
                CoverImage.Source = cover;
                CoverImage.IsVisible = true;
                CoverLetter.IsVisible = false;
            }

            GenreBox.ItemsSource = await App.DbContext.Genres.OrderBy(x => x.Name).ToListAsync();
            PlatformBox.ItemsSource = await App.DbContext.Platforms.OrderBy(x => x.Name).ToListAsync();
            TagBox.ItemsSource = await App.DbContext.Tags.OrderBy(x => x.Name).ToListAsync();

            PlayButton.IsEnabled = !string.IsNullOrWhiteSpace(_game.LaunchPath);
            if (string.IsNullOrWhiteSpace(_game.LaunchPath))
                StatusText.Text = "Чтобы играть отсюда — укажи путь к .exe в редактировании";

            await LoadListsAsync();
        }

        private async void PlayButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_game == null)
                return;

            PlayButton.IsEnabled = false;
            StatusText.Text = "Игра запущена... Закрой игру, чтобы сохранить сессию.";

            var result = await GameLauncher.LaunchAndTrackAsync(_game);

            InfoText.Text = "Год: " + _game.ReleaseYear +
                            " | Оценка: " + _game.Rating +
                            " | Минуты: " + _game.PlaytimeMinutes;
            HeroMeta.Text = InfoText.Text;

            await LoadListsAsync();
            StatusText.Text = result.Message;
            PlayButton.IsEnabled = !string.IsNullOrWhiteSpace(_game.LaunchPath);
        }

        private async Task LoadListsAsync()
        {
            GenresList.ItemsSource = await App.DbContext.GameGenres
                .Include(x => x.Genres)
                .Where(x => x.Game_Id == _game.Id)
                .ToListAsync();

            PlatformsList.ItemsSource = await App.DbContext.GamePlatforms
                .Include(x => x.Platforms)
                .Where(x => x.Game_Id == _game.Id)
                .ToListAsync();

            TagsList.ItemsSource = await App.DbContext.GameTags
                .Include(x => x.Tags)
                .Where(x => x.Game_Id == _game.Id)
                .ToListAsync();

            NotesList.ItemsSource = await App.DbContext.Notes
                .Where(x => x.Game_Id == _game.Id)
                .OrderByDescending(x => x.CreateAt)
                .ToListAsync();

            SessionsList.ItemsSource = await App.DbContext.PlaySessions
                .Where(x => x.Game_Id == _game.Id)
                .OrderByDescending(x => x.CreateAt)
                .ToListAsync();

            StatusText.Text = "";
        }

        private async void AddGenre_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (GenreBox.SelectedItem is not Genres genre)
            {
                StatusText.Text = "Выбери жанр";
                return;
            }

            bool exists = await App.DbContext.GameGenres
                .AnyAsync(x => x.Game_Id == _game.Id && x.Genre_Id == genre.Id);
            if (exists)
            {
                StatusText.Text = "Такой жанр уже есть";
                return;
            }

            App.DbContext.GameGenres.Add(new GameGenres
            {
                Game_Id = _game.Id,
                Genre_Id = genre.Id
            });
            await App.DbContext.SaveChangesAsync();
            await LoadListsAsync();
        }

        private async void RemoveGenre_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (GenresList.SelectedItem is not GameGenres item)
            {
                StatusText.Text = "Выбери жанр в списке";
                return;
            }

            App.DbContext.GameGenres.Remove(item);
            await App.DbContext.SaveChangesAsync();
            await LoadListsAsync();
        }

        private async void AddPlatform_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (PlatformBox.SelectedItem is not Platforms platform)
            {
                StatusText.Text = "Выбери платформу";
                return;
            }

            bool exists = await App.DbContext.GamePlatforms
                .AnyAsync(x => x.Game_Id == _game.Id && x.Platform_Id == platform.Id);
            if (exists)
            {
                StatusText.Text = "Такая платформа уже есть";
                return;
            }

            App.DbContext.GamePlatforms.Add(new GamePlatforms
            {
                Game_Id = _game.Id,
                Platform_Id = platform.Id
            });
            await App.DbContext.SaveChangesAsync();
            await LoadListsAsync();
        }

        private async void RemovePlatform_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (PlatformsList.SelectedItem is not GamePlatforms item)
            {
                StatusText.Text = "Выбери платформу в списке";
                return;
            }

            App.DbContext.GamePlatforms.Remove(item);
            await App.DbContext.SaveChangesAsync();
            await LoadListsAsync();
        }

        private async void AddTag_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TagBox.SelectedItem is not Tags tag)
            {
                StatusText.Text = "Выбери тег";
                return;
            }

            bool exists = await App.DbContext.GameTags
                .AnyAsync(x => x.Game_Id == _game.Id && x.Tag_Id == tag.Id);
            if (exists)
            {
                StatusText.Text = "Такой тег уже есть";
                return;
            }

            App.DbContext.GameTags.Add(new GameTags
            {
                Game_Id = _game.Id,
                Tag_Id = tag.Id
            });
            await App.DbContext.SaveChangesAsync();
            await LoadListsAsync();
        }

        private async void RemoveTag_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TagsList.SelectedItem is not GameTags item)
            {
                StatusText.Text = "Выбери тег в списке";
                return;
            }

            App.DbContext.GameTags.Remove(item);
            await App.DbContext.SaveChangesAsync();
            await LoadListsAsync();
        }

        private async void AddNote_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NoteBox.Text))
            {
                StatusText.Text = "Напиши заметку";
                return;
            }

            App.DbContext.Notes.Add(new Notes
            {
                Game_Id = _game.Id,
                Body = NoteBox.Text.Trim(),
                CreateAt = DateTime.UtcNow
            });
            await App.DbContext.SaveChangesAsync();
            NoteBox.Text = "";
            await LoadListsAsync();
        }

        private async void RemoveNote_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (NotesList.SelectedItem is not Notes note)
            {
                StatusText.Text = "Выбери заметку";
                return;
            }

            App.DbContext.Notes.Remove(note);
            await App.DbContext.SaveChangesAsync();
            await LoadListsAsync();
        }

        private async void AddSession_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!int.TryParse(SessionMinutesBox.Text, out int minutes) || minutes <= 0)
            {
                StatusText.Text = "Минуты должны быть числом больше 0";
                return;
            }

            string comment = SessionCommentBox.Text ?? "";

            App.DbContext.PlaySessions.Add(new PlaySessions
            {
                Game_Id = _game.Id,
                Minutes = minutes,
                PlayedOn = DateTime.UtcNow.Date,
                Comment = comment,
                CreateAt = DateTime.UtcNow
            });

            _game.PlaytimeMinutes = _game.PlaytimeMinutes + minutes;
            _game.UpdateAt = DateTime.UtcNow;

            await App.DbContext.SaveChangesAsync();

            SessionMinutesBox.Text = "";
            SessionCommentBox.Text = "";
            InfoText.Text = "Год: " + _game.ReleaseYear +
                            " | Оценка: " + _game.Rating +
                            " | Минуты: " + _game.PlaytimeMinutes;
            HeroMeta.Text = InfoText.Text;
            await LoadListsAsync();
        }

        private async void RemoveSession_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (SessionsList.SelectedItem is not PlaySessions session)
            {
                StatusText.Text = "Выбери сессию";
                return;
            }

            _game.PlaytimeMinutes = _game.PlaytimeMinutes - session.Minutes;
            if (_game.PlaytimeMinutes < 0)
                _game.PlaytimeMinutes = 0;
            _game.UpdateAt = DateTime.UtcNow;

            App.DbContext.PlaySessions.Remove(session);
            await App.DbContext.SaveChangesAsync();

            InfoText.Text = "Год: " + _game.ReleaseYear +
                            " | Оценка: " + _game.Rating +
                            " | Минуты: " + _game.PlaytimeMinutes;
            HeroMeta.Text = InfoText.Text;
            await LoadListsAsync();
        }

        private void BackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TopLevel.GetTopLevel(this) is MainWindow window)
                window.MainControl.Content = new Library();
        }
    }
}
