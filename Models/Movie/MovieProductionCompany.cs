namespace LumeAI.Models.Movie
{
    public class MovieProductionCompany
    {
        public int MovieId { get; set; }
        public int ProductionCompanyId { get; set; }

        public Movie Movie { get; set; }
        public ProductionCompany ProductionCompany { get; set; }
    }
}
