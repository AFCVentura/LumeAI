using LumeAI.Data.Mappings.Movie;
using LumeAI.Models;
using LumeAI.Models.Movie;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace LumeAI.Data
{
    public class LumeAIDataContext : DbContext
    {
        #region DbSets dos modelos do filme
        public DbSet<Cluster> Clusters { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<MovieKeyword> MovieKeywords { get; set; }
        public DbSet<MovieProductionCompany> MovieProductionCompanies { get; set; }
        public DbSet<MovieProductionCountry> MovieProductionCountries { get; set; }
        public DbSet<MovieSpokenLanguage> MovieSpokenLanguages { get; set; }
        public DbSet<ProductionCompany> ProductionCompanies { get; set; }
        public DbSet<ProductionCountry> ProductionCountries { get; set; }
        public DbSet<SpokenLanguage> SpokenLanguages { get; set; }
        #endregion

        #region Mapeamentos dos modelos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClusterMap());
            modelBuilder.ApplyConfiguration(new GenreMap());
            modelBuilder.ApplyConfiguration(new KeywordMap());
            modelBuilder.ApplyConfiguration(new MovieMap());
            modelBuilder.ApplyConfiguration(new MovieGenreMap());
            modelBuilder.ApplyConfiguration(new MovieKeywordMap());
            modelBuilder.ApplyConfiguration(new MovieProductionCompanyMap());
            modelBuilder.ApplyConfiguration(new MovieProductionCountryMap());
            modelBuilder.ApplyConfiguration(new MovieSpokenLanguageMap());
            modelBuilder.ApplyConfiguration(new ProductionCompanyMap());
            modelBuilder.ApplyConfiguration(new ProductionCountryMap());
            modelBuilder.ApplyConfiguration(new SpokenLanguageMap());
        }
        #endregion
        public LumeAIDataContext(DbContextOptions<LumeAIDataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.EnableSensitiveDataLogging();
        }

    }
}
