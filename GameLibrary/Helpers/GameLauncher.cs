using GameLibrary.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace GameLibrary.Helpers
{
    public class LaunchResult
    {
        public bool Ok { get; set; }
        public string Message { get; set; } = "";
        public int Minutes { get; set; }
    }

    public static class GameLauncher
    {
        // Как работает сессия:
        // 1) запоминаем время старта
        // 2) запускаем .exe
        // 3) ждём, пока процесс закроется
        // 4) считаем минуты и пишем PlaySessions + PlaytimeMinutes
        public static async Task<LaunchResult> LaunchAndTrackAsync(Games game)
        {
            if (string.IsNullOrWhiteSpace(game.LaunchPath))
            {
                return new LaunchResult
                {
                    Ok = false,
                    Message = "Сначала укажи путь к .exe в редактировании игры"
                };
            }

            if (!File.Exists(game.LaunchPath))
            {
                return new LaunchResult
                {
                    Ok = false,
                    Message = "Файл не найден: " + game.LaunchPath
                };
            }

            DateTime start = DateTime.UtcNow;

            try
            {
                var info = new ProcessStartInfo
                {
                    FileName = game.LaunchPath,
                    WorkingDirectory = Path.GetDirectoryName(game.LaunchPath) ?? "",
                    UseShellExecute = true
                };

                Process? process = Process.Start(info);
                if (process == null)
                {
                    return new LaunchResult
                    {
                        Ok = false,
                        Message = "Не удалось запустить игру"
                    };
                }

                await process.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                return new LaunchResult
                {
                    Ok = false,
                    Message = "Ошибка запуска: " + ex.Message
                };
            }

            double seconds = (DateTime.UtcNow - start).TotalSeconds;

            // короткие запуски (лаунчер сразу закрылся) не сохраняем
            if (seconds < 30)
            {
                return new LaunchResult
                {
                    Ok = true,
                    Minutes = 0,
                    Message = "Сессия короче 30 сек — не сохранил"
                };
            }

            int minutes = (int)Math.Round(seconds / 60.0);
            if (minutes < 1)
                minutes = 1;

            App.DbContext.PlaySessions.Add(new PlaySessions
            {
                Game_Id = game.Id,
                Minutes = minutes,
                PlayedOn = DateTime.UtcNow.Date,
                Comment = "Автосессия из лаунчера",
                CreateAt = DateTime.UtcNow
            });

            game.PlaytimeMinutes = game.PlaytimeMinutes + minutes;
            game.UpdateAt = DateTime.UtcNow;
            await App.DbContext.SaveChangesAsync();

            return new LaunchResult
            {
                Ok = true,
                Minutes = minutes,
                Message = "Сохранена сессия: " + minutes + " мин."
            };
        }
    }
}
