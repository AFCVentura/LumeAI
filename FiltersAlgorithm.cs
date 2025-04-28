using CsvHelper;
using System.Globalization;
using Microsoft.ML;
using CsvHelper.Configuration;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace LumeAI
{
    public static class FiltersAlgorithm
    {

        // Este método filtra os dados do arquivo original e exporta para um novo CSV.
        public static void FiltrarEExportarCsv(string oldDatasetFilePath, string newDatasetFilePath)
        {
            // Caminho para o csv puro
            string inputPath = oldDatasetFilePath;

            // Caminho para o csv filtrado
            string outputPath = newDatasetFilePath;

            // Cria o leitor e escritor de CSVs
            using var reader = new StreamReader(inputPath);
            using var writer = new StreamWriter(outputPath);

            // Dica: Você pode minimizar a região, aí esconde esse monte de código e fica mais fácil de ler
            #region Lista Negra
            // lista negra: Palavras-chave que definem obras que devem ser removidas na filtragem
            HashSet<string> blacklistedKeywords = new HashSet<string>
            {
                "stand-up comedy", "concert", "reality show", "live performance", "concert film"
            };
            #endregion
            // Dica: Você pode minimizar a região, aí esconde esse monte de código e fica mais fácil de ler
            #region Lista Cinza
            // Lista cinza: palavras-chave que devem ser ignoradas pelo k-means (no momento estão sendo removidas)
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
                "sequel",
                // USA
                "new york city",
                "los angeles",
                "chicago",
                "louisiana",
                "florida",
                "usa",
                "illinois",
                "san francisco",
                "alabama",
                "alaska",
                "arizona",
                "arkansas",
                "california",
                "colorado",
                "connecticut",
                "delaware",
                "florida",
                "georgia",
                "hawaii",
                "idaho",
                "illinois",
                "indiana",
                "iowa",
                "kansas",
                "kentucky",
                "louisiana",
                "maine",
                "maryland",
                "massachusetts",
                "michigan",
                "minnesota",
                "mississippi",
                "missouri",
                "montana",
                "nebraska",
                "nevada",
                "new hampshire",
                "new jersey",
                "new mexico",
                "new york",
                "north carolina",
                "north dakota",
                "ohio",
                "oklahoma",
                "oregon",
                "pennsylvania",
                "rhode island",
                "south carolina",
                "south dakota",
                "tennessee",
                "texas",
                "utah",
                "vermont",
                "virginia",
                "washington",
                "west virginia",
                "wisconsin",
                "wyoming",
                // UK
                "london",
                "england",
                // Outros
                "france",
                "paris",

            };
            #endregion

            // Configura a leitura do CSV 
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
                HeaderValidated = null,
                MissingFieldFound = null
            });

            // Define a classe que mapeia os dados do CSV para a classe RawMovieData
            csv.Context.RegisterClassMap<RawMovieDataMap>();

            // Lê todos os dados do CSV e salva num Enumerable de RawMovieData
            IEnumerable<RawMovieData> allMovies = csv.GetRecords<RawMovieData>().ToList();

            // Filtra o enumerable com todos os filmes
            IEnumerable<MovieData> filteredMovies = allMovies
                .Where(m => m.VoteAverage >= 5.5f && // Tem que ter a nota acima de 5.5
                            m.VoteCount >= 150 && // Tem que ter pelo menos 150 avaliações
                            m.Adult == false && // Não pode ser filme adulto
                            m.Status == "Released" && // Tem que ter sido lançado
                            m.PosterPath is not null && // Tem que ter um poster
                            !string.IsNullOrWhiteSpace(m.Keywords) && // Não pode ter o campo keywords vazio
                            !m.Keywords
                                .Replace(", ", ",") // normaliza os separadores
                                .Split(',')
                                .Any(k => blacklistedKeywords.Contains(k.Trim()))) // Ñão pode ter alguma keyword que esteja na lista negra
                                                                                   // Tratamentos extras dos filmes
                .Select(m =>
                {
                    // Remove espaços após vírgulas para ele não tratar "Gênero" e " Gênero" diferente
                    m.Genres = m.Genres?.Replace(", ", ",");
                    m.ProductionCompanies = m.ProductionCompanies?.Replace(", ", ",");
                    m.ProductionCountries = m.ProductionCountries?.Replace(", ", ",");
                    m.SpokenLanguages = m.SpokenLanguages?.Replace(", ", ",");
                    m.Keywords = m.Keywords?.Replace(", ", ",");


                    // Remove todas as palavras-chave que estão na lista cinza
                    // OBS: Repensar se vale a pena de fato remover elas, pois talvez sejam úteis de exibir na tela depois
                    var keywords = m.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(k => k.Trim())
                                                 .Where(k => !greylistedKeywords.Contains(k));

                    // Junta as palavras chaves depois de ter separado para tirar as da lista cinza
                    m.Keywords = string.Join(",", keywords);

                    // Retorna o filme passado pela filtragem já com o novo model
                    return new MovieData
                    {
                        Id = m.Id,
                        Title = m.Title,
                        VoteAverage = m.VoteAverage,
                        VoteCount = m.VoteCount,
                        Status = m.Status,
                        ReleaseYear = DateTime.TryParse(m.ReleaseDate, out var date) ? date.Year : 0,
                        Revenue = m.Revenue,
                        Runtime = m.Runtime,
                        Adult = m.Adult,
                        BackdropPath = m.BackdropPath,
                        Budget = m.Budget,
                        Homepage = m.Homepage,
                        ImdbId = m.ImdbId,
                        OriginalLanguage = m.OriginalLanguage,
                        OriginalTitle = m.OriginalTitle,
                        Overview = m.Overview,
                        Popularity = m.Popularity,
                        PosterPath = m.PosterPath,
                        Tagline = m.Tagline,
                        Genres = m.Genres,
                        ProductionCompanies = m.ProductionCompanies,
                        ProductionCountries = m.ProductionCountries,
                        SpokenLanguages = m.SpokenLanguages,
                        Keywords = m.Keywords,
                    };
                });



            using var newCsv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = args => true,
            });

            newCsv.WriteHeader<MovieData>();
            newCsv.NextRecord();
            newCsv.WriteRecords(filteredMovies);

            Console.WriteLine($"Arquivo CSV filtrado salvo em: {outputPath}");
        }
    }
}
