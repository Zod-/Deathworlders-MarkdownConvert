using System;
using System.IO;
using YamlDotNet.Serialization;

namespace MarkdownConverter
{
    public class MetaInformation
    {
        [YamlMember(Alias = "author")]
        public string Author { get; set; }
        [YamlMember(Alias = "title")]
        public string Title { get; set; }
        public FileInfo CoverImageFile { get; set; }
        [YamlMember(Alias = "date")]
        public DateTime PublishDate { get; set; }
    }
}