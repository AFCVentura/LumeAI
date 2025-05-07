namespace LumeAI.Models.Movie
{
    public class ProductionCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<MovieProductionCompany> MovieProductionCompanies { get; set; }
    }
}
