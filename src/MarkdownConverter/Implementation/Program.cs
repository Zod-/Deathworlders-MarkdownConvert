using Markdig;
using MarkdownConverter.Properties;
using McMaster.Extensions.CommandLineUtils;
using net.vieapps.Components.Utility.Epub;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using DinkToPdf;

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
            var chaptersHtml = ChapterUtils.SplitIntoChapters(html, "Date Point").ToList();
            var chapters = ChapterUtils.GetMetaForChapters(chaptersHtml).ToList();

            GenerateEpub(MarkdownFile.Replace(".md", ".epub"), chapters, mergedMeta);
            GeneratePdf(MarkdownFile.Replace(".md", ".pdf"), chapters, mergedMeta);
            return 0;
        }

        private static void GenerateEpub(string path, IReadOnlyList<Chapter> chaptersHtml, MetaInformation meta)
        {
            var epub = new Document();
            epub.AddBookIdentifier(Guid.NewGuid().ToString());
            epub.AddLanguage("English");
            //TODO Publish date
            epub.AddMetaItem("dc:date", meta.PublishDate.ToString("YYYY-MM-DD"));
            epub.AddStylesheetData("style.css", Resources.style);

            epub.AddTitle(meta.Title);
            epub.AddAuthor(meta.Author);

            var chapterNumber = 0;

            if (meta.CoverImageFile?.Exists == true)
            {
                var coverImageId = epub.AddImageData(meta.CoverImageFile.Name, File.ReadAllBytes(meta.CoverImageFile.FullName));
                epub.AddMetaItem("cover", coverImageId);

                var coverPage = Resources.epub_cover_template.Replace("{cover}", meta.CoverImageFile.Name);
                epub.AddXhtmlData($"page{chapterNumber}.xhtml", coverPage);
                epub.AddNavPoint("Cover", $"page{chapterNumber}.xhtml", chapterNumber);
                chapterNumber++;
            }

            var chapterTemplate = Resources.epub_template.Replace("{title}", meta.Title);
            for (var i = 0; i < chaptersHtml.Count; i++, chapterNumber++)
            {
                var chapter = chaptersHtml[i];
                var chapterFile = $"page{chapterNumber}.xhtml";
                var chapterContent = chapterTemplate.Replace("{body}", chapter.Html);
                epub.AddXhtmlData(chapterFile, chapterContent);
                epub.AddNavPoint(chapter.GetNavPoint(i + 1), chapterFile, chapterNumber);
            }

            epub.Generate(path);
        }

        private static void GeneratePdf(string path, IEnumerable<Chapter> chaptersHtml, MetaInformation meta)
        {
            var joinedChapters = string.Join("\n<div class=\"pagebreak\"></div>\n", chaptersHtml.Select(c => c.Html));

            var cover = string.Empty;

            var chapterContent = Resources.pdf_template
                .Replace("{title}", meta.Title)
                .Replace("{style}", Resources.style)
                .Replace("{body}", joinedChapters)
                .Replace("{cover}", cover);

            var settings = new GlobalSettings
            {
                UseCompression = true,
                PaperSize = PaperKind.A4,
                DocumentTitle = meta.Title
            };

            var chapterSettings = new ObjectSettings
            {
                HtmlContent = chapterContent
            };

            var pdfDocument = new HtmlToPdfDocument
            {
                GlobalSettings = settings,
                Objects = { chapterSettings }
            };

            var converter = new BasicConverter(new PdfTools());

            var bytes = converter.Convert(pdfDocument);
            File.WriteAllBytes(path, bytes);
        }
    }
}
