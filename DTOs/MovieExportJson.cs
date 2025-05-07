using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeAI.DTOs
{
    // Essa classe serve como DTO para gerar o json com todas as informações dos filmes
    class MovieExportJson
    {
        public string Id { get; set; }
        public string? Title { get; set; }
        public float? VoteAverage { get; set; }
        public int? VoteCount { get; set; }
        public string? Status { get; set; }
        public int? ReleaseYear { get; set; }
        public long? Revenue { get; set; }
        public float? Runtime { get; set; }
        public bool? Adult { get; set; }
        public string? BackdropPath { get; set; }
        public long? Budget { get; set; }
        public string? Homepage { get; set; }
        public string? ImdbId { get; set; }
        public string? OriginalLanguage { get; set; }
        public string? OriginalTitle { get; set; }
        public string? Overview { get; set; }
        public float? Popularity { get; set; }
        public string? PosterPath { get; set; }
        public string? Tagline { get; set; }
        public uint ClusterId { get; set; }


        // Listas normalizadas
        public List<string>? Genres { get; set; } = new();
        public List<string>? Keywords { get; set; } = new();
        public List<string>? ProductionCompanies { get; set; } = new();
        public List<string>? ProductionCountries { get; set; } = new();
        public List<string>? SpokenLanguages { get; set; } = new();
    }
}
