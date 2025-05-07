using LumeAI.Models.Movie;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LumeAI.Data.Mappings.Movie
{
    public class SpokenLanguageMap : IEntityTypeConfiguration<SpokenLanguage>
    {
        public void Configure(EntityTypeBuilder<SpokenLanguage> builder)
        {
            builder.HasKey(k => k.Id);

            builder.Property(k => k.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(k => k.MovieSpokenLanguages)
                   .WithOne(mk => mk.SpokenLanguage)
                   .HasForeignKey(mk => mk.SpokenLanguageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
