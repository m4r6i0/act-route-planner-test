using System.Collections.Generic;

namespace RoutePlanner.Domain.Entities
{
    public class TravelPath
    {
        public List<string> Nodes { get; }
        public int Cost { get; }

        public TravelPath(List<string> nodes, int cost)
        {
            Nodes = nodes;
            Cost = cost;
        }
    }
}
