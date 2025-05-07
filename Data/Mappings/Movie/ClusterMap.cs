using LumeAI.Models.Movie;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeAI.Data.Mappings.Movie
{
    public class ClusterMap : IEntityTypeConfiguration<Cluster>
    {
        public void Configure(EntityTypeBuilder<Cluster> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CentroidVectorJson)
                   .HasColumnType("json");

            builder.HasMany(c => c.Movies)
              .WithOne(m => m.Cluster)
              .HasForeignKey(m => m.ClusterId)
              .OnDelete(DeleteBehavior.Cascade); // se quiser deletar os filmes ao deletar o cluster
        }
    }
}
