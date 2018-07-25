using System;
using System.IO;
using Xamarin.Yaml.Parser;

namespace MarkdownConverter
{
    public static class MetaUtils
    {
        private const string StartEndIndicator = "---";
        public static string GetYamlHeader(string text)
        {
            var headerStart = text.IndexOf(StartEndIndicator, StringComparison.InvariantCulture);
            var headerEnd = text.IndexOf(StartEndIndicator, headerStart + 1, StringComparison.InvariantCulture);
            if (headerStart == -1 || headerEnd == -1)
            {
                return string.Empty;
            }
            var header = text.Substring(headerStart, headerEnd + StartEndIndicator.Length);
            return header;
        }

        public static MetaInformation GetMetaFromYaml(string yamlText)
        {
            var chapterYaml = new Parser(new ParserConfig(), yamlText);
            var meta = new MetaInformation
            {
                Author = chapterYaml.FindValue("author"),
                Title = chapterYaml.FindValue("title")
            };

            var date = chapterYaml.FindValue("date");
            if (!string.IsNullOrEmpty(date))
            {
                meta.PublishDate = DateTime.Parse(date);
            }

            return meta;
        }

        public static MetaInformation MergeMeta(params MetaInformation[] metas)
        {
            var mergedMeta = new MetaInformation();
            foreach (var meta in metas)
            {
                if (meta == null)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(meta.Author))
                {
                    mergedMeta.Author = meta.Author;
                }
                if (meta.CoverImageFile?.Exists == true)
                {
                    mergedMeta.CoverImageFile = meta.CoverImageFile;
                }
                if (!string.IsNullOrEmpty(meta.Title))
                {
                    mergedMeta.Title = $"{mergedMeta.Title} {meta.Title}";
                }
                if (mergedMeta.PublishDate < meta.PublishDate)
                {
                    mergedMeta.PublishDate = meta.PublishDate;
                }
            }
            return mergedMeta;
        }

        public static MetaInformation FindBookIndexMeta(string markdownFile)
        {
            var mdFileInfo = new FileInfo(markdownFile);
            var indexFile = Path.Combine(mdFileInfo.DirectoryName, "_index.md");
            if (!File.Exists(indexFile))
            {
                return default(MetaInformation);
            }

            var bookIndexText = File.ReadAllText(indexFile);
            var bookYamlText = GetYamlHeader(bookIndexText);
            var bookMeta = GetMetaFromYaml(bookYamlText);
            return bookMeta;
        }

        public static MetaInformation FindBookCoverMeta(string markdownFile)
        {
            var mdFileInfo = new FileInfo(markdownFile);
            foreach (var coverExtension in new[] { ".png", ".jpg", ".jpeg" })
            {
                var coverFile = mdFileInfo.FullName.Replace(".md", coverExtension);
                if (!File.Exists(coverFile))
                {
                    continue;
                }

                return new MetaInformation
                {
                    CoverImageFile = new FileInfo(coverFile)
                };
            }

            return default(MetaInformation);
        }
    }
}