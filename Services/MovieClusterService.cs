using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LumeAI.DTOs;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace LumeAI.Services
{
    class MovieClusterService
    {
        public static void GetClusters(string datasetPath, string outputReportDataPath, string outputDataPath, string modelPath)
        {
            Console.WriteLine("Iniciando configuração do treinamento.");
            // Inicializa o contexto, seed serve para os resultados serem reprodutíveis sempre, sempre são os mesmos
            var mlContext = new MLContext(seed: 1);

            // Carregar os dados do csv para uma estrutura chamada dataView
            // Note que é passado um tipo MovieData, que é uma classe que representa a estrutura dos dados
            var dataView = mlContext.Data.LoadFromTextFile<MovieData>(
                path: datasetPath, // caminho do csv
                hasHeader: true, // tem cabeçalho no csv?
                separatorChar: ',', // separador de colunas
                allowQuoting: true, // permite campos entre aspas? (essêncial, nosso csv tem aspas)
                allowSparse: false // Não permite dados esparsos 
                );

            // Pipeline de conversão dos dados brutos em dados vetorizados, o tipo de dado que a IA entende
            // Essa parte vai ter vários Appends, que são como se fossem "passos" de transformação
            // Aqui, a gente vai transformar cada coluna relevante em um vetor de números para a IA entender
            var pipeline = mlContext.Transforms

                // 1. Etapas textuais
                // 1.1 Transforma os gêneros em texto vetorizado
                .Text.FeaturizeText("GenreFeatures", nameof(MovieData.Genres))
                // 1.2 Transforma as palavras-chaves em texto vetorizado
                .Append(mlContext.Transforms.Text.FeaturizeText("KeywordFeatures", nameof(MovieData.Keywords)))

                // 2. Aplicação de peso para as etapas textuais ( 3 de gêneros para 2 de palavras-chave)
                // 2.1 Aplicação de peso nos gêneros
                .Append(mlContext.Transforms.Concatenate("WeightedGenreFeatures",
                    "GenreFeatures", "GenreFeatures", "GenreFeatures"))
                // 2.2 Aplicação de peso nas palavras-chave
                .Append(mlContext.Transforms.Concatenate("WeightedKeywordFeatures",
                    "KeywordFeatures", "KeywordFeatures"))

                // 3. Junção de todos os dados vetorizados em um só vetor final chamado "Features"
                .Append(mlContext.Transforms.Concatenate("Features",
                    "WeightedGenreFeatures", "WeightedKeywordFeatures"));


            // Aplica a transformação anterior no DataView
            var transformedData = pipeline.Fit(dataView).Transform(dataView);

            // Configurando as variáveis do treinamento via K-means
            var options = new KMeansTrainer.Options
            {
                FeatureColumnName = "Features", // Nome da coluna que vai ser usada, resultante do pipeline
                NumberOfClusters = 90, // Número de clusters desejados, fórmula: x ~= sqrt(n/2)
                MaximumNumberOfIterations = 150, // Número máximo de iterações, ou seja, a quantidade de vezes que o K-means vai tentar ajustar os centróides dos clusters
            };

            // Instanciamos o treinador da IA, passando as opções
            var trainer = mlContext.Clustering.Trainers.KMeans(options);

            Console.WriteLine("Iniciando treinamento.");

            // Mandamos o treinador treinar o modelo, passando os dados transformados
            var model = trainer.Fit(transformedData);

            // Aplica o modelo treinado nos dados transformados, em essência, atribui um ClusterId para cada filme
            var predictionsDataView = model.Transform(transformedData);

            Console.WriteLine("Treinamento Concluido. Iniciando definição dos centróides");

            var kMeansModel = model as ClusteringPredictionTransformer<KMeansModelParameters>;
            var kMeans = kMeansModel?.Model;

            // Cria array de centróides (inicialmente null, será preenchido pelo método)
            VBuffer<float>[] centroids = null!;
            int numberOfClusters;

            // Chama o método corretamente
            kMeans.GetClusterCentroids(ref centroids, out numberOfClusters);

            if (centroids == null)
                throw new InvalidOperationException("Não foi possível obter os centróides do modelo.");

            // Salva o modelo treinado em um arquivo .zip
            mlContext.Model.Save(model, transformedData.Schema, modelPath);
            Console.WriteLine("Definição dos centróides concluida.");
            Console.WriteLine($"Modelo Salvo em {modelPath}");

            Console.WriteLine("Iniciando mapeamento dos dados para JSON do relatório");

            // Cria enumerable a partir do DataView
            var filteredEnumerable = mlContext.Data.CreateEnumerable<MovieData>(dataView, reuseRowObject: false);

            #region Json para o relatório
            // Cria uma lista de itens que é uma mistura de alguns dados dos filmes (apenas os relevantes pro relatório) e o ClusterId desses filmes
            var predictionList = mlContext.Data.CreateEnumerable<MovieClusterPrediction>(predictionsDataView, reuseRowObject: false)
            .Zip(filteredEnumerable, (prediction, movie) => new Predictions
            {
                Title = movie.Title,
                PosterPath = movie.PosterPath,
                VoteAverage = movie.VoteAverage,
                Genres = movie.Genres,
                Keywords = movie.Keywords,
                VoteCount = movie.VoteCount,
                ReleaseYear = movie.ReleaseYear,
                ClusterId = prediction.ClusterId
            })
            .ToList();

            // Salva os dados dos filmes em JSON
            var json = JsonSerializer.Serialize(predictionList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputReportDataPath, json);
            #endregion

            Console.WriteLine("Iniciando mapeamento dos dados para JSON completo");
            #region Json com todos os dados (backup e salvar no banco de dados)
            Console.WriteLine("Mapeando centróides");
            var centroidList = centroids
                .Select((v, i) => new ClusterExportJson
                {
                    Id = i,
                    Centroid = v.DenseValues().ToArray()
                })
                .ToList();

            Console.WriteLine("Mapeando filmes");
            var fullPredictionList = mlContext.Data.CreateEnumerable<MovieClusterPrediction>(predictionsDataView, reuseRowObject: false)
                .Zip(filteredEnumerable, (prediction, movie) => new MovieExportJson
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    PosterPath = movie.PosterPath,
                    VoteAverage = movie.VoteAverage,
                    VoteCount = movie.VoteCount,
                    Status = movie.Status,
                    ReleaseYear = movie.ReleaseYear,
                    Revenue = movie.Revenue,
                    Runtime = movie.Runtime,
                    Adult = movie.Adult,
                    BackdropPath = movie.BackdropPath,
                    Budget = movie.Budget,
                    Homepage = movie.Homepage,
                    ImdbId = movie.ImdbId,
                    OriginalLanguage = movie.OriginalLanguage,
                    OriginalTitle = movie.OriginalTitle,
                    Overview = movie.Overview,
                    Popularity = movie.Popularity,
                    Tagline = movie.Tagline,
                    Genres = movie.Genres?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList() ?? new List<string>(),
                    ProductionCompanies = movie.ProductionCompanies?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(pc => pc.Trim()).ToList() ?? new List<string>(),
                    ProductionCountries = movie.ProductionCountries?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(pc => pc.Trim()).ToList() ?? new List<string>(),
                    SpokenLanguages = movie.SpokenLanguages?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(sl => sl.Trim()).ToList() ?? new List<string>(),
                    Keywords = movie.Keywords?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(k => k.Trim()).ToList() ?? new List<string>(),
                    ClusterId = prediction.ClusterId,
                })
                .ToList();

            Console.WriteLine("Unindo filmes e centróides");
            var finalExport = new
            {
                Centroids = centroidList,
                Movies = fullPredictionList
            };

            Console.WriteLine("Salvando no JSON completo");

            // Salva em JSON
            var fullJson = JsonSerializer.Serialize(finalExport, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputDataPath, fullJson);
            #endregion
            Console.WriteLine("Etapa de clusterização concluida");
        }

        public static void GenerateReport(string inputDataPath, string outputReportPath)
        {
            Console.WriteLine("Iniciando etapa de geração de relatório");
            Console.WriteLine("Desserealizando JSON");
            // Acessa o JSON e deserializa
            var json = File.ReadAllText(inputDataPath);
            var clusteredMovies = JsonSerializer.Deserialize<List<Predictions>>(json);
            var groupedClusters = clusteredMovies.GroupBy(m => m.ClusterId);


            // Imprime os clusters e a quantidade de filmes no console
            Console.WriteLine();
            foreach (var cluster in groupedClusters.OrderBy(c => c.Key))
            {
                Console.WriteLine($"Cluster {cluster.Key}: {cluster.ToList().Count}");
            }

            Console.WriteLine("Imprimindo clusters no console:");

            // Cria o caminho do relatório
            var clusterReportPath = outputReportPath;


            Console.WriteLine("Gerando relatório...");
            // Começa a escrever o relatório em markdown
            using (var writer = new StreamWriter(clusterReportPath, false))
            {

                IDictionary<uint, int> biggestCLuster;
                IDictionary<uint, int> smallestCLuster;
                IDictionary<uint, int> bestCLuster;
                IDictionary<uint, int> worstCLuster;

                IDictionary<string, int> amountOfClustersWithEachGenre = new Dictionary<string, int>
                {
                    { "drama", 0 },
                    { "documentary", 0 },
                    { "comedy", 0 },
                    { "animation", 0 },
                    { "horror", 0 },
                    { "romance", 0 },
                    { "music", 0 },
                    { "thriller", 0 },
                    { "action", 0 },
                    { "crime", 0 },
                    { "family", 0 },
                    { "tv movie", 0 },
                    { "adventure", 0 },
                    { "fantasy", 0 },
                    { "science fiction", 0 },
                    { "mystery", 0 },
                    { "history", 0 },
                    { "war", 0 },
                    { "western", 0 }
                };

                foreach (var cluster in groupedClusters.OrderBy(c => c.Key))
                {
                    var filmes = cluster.ToList();
                    var total = filmes.Count;


                    // Gêneros mais comuns
                    var topGeneros = filmes
                    .SelectMany(f => f.Genres.Split(',')) // troque para f.Genres se você tiver esse campo no objeto
                    .GroupBy(g => g)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key.ToLower());

                    foreach (var genre in topGeneros)
                    {
                        if (amountOfClustersWithEachGenre.ContainsKey(genre))
                        {
                            amountOfClustersWithEachGenre[genre]++;
                        }
                    }

                    var topKeywords = filmes
                        .SelectMany(f => f.Keywords.Split(','))
                        .GroupBy(k => k)
                        .OrderByDescending(k => k.Count())
                        .Take(15)
                        .Select(k => $"{k.Key} ({k.Count()})");

                }
                writer.WriteLine("Gêneros mais comuns (número de aparições nos clusters)\r\n");

                foreach (var genre in amountOfClustersWithEachGenre.OrderByDescending(g => g.Value))
                {
                    writer.WriteLine($"* {genre.Key}: {genre.Value}");
                }

                writer.WriteLine("\nVersão resumida dos clusters");
                foreach (var cluster in groupedClusters.OrderBy(c => c.Key))
                {
                    var filmes = cluster.ToList();
                    var total = filmes.Count;

                    // Gêneros mais comuns
                    var topGeneros = filmes
                        .SelectMany(f => f.Genres.Split(',')) // troque para f.Genres se você tiver esse campo no objeto
                        .GroupBy(g => g)
                        .Where(g => g.Count() > total / 5)
                        .OrderByDescending(g => g.Count())
                        .Select(g => $"{g.Key} ({g.Count() * 100 / total}%)");

                    var topKeywords = filmes
                        .SelectMany(f => f.Keywords.Split(','))
                        .GroupBy(k => k)
                        .Where(k => k.Count() > total / 5)
                        .OrderByDescending(k => k.Count())
                        .Select(k => $"{k.Key} ({k.Count() * 100 / total}%)");

                    writer.WriteLine($"* Cluster {cluster.Key}. Total: {total}");
                    writer.WriteLine($"\t* {string.Join(", ", topGeneros)}");
                    writer.WriteLine($"\t* {(topKeywords.Count() != 0 ? string.Join(", ", topKeywords) : "nenhuma palavra-chave relevante")} \n");
                }
                foreach (var cluster in groupedClusters.OrderBy(c => c.Key))
                {
                    var filmes = cluster.ToList();
                    var total = filmes.Count;

                    // Gêneros mais comuns
                    var topGeneros = filmes
                        .SelectMany(f => f.Genres.Split(',')) // troque para f.Genres se você tiver esse campo no objeto
                        .GroupBy(g => g)
                        .OrderByDescending(g => g.Count())
                        .Select(g => $"{g.Key} ({g.Count()})");

                    var topKeywords = filmes
                        .SelectMany(f => f.Keywords.Split(','))
                        .GroupBy(k => k)
                        .OrderByDescending(k => k.Count())
                        .Take(15)
                        .Select(k => $"{k.Key} ({k.Count()})");

                    // Médias
                    var mediaAno = Convert.ToInt32(filmes.Average(f => f.ReleaseYear));
                    var mediaNota = filmes.Average(f => f.VoteAverage);
                    var mediaVotos = Convert.ToInt32(filmes.Average(f => f.VoteCount));

                    // Maiores
                    var menoresAnos = filmes.OrderBy(f => f.ReleaseYear).Take(5);
                    var maioresAnos = filmes.OrderByDescending(f => f.ReleaseYear).Take(5);
                    var menoresNotas = filmes.OrderBy(f => f.VoteAverage).Take(5);
                    var maioresNotas = filmes.OrderByDescending(f => f.VoteAverage).Take(5);
                    var menoresVotos = filmes.OrderBy(f => f.VoteCount).Take(5);
                    var maioresVotos = filmes.OrderByDescending(f => f.VoteCount).Take(5);



                    writer.WriteLine($"# Cluster {cluster.Key}");

                    writer.WriteLine($"**Total de filmes:** {total}");

                    writer.WriteLine($"**Média de ano de lançamento:** {mediaAno}");
                    writer.WriteLine($"**Menor ano:** {menoresAnos?.First().ReleaseYear} ({menoresAnos?.First().Title})");
                    writer.WriteLine($"**Maior ano:** {maioresAnos?.First().ReleaseYear} ({maioresAnos?.First().Title})");

                    writer.WriteLine("\n## 5 Mais Velhos \n");
                    writer.WriteLine(string.Join(" | ", menoresAnos.Select(f => $"**{f.Title}**")));
                    writer.WriteLine(" -- | -- | -- | -- | -- ");
                    writer.WriteLine(string.Join(" | ", menoresAnos.Select(f => $"*{f.ReleaseYear}*")));
                    writer.WriteLine(string.Join(" | ", menoresAnos.Select(f => $" ![](https://image.tmdb.org/t/p/w200{f.PosterPath})")));
                    writer.Write("\n");

                    writer.WriteLine("## 5 mais novos\n");
                    writer.WriteLine(string.Join(" | ", maioresAnos.Select(f => $"**{f.Title}**")));
                    writer.WriteLine(" -- | -- | -- | -- | -- ");
                    writer.WriteLine(string.Join(" | ", maioresAnos.Select(f => $"*{f.ReleaseYear}*")));
                    writer.WriteLine(string.Join(" | ", maioresAnos.Select(f => $"![](https://image.tmdb.org/t/p/w200{f.PosterPath})")));
                    writer.Write("\n");

                    writer.WriteLine($"**Média da nota:** {mediaNota:F2}");
                    writer.WriteLine($"**Menor nota:** {menoresNotas?.First().VoteAverage:F2} ({menoresNotas?.First().Title})");
                    writer.WriteLine($"**Maior nota:** {maioresNotas?.First().VoteAverage:F2} ({maioresNotas?.First().Title})");

                    writer.WriteLine($"\n## 5 mais bem avaliados\n");
                    writer.WriteLine(string.Join(" | ", maioresNotas.Select(f => $"**{f.Title}**")));
                    writer.WriteLine(" -- | -- | -- | -- | -- ");
                    writer.WriteLine(string.Join(" | ", maioresNotas.Select(f => $"*{f.VoteAverage:F2}*")));
                    writer.WriteLine(string.Join(" | ", maioresNotas.Select(f => $"![](https://image.tmdb.org/t/p/w200{f.PosterPath})")));
                    writer.Write("\n");

                    writer.WriteLine($"## 5 menos bem avaliados\n");
                    writer.WriteLine(string.Join(" | ", menoresNotas.Select(f => $"**{f.Title}**")));
                    writer.WriteLine(" -- | -- | -- | -- | -- ");
                    writer.WriteLine(string.Join(" | ", menoresNotas.Select(f => $"*{f.VoteAverage:F2}*")));
                    writer.WriteLine(string.Join(" | ", menoresNotas.Select(f => $"![](https://image.tmdb.org/t/p/w200{f.PosterPath})")));
                    writer.Write("\n");

                    writer.WriteLine($"**Média de votos:** {mediaVotos:F1}");
                    writer.WriteLine($"**Menor votos:** {menoresVotos?.First().VoteCount} ({menoresVotos?.First().Title})");
                    writer.WriteLine($"**Maior votos:** {maioresVotos?.First().VoteCount} ({maioresVotos?.First().Title})");

                    writer.WriteLine($"\n## 5 mais avaliados\n");
                    writer.WriteLine(string.Join(" | ", maioresVotos.Select(f => $"**{f.Title}**")));
                    writer.WriteLine(" -- | -- | -- | -- | -- ");
                    writer.WriteLine(string.Join(" | ", maioresVotos.Select(f => $"*{f.VoteCount}*")));
                    writer.WriteLine(string.Join(" | ", maioresVotos.Select(f => $"![](https://image.tmdb.org/t/p/w200{f.PosterPath})")));
                    writer.Write("\n");

                    writer.WriteLine($"## 5 menos avaliados\n");
                    writer.WriteLine(string.Join(" | ", menoresVotos.Select(f => $"**{f.Title}**")));
                    writer.WriteLine(" -- | -- | -- | -- | -- ");
                    writer.WriteLine(string.Join(" | ", menoresVotos.Select(f => $"*{f.VoteCount}*")));
                    writer.WriteLine(string.Join(" | ", menoresVotos.Select(f => $"![](https://image.tmdb.org/t/p/w200{f.PosterPath})")));
                    writer.Write("\n");


                    writer.WriteLine("**Gêneros mais comuns:**");
                    foreach (var genre in topGeneros)
                    {
                        writer.WriteLine($"* {genre}");
                    }

                    writer.WriteLine("\n\n**Palavras-chave mais comuns:**");
                    foreach (var keyword in topKeywords)
                    {
                        writer.WriteLine($"* {keyword}");
                    }

                    writer.WriteLine();

                }
                Console.WriteLine($"Relatório concluido em {outputReportPath}");
            }
        }
    }
}
