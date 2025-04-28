using LumeAI.Models.Movie;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LumeAI.Data.Mappings.Movie
{
    public class MovieSpokenLanguageMap : IEntityTypeConfiguration<MovieSpokenLanguage>
    {
        public void Configure(EntityTypeBuilder<MovieSpokenLanguage> builder)
        {
            builder.HasKey(mk => new { mk.MovieId, mk.SpokenLanguageId }); // Chave composta

            builder.HasOne(mk => mk.Movie)
                   .WithMany(m => m.MovieSpokenLanguages)
                   .HasForeignKey(mk => mk.MovieId);

            builder.HasOne(mk => mk.SpokenLanguage)
                   .WithMany(k => k.MovieSpokenLanguages)
                   .HasForeignKey(mk => mk.SpokenLanguageId);
        }
    }
}
