using System;
using System.IO;

namespace Deathworlders.MarkdownConvert
{
    public static class FileUtils
    {
        public static string GetFilePath(string sourceFile, string extension, string outFolder, bool keepParentFolder)
        {
            var filename = Path.ChangeExtension(sourceFile, extension);
            if (string.IsNullOrEmpty(outFolder))
            {
                return filename;
            }

            if (keepParentFolder)
            {
                var directoryName = Path.GetFileName(Path.GetDirectoryName(sourceFile));
                outFolder = Path.Combine(outFolder, directoryName);
            }

            filename = Path.Combine(outFolder, Path.GetFileName(filename));
            return filename;
        }

        public static void CreateDirectory(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directoryName);
        }
    }
}