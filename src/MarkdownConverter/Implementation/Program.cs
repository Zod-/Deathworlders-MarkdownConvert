using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Xamarin.Yaml.Parser;

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

            var chapterYamlText = YamlUtil.GetYamlHeader(markdown, "---");
            markdown = markdown.Remove(0, chapterYamlText.Length);
            var chapterMeta = YamlUtil.GetMetaFromYaml(chapterYamlText);


            return 0;
        }
    }
}
