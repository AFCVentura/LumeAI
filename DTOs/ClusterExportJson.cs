using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeAI.DTOs
{
    class ClusterExportJson
    {
        public int Id { get; set; }
        public float[] Centroid { get; set; } = [];
    }
}
