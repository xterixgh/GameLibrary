using Avalonia.Media.Imaging;
using System;
using System.IO;

namespace GameLibrary.Helpers
{
    public static class CoverHelper
    {
        public static string CoversFolder
        {
            get
            {
                string folder = Path.Combine(AppContext.BaseDirectory, "Covers");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                return folder;
            }
        }

        public static Bitmap? Load(string? path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return null;

                string fullPath = path;
                if (!Path.IsPathRooted(path))
                    fullPath = Path.Combine(CoversFolder, path);

                if (!File.Exists(fullPath))
                    return null;

                return new Bitmap(fullPath);
            }
            catch
            {
                return null;
            }
        }

        public static string SaveCopy(string sourcePath, int userId)
        {
            string ext = Path.GetExtension(sourcePath);
            if (string.IsNullOrWhiteSpace(ext))
                ext = ".png";

            string fileName = "cover_" + userId + "_" + DateTime.Now.Ticks + ext;
            string dest = Path.Combine(CoversFolder, fileName);
            File.Copy(sourcePath, dest, true);
            return fileName;
        }
    }
}
