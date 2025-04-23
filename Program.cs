namespace LumeAI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filter = new Filter();

            // Caminho do arquivo CSV
            string datasetFilePath = "C:\\dev\\ASPNET Core\\Lume\\LumeAI\\dataset.csv";
            string filteredDatasetFilePath = "C:\\dev\\ASPNET Core\\Lume\\LumeAI\\datasetFiltered.csv";

            //filter.FiltrarEExportarCsv(datasetFilePath, filteredDatasetFilePath);

            string outputFilePath = "C:\\dev\\ASPNET Core\\Lume\\LumeAI\\outputClustersFile.md";
            string modelPath = "C:\\dev\\ASPNET Core\\Lume\\LumeAI\\modeloTreinado.zip";


            Clusters.GetClusters(filteredDatasetFilePath, outputFilePath, modelPath);
        }
    }
}
