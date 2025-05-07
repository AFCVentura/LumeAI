using LumeAI.Models.Movie;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LumeAI.Data.Mappings.Movie
{
    public class ProductionCountryMap : IEntityTypeConfiguration<ProductionCountry>
    {
        public void Configure(EntityTypeBuilder<ProductionCountry> builder)
        {
            builder.HasKey(k => k.Id);

            builder.Property(k => k.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(k => k.MovieProductionCountries)
                   .WithOne(mk => mk.ProductionCountry)
                   .HasForeignKey(mk => mk.ProductionCountryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
