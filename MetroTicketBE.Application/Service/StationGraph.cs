using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.WebAPI.Extentions
{
    public class StationGraph
    {
        private Dictionary<Guid, List<(Guid neighborId, double weight)>> graph = new();

        public StationGraph(IEnumerable<MetroLine> metroLines)
        {
            foreach (var line in metroLines)
            {
                var orderedStations = line.MetroLineStations.OrderBy(s => s.StationOder).ToList();

                for (int i = 0; i < orderedStations.Count - 1; i++)
                {
                    var current = orderedStations[i];
                    var next = orderedStations[i + 1];

                    double distance = Math.Abs(next.DistanceFromStart - current.DistanceFromStart);

                    AddEdge(current.StationId, next.StationId, distance);
                    AddEdge(next.StationId, current.StationId, distance);
                }
            }
        }

        private void AddEdge(Guid from, Guid to, double weight)
        {
            if (!graph.ContainsKey(from))
            {
                graph[from] = new();
            }

            graph[from].Add((to, weight));
        }

        public List<Guid> FindShortestPath(Guid startId, Guid endId)
        {
            var distances = new Dictionary<Guid, double>();
            var previous = new Dictionary<Guid, Guid?>();
            var queue = new PriorityQueue<Guid, double>();

            foreach (var stationId in graph.Keys)
            {
                distances[stationId] = double.PositiveInfinity;
                previous[stationId] = null;
            }

            distances[startId] = 0;
            queue.Enqueue(startId, 0);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == endId) break;

                foreach (var (neighbor, weight) in graph[current])
                {
                    double alt = distances[current] + weight;

                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = current;
                        queue.Enqueue(neighbor, alt);
                    }
                }
            }

            var path = new List<Guid>();
            var cur = endId;

            while (cur != Guid.Empty && previous[cur] is not null)
            {
                path.Insert(0, cur);
                cur = previous[cur].Value;
            }

            if (distances[endId] == double.PositiveInfinity)
            {
                return [];
            }

            path.Insert(0, startId);
            return path;
        }
    }
}
