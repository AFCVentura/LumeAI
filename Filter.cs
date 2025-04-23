using CsvHelper;
using System.Globalization;
using Microsoft.ML;

namespace LumeAI
{
    class Filter
    {

        // Este método filtra os dados do arquivo original e exporta para um novo CSV.
        public String FiltrarEExportarCsv(string datasetFilePath)
        {

            // Cria o contexto do ML.NET
            var mlContext = new MLContext();

            // lista negra: palavras-chave que definem obras que devem ser removidas na filtragem
            HashSet<string> blacklistedKeywords = new HashSet<string>
            {
                "stand-up comedy", "concert", "reality show", "live performance", "concert film"
            };

            // Lista cinza: palavras-chave que devem ser ignoradas pelo k-means
            HashSet<string> greylistedKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "based on true story",
                "biography",
                "based on novel",
                "based on novel or book",
                "based on young adult novel",
                "based on children's book",
                "based on short story",
                "based on video game",
                "based on tv series",
                "based on myths",
                "based on comic",
                "based on play or musical",
                "based on memoir or autobiography",
                "woman director",
                "aftercreditsstinger",
                "duringcreditsstinger",
                "sequel"
            };

            // Carrega os dados do CSV original em memória
            var dadosOriginais = mlContext.Data.LoadFromTextFile<RawMovieData>(
                datasetFilePath,
                separatorChar: ',',
                hasHeader: true,
                allowQuoting: true, // permite campos entre aspas? (essêncial, nosso csv tem aspas)
                allowSparse: false // Não permite dados esparsos 
            );

            // Converte os dados para uma lista enumerável para aplicar filtros LINQ
            var filmes = mlContext.Data.CreateEnumerable<RawMovieData>(dadosOriginais, reuseRowObject: false);

            // Aplica os filtros: remove filmes adultos, sem poster ou palavras-chave,
            // e remove palavras-chave indesejadas (lista negra e lista cinza)
            var filmesFiltrados = filmes
                .Where(f =>
                    !f.Adult &&
                    !string.IsNullOrWhiteSpace(f.PosterPath) &&
                    !string.IsNullOrWhiteSpace(f.Keywords) &&
                    !blacklistedKeywords.Any(kw =>
                        f.Keywords.Contains(kw, StringComparison.OrdinalIgnoreCase))
                )
                .Select(f => new MovieData
                {
                    Title = f.Title,
                    Genres = f.Genres,
                    Keywords = string.Join(" ", f.Keywords.Split(' ')
                        .Where(kw => !greylistedKeywords.Contains(kw, StringComparer.OrdinalIgnoreCase))),
                    OriginalLanguage = f.OriginalLanguage,
                    ProductionCountries = f.ProductionCountries,
                    VoteAverage = f.VoteAverage,
                    VoteCount = f.VoteCount,
                    ReleaseYear = DateTime.TryParse(f.ReleaseDate, out var data) ? data.Year : 0 // Tenta converter ReleaseDate para DateTime e extrair o ano, senão define como 0
                })
                .ToList();

            // Caminho para o novo arquivo
            string filteredDatasetFilePath = "C:\\Users\\User\\source\\repos\\LumeAI\\filteredDataset.csv";
            // Exporta os filmes filtrados para um novo arquivo CSV
            using var escritor = new StreamWriter(filteredDatasetFilePath);
            using var csv = new CsvWriter(escritor, CultureInfo.InvariantCulture);
            csv.WriteRecords(filmesFiltrados);

            Console.WriteLine($"Arquivo CSV filtrado salvo em: {filteredDatasetFilePath}");
            return filteredDatasetFilePath;
        }
    }
}
