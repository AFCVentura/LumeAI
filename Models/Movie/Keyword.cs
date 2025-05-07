namespace LumeAI.Models.Movie
{
    public class Keyword
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<MovieKeyword> MovieKeywords { get; set; }
    }
}
