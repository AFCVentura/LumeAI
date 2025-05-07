namespace LumeAI.Models.Movie
{
    public class MovieSpokenLanguage
    {
        public int MovieId { get; set; }
        public int SpokenLanguageId { get; set; }

        public Movie Movie { get; set; }
        public SpokenLanguage SpokenLanguage { get; set; }
    }
}
