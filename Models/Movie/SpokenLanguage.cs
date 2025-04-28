namespace LumeAI.Models.Movie
{
    public class SpokenLanguage
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<MovieSpokenLanguage> MovieSpokenLanguages { get; set; }
    }
}
