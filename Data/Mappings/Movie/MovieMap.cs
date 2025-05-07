using Microsoft.EntityFrameworkCore;

namespace LumeAI.Data.Mappings.Movie
{
    // Tive que colocar o caminho da classe pois ele não entendeu que Movie era a classe e não o namespace
    public class MovieMap : IEntityTypeConfiguration<LumeAI.Models.Movie.Movie>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<LumeAI.Models.Movie.Movie> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title).HasMaxLength(255);

            builder.HasOne(m => m.Cluster)
                   .WithMany(c => c.Movies)
                   .HasForeignKey(m => m.ClusterId);

            builder.HasMany(m => m.MovieGenres)
                   .WithOne(mg => mg.Movie)
                   .HasForeignKey(mg => mg.MovieId);

            builder.HasMany(m => m.MovieKeywords)
                   .WithOne(mk => mk.Movie)
                   .HasForeignKey(mk => mk.MovieId);

            builder.HasMany(m => m.MovieProductionCompanies)
                   .WithOne(mk => mk.Movie)
                   .HasForeignKey(mk => mk.MovieId);

            builder.HasMany(m => m.MovieProductionCountries)
                   .WithOne(mk => mk.Movie)
                   .HasForeignKey(mk => mk.MovieId);

            builder.HasMany(m => m.MovieSpokenLanguages)
                   .WithOne(mk => mk.Movie)
                   .HasForeignKey(mk => mk.MovieId);


        }
    }
}
