using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeAI
{
    // Essa classe serve para gerar o csv filtrado e como mapeador desse csv filtrado para treinar a IA
    class MovieData
    {
        [LoadColumn(0)] public string Id { get; set; }
        [LoadColumn(1)] public string Title { get; set; }
        [LoadColumn(2)] public float VoteAverage { get; set; }
        [LoadColumn(3)] public int VoteCount { get; set; }
        [LoadColumn(4)] public string Status { get; set; }
        [LoadColumn(5)] public float ReleaseYear { get; set; }
        [LoadColumn(6)] public long Revenue { get; set; }
        [LoadColumn(7)] public float Runtime { get; set; }
        [LoadColumn(8)] public bool Adult { get; set; }
        [LoadColumn(9)] public string BackdropPath { get; set; }
        [LoadColumn(10)] public long Budget { get; set; }
        [LoadColumn(11)] public string Homepage { get; set; }
        [LoadColumn(12)] public string ImdbId { get; set; }
        [LoadColumn(13)] public string OriginalLanguage { get; set; }
        [LoadColumn(14)] public string OriginalTitle { get; set; }
        [LoadColumn(15)] public string Overview { get; set; }
        [LoadColumn(16)] public float Popularity { get; set; }
        [LoadColumn(17)] public string PosterPath { get; set; }
        [LoadColumn(18)] public string Tagline { get; set; }
        [LoadColumn(19)] public string Genres { get; set; }
        [LoadColumn(20)] public string ProductionCompanies { get; set; }
        [LoadColumn(21)] public string ProductionCountries { get; set; }
        [LoadColumn(22)] public string SpokenLanguages { get; set; }
        [LoadColumn(23)] public string Keywords { get; set; }

    }
}
