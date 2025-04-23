using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeAI
{
    class MovieData
    {
        [LoadColumn(1)] public string Title { get; set; }
        [LoadColumn(2)] public float VoteAverage { get; set; }
        [LoadColumn(3)] public int VoteCount { get; set; }
        [LoadColumn(4)] public string Status { get; set; }
        [LoadColumn(5)] public string ReleaseDate { get; set; }
        [LoadColumn(8)] public bool Adult { get; set; }
        [LoadColumn(13)] public string OriginalLanguage { get; set; }
        [LoadColumn(17)] public string PosterPath { get; set; }
        [LoadColumn(19)] public string Genres { get; set; }
        [LoadColumn(21)] public string ProductionCountries { get; set; }
        [LoadColumn(23)] public string Keywords { get; set; }

        public float ReleaseYear { get; set; } // calculado depois
    }
}
