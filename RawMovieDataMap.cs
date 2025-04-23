using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeAI
{
    // classe para mapear os dados do CSV antigo
    class RawMovieDataMap : ClassMap<RawMovieData>
    {

        public RawMovieDataMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.Title).Name("title");
            Map(m => m.VoteAverage).Name("vote_average");
            Map(m => m.VoteCount).Name("vote_count");
            Map(m => m.Status).Name("status");
            Map(m => m.ReleaseDate).Name("release_date");
            Map(m => m.Revenue).Name("revenue");
            Map(m => m.Runtime).Name("runtime");
            Map(m => m.Adult).Name("adult");
            Map(m => m.BackdropPath).Name("backdrop_path");
            Map(m => m.Budget).Name("budget");
            Map(m => m.Homepage).Name("homepage");
            Map(m => m.ImdbId).Name("imdb_id");
            Map(m => m.OriginalLanguage).Name("original_language");
            Map(m => m.OriginalTitle).Name("original_title");
            Map(m => m.Overview).Name("overview");
            Map(m => m.Popularity).Name("popularity");
            Map(m => m.PosterPath).Name("poster_path");
            Map(m => m.Tagline).Name("tagline");
            Map(m => m.Genres).Name("genres");
            Map(m => m.ProductionCompanies).Name("production_companies");
            Map(m => m.ProductionCountries).Name("production_countries");
            Map(m => m.SpokenLanguages).Name("spoken_languages");
            Map(m => m.Keywords).Name("keywords");
        }
    }
}
