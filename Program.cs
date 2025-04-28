using LumeAI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LumeAI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Configuração do banco de dados e do DbContext
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("CONNECTION_STRING");

            // Cria o options manualmente
            var optionsBuilder = new DbContextOptionsBuilder<LumeAIDataContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            using var db = new LumeAIDataContext(optionsBuilder.Options);
            #endregion

            // Caminho da base original
            string caminhoDatasetOriginal = "C:\\dev\\ASPNET Core\\Lume\\LumeAI\\Resources\\dataset.csv";

            // Caminho da base depois de filtrada
            string caminhoDatasetFiltrado = "C:\\dev\\ASPNET Core\\Lume\\LumeAI\\Resources\\datasetFiltrado.csv";

            // Caminho do modelo da IA
            string caminhoModeloIA = "C:\\dev\\ASPNET Core\\Lume\\LumeAI\\Resources\\modeloTreinado.zip";

            // Caminho do json para gerar relatório
            string caminhoJsonRelatorio = "c:\\dev\\ASPNET Core\\Lume\\LumeAI\\Resources\\dadosParaRelatorio.json";

            // Caminho do Relatório
            string caminhoRelatorio = "C:\\dev\\ASPNET Core\\Lume\\LumeAI\\Resources\\relatorio.md";


            // Filtrando...
            FiltersAlgorithm.FiltrarEExportarCsv(caminhoDatasetOriginal, caminhoDatasetFiltrado);

            // Treinando...
            //Clusters.GetClusters(caminhoDatasetFiltrado, caminhoJsonRelatorio, caminhoModeloIA);

            // Gerando Relatório...
            ClusterizationAI.GenerateReport(caminhoJsonRelatorio, caminhoRelatorio);
        }
    }
}
