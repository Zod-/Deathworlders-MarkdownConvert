using System;
using Xamarin.Yaml.Parser;

namespace MarkdownConverter
{
    public static class YamlUtil
    {
        public static string GetYamlHeader(string text, string startEndIndicator)
        {
            var headerStart = text.IndexOf(startEndIndicator, StringComparison.InvariantCulture);
            var headerEnd = text.IndexOf(startEndIndicator, headerStart + 1, StringComparison.InvariantCulture);
            if (headerStart == -1 || headerEnd == -1)
            {
                return string.Empty;
            }
            var header = text.Substring(headerStart, headerEnd + startEndIndicator.Length);
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
                meta.Date = DateTime.Parse(date);
            }

            return meta;
        }
    }
}