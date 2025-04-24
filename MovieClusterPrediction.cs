using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeAI
{
    // Essa classe serve para agrupar os itens resultantes do treinamento da IA para gerar o relatório
    class MovieClusterPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint ClusterId { get; set; }

        public float[] Features { get; set; }
    }
}
