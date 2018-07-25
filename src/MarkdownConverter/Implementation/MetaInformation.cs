using System;
using System.IO;

namespace MarkdownConverter
{
    public class MetaInformation
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public FileInfo CoverImageFile { get; set; }
        public DateTime PublishDate { get; set; }
    }
}