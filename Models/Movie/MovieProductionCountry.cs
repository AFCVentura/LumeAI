namespace LumeAI.Models.Movie
{
    public class MovieProductionCountry
    {
        public int MovieId { get; set; }
        public int ProductionCountryId { get; set; }

        public Movie Movie { get; set; }
        public ProductionCountry ProductionCountry { get; set; }
    }
}
