namespace LumeAI.Models.Movie
{
    public class Movie
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public float VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public string? Status { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public float Runtime { get; set; }
        public bool Adult { get; set; }
        public string? BackdropPath { get; set; }
        public long Budget { get; set; }
        public string? Homepage { get; set; }
        public string? ImdbId { get; set; }
        public string? OriginalLanguage { get; set; }
        public string? OriginalTitle { get; set; }
        public string? Overview { get; set; }
        public float Popularity { get; set; }
        public string PosterPath { get; set; }
        public string? Tagline { get; set; }
        public int ClusterId { get; set; }
        public Cluster Cluster { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; }
        public ICollection<MovieKeyword> MovieKeywords { get; set; }
        public ICollection<MovieProductionCompany> MovieProductionCompanies { get; set; }
        public ICollection<MovieProductionCountry> MovieProductionCountries { get; set; }
        public ICollection<MovieSpokenLanguage> MovieSpokenLanguages { get; set; }
    }
}
