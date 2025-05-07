using Microsoft.ML.Data;


namespace LumeAI.DTOs
{
    // Essa classe serve apenas para mapear os dados do csv puro, o csv filtrado é gerado com base nos dados da classe MovieData
    class RawMovieData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public float VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public string Status { get; set; }
        public string ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public float Runtime { get; set; }
        public bool Adult { get; set; }
        public string BackdropPath { get; set; }
        public long Budget { get; set; }
        public string Homepage { get; set; }
        public string ImdbId { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public float Popularity { get; set; }
        public string PosterPath { get; set; }
        public string Tagline { get; set; }
        public string Genres { get; set; }
        public string ProductionCompanies { get; set; }
        public string ProductionCountries { get; set; }
        public string SpokenLanguages { get; set; }
        public string Keywords { get; set; }
    }
}
