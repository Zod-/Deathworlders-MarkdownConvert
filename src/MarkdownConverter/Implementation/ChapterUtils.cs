using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownConverter
{
    public static class ChapterUtils
    {
        public static IEnumerable<Chapter> GetMetaForChapters(IEnumerable<string> chaptersHtml)
        {
            foreach (var chapterHtml in chaptersHtml)
            {
                var split = chapterHtml.Split("\n");
                var chapter = new Chapter
                {
                    Html = chapterHtml,
                    DatePoint = TryGetChapterMeta(split, 0),
                    Location = TryGetChapterMeta(split, 1),
                    Protagonist = TryGetChapterMeta(split, 2)
                };
                yield return chapter;
            }
        }

        public static string TryGetChapterMeta(IReadOnlyList<string> split, int index)
        {
            return string.Empty;
            //TODO Add nav points to all markdowns or try to guess what the chapter meta should be.
            //if (index >= split.Count)
            //{
            //  return string.Empty;
            //}
            //Match match = Regex.Match(split[index], @"<strong>([^<]+)<");
            //return match.Success ? match.Groups[1].Value : string.Empty;
        }

        public static IEnumerable<string> SplitIntoChapters(string html, string chapterSplit)
        {
            var buffer = new StringBuilder();
            foreach (var line in html.Split("\n"))
            {
                if (Regex.IsMatch(line, chapterSplit) && buffer.Length != 0)
                {
                    yield return buffer.ToString();
                    buffer.Clear();
                }
                buffer.AppendLine(line);
            }

            if (buffer.Length != 0)
            {
                yield return buffer.ToString();
            }
        }
    }
}
