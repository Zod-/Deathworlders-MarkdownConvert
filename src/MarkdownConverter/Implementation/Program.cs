using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Markdig;
using MarkdownConverter.Properties;
using McMaster.Extensions.CommandLineUtils;
using net.vieapps.Components.Utility.Epub;

namespace MarkdownConverter
{
    [HelpOption]
    public class Program
    {
        [Required]
        [Argument(0, Description = "The markdown.md file to be converted.")]
        [FileExists]
        public string MarkdownFile { get; set; }



        private static int Main(string[] args)
        {
            return CommandLineApplication.Execute<Program>(args);
        }

        private int OnExecute()
        {
            var markdown = File.ReadAllText(MarkdownFile);
            var chapterYamlText = MetaUtils.GetYamlHeader(markdown);
            markdown = markdown.Remove(0, chapterYamlText.Length);

            var chapterMeta = MetaUtils.GetMetaFromYaml(chapterYamlText);
            var indexMeta = MetaUtils.FindBookIndexMeta(MarkdownFile);
            var imageMeta = MetaUtils.FindBookCoverMeta(MarkdownFile);
            var mergedMeta = MetaUtils.MergeMeta(indexMeta, chapterMeta, imageMeta);

            var html = Markdown.ToHtml(markdown);
            var chaptersHtml = SplitIntoChapters(html, "Date Point").ToList();

            GenerateEpub(MarkdownFile.Replace(".md", ".epub"), chaptersHtml, mergedMeta);

            return 0;
        }

        private static IEnumerable<string> SplitIntoChapters(string html, string chapterSplit)
        {
            var lines = html.Split("\n");
            var buffer = new StringBuilder();
            foreach (var line in lines)
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

        private static void GenerateEpub(string path, IReadOnlyList<string> chaptersHtml, MetaInformation meta)
        {
            var epub = new Document();
            epub.AddBookIdentifier(Guid.NewGuid().ToString());
            epub.AddLanguage("English");
            epub.AddStylesheetData("style.css", Resources.style);

            epub.AddTitle(meta.Title);
            epub.AddAuthor(meta.Author);
            if (meta.CoverImageFile?.Exists == true)
            {
                var coverImageId = epub.AddImageData(meta.CoverImageFile.Name, File.ReadAllBytes(meta.CoverImageFile.FullName));
                epub.AddMetaItem("cover", coverImageId);
            }

            var chapterTemplate = Resources.epub_template.Replace("{title}", meta.Title);

            for (var i = 0; i < chaptersHtml.Count; i++)
            {
                var chapterFile = $"page{i}.xhtml";
                var chapterContent = chapterTemplate.Replace("{body}", chaptersHtml[i]);
                epub.AddXhtmlData(chapterFile, chapterContent);
            }

            epub.Generate(path);
        }
    }
}
