namespace LumeAI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filter = new Filter();

            // Caminho do arquivo CSV
            string datasetFilePath = "C:\\Users\\User\\source\\repos\\LumeAI\\dataset.csv";
            string filteredDatasetFilePath = filter.FiltrarEExportarCsv(datasetFilePath);
            string outputFilePath = "C:\\Users\\User\\source\\repos\\LumeAI\\outputClustersFile.md";
            string modelPath = "C:\\Users\\User\\source\\repos\\LumeAI\\modeloTreinado.zip";

            
            Clusters.GetClusters(filteredDatasetFilePath, outputFilePath, modelPath);
        }
    }
}
