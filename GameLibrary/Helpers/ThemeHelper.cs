using Avalonia;
using Avalonia.Styling;
using System;
using System.IO;

namespace GameLibrary.Helpers
{
    public static class ThemeHelper
    {
        private static string FilePath => Path.Combine(AppContext.BaseDirectory, "theme.txt");

        public static bool IsDark { get; private set; } = true;

        public static void Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string text = File.ReadAllText(FilePath).Trim().ToLower();
                    IsDark = text != "light";
                }
            }
            catch
            {
                IsDark = true;
            }

            Apply();
        }

        public static void Toggle()
        {
            IsDark = !IsDark;
            Apply();
            Save();
        }

        public static void Apply()
        {
            if (Application.Current == null)
                return;

            Application.Current.RequestedThemeVariant = IsDark
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
        }

        private static void Save()
        {
            try
            {
                File.WriteAllText(FilePath, IsDark ? "dark" : "light");
            }
            catch
            {
            }
        }

        public static string ButtonText()
        {
            return IsDark ? "Светлая тема" : "Тёмная тема";
        }
    }
}
