namespace LumeAI.Models.Movie
{
    public class MovieKeyword
    {
        public int MovieId { get; set; }
        public int KeywordId { get; set; }

        public Movie Movie { get; set; }
        public Keyword Keyword { get; set; }
    }
}
