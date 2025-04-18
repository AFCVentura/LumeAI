namespace LumeAI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Caminho do arquivo CSV
            string datasetFilePath = "C:\\dev\\ASPNET Core\\Lume\\LumeAI\\dataset.csv";
            string outputFilePath = "C:\\dev\\ASPNET Core\\Lume\\LumeAI\\outputClustersFile.md";

            Clusters.GetClusters(datasetFilePath, outputFilePath);
        }
    }
}
