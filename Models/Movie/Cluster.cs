using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace LumeAI.Models.Movie
{
    public class Cluster
    {
        public int Id { get; set; }
        public ICollection<Movie> Movies { get; set; }
        // Mapeamento para o banco
        public string CentroidVectorJson { get; set; }

        // Propriedade não mapeada para facilitar o uso no código
        [NotMapped]
        public float[] CentroidVector
        {
            get => JsonSerializer.Deserialize<float[]>(CentroidVectorJson);
            set => CentroidVectorJson = JsonSerializer.Serialize(value);
        }

        public Cluster()
        {
            Movies = new List<Movie>();
        }
    }
}
