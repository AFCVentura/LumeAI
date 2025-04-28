using LumeAI.Models.Movie;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LumeAI.Data.Mappings.Movie
{
    public class MovieProductionCountryMap : IEntityTypeConfiguration<MovieProductionCountry>
    {
        public void Configure(EntityTypeBuilder<MovieProductionCountry> builder)
        {
            builder.HasKey(mk => new { mk.MovieId, mk.ProductionCountryId }); // Chave composta

            builder.HasOne(mk => mk.Movie)
                   .WithMany(m => m.MovieProductionCountries)
                   .HasForeignKey(mk => mk.MovieId);

            builder.HasOne(mk => mk.ProductionCountry)
                   .WithMany(k => k.MovieProductionCountries)
                   .HasForeignKey(mk => mk.ProductionCountryId);
        }
    }
}
