using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeAI
{
    class Predictions
    {
        public string Title { get; set; }
        public string PosterPath { get; set; }
        public float VoteAverage { get; set; }
        public string Genres { get; set; }
        public string Keywords { get; set; }
        public int VoteCount { get; set; }
        public int ReleaseYear { get; set; }
        public uint ClusterId { get; set; }

    }
}
