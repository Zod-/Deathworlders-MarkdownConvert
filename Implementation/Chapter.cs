namespace Deathworlders.MarkdownConvert
{
    public class Chapter
    {
        public string DatePoint { get; set; }
        public string Location { get; set; }
        public string Protagonist { get; set; }
        public string Html { get; set; }

        public string GetNavPoint(int chapter)
        {
            if (!string.IsNullOrEmpty(Protagonist))
            {
                return Protagonist;
            }
            if (!string.IsNullOrEmpty(Location))
            {
                return Location;
            }
            if (!string.IsNullOrEmpty(DatePoint))
            {
                return DatePoint;
            }

            return $"Chapter {chapter}";
        }
    }
}