namespace LumeAI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Caminho do arquivo CSV
            string datasetFilePath = "C:\\Users\\User\\source\\repos\\LumeAI\\dataset.csv";
            string outputFilePath = "C:\\Users\\User\\source\\repos\\LumeAI\\outputClustersFile.md";
            string modelPath = "C:\\Users\\User\\source\\repos\\LumeAI\\modeloTreinado.zip";

            Clusters.GetClusters(datasetFilePath, outputFilePath, modelPath);
        }
    }
}
