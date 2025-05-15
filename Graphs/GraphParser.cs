using System.IO;

namespace OptimizationMethods.Graphs
{
    internal class GraphParser
    {
        /// <summary>
        /// Loads a graph from a file.
        /// Supports both logically directed and undirected graphs.
        /// </summary>
        public static Graph LoadGraphFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            bool? isDirectedLogical = null;
            Graph graph = null;

            var lines = File.ReadAllLines(filePath).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();

                if (line.StartsWith("#DIGRAPH", StringComparison.OrdinalIgnoreCase))
                {
                    isDirectedLogical = bool.Parse(lines[++i].Trim());
                    graph = new Graph(isDirectedLogical.Value);
                }
                else if (line.StartsWith("#EDGES", StringComparison.OrdinalIgnoreCase))
                {
                    for (int j = i + 1; j < lines.Count; j++)
                    {
                        var edgeData = lines[j].Trim();
                        if (string.IsNullOrWhiteSpace(edgeData)) continue;

                        var parts = edgeData.Split(' ');
                        if (parts.Length < 2) continue;

                        int from = int.Parse(parts[0]);
                        int to = int.Parse(parts[1]);
                        int weight = parts.Length >= 3 ? int.Parse(parts[2]) : 1;

                        graph?.AddEdge(from, to, weight);
                    }
                    break;
                }
            }

            if (graph == null)
                throw new InvalidDataException("Graph file missing #DIGRAPH section or improperly formatted.");

            return graph;
        }

        public static (Graph graph, Dictionary<int, int> initialMatching) LoadGraphAndInitialMatching(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            bool? isDirectedLogical = null;
            Graph graph = null;
            var initialMatching = new Dictionary<int, int>();
            var matchedVertices = new HashSet<int>();

            var lines = File.ReadAllLines(filePath).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();

                if (line.StartsWith("#DIGRAPH", StringComparison.OrdinalIgnoreCase))
                {
                    isDirectedLogical = bool.Parse(lines[++i].Trim());
                    graph = new Graph(isDirectedLogical.Value);
                }
                else if (line.StartsWith("#EDGES", StringComparison.OrdinalIgnoreCase))
                {
                    for (int j = i + 1; j < lines.Count; j++)
                    {
                        var edgeData = lines[j].Trim();
                        if (string.IsNullOrWhiteSpace(edgeData)) continue;

                        var parts = edgeData.Split(' ');
                        if (parts.Length < 2) continue;

                        int from = int.Parse(parts[0]);
                        int to = int.Parse(parts[1]);
                        int weight = parts.Length >= 3 ? int.Parse(parts[2]) : 1;

                        graph?.AddEdge(from, to, weight);

                        // Add to initial matching if weight > 1 and both vertices are unmatched
                        if (weight > 1 &&
                            !matchedVertices.Contains(from) &&
                            !matchedVertices.Contains(to))
                        {
                            initialMatching[from] = to;
                            initialMatching[to] = from;
                            matchedVertices.Add(from);
                            matchedVertices.Add(to);
                        }
                    }
                    break;
                }
            }

            if (graph == null)
                throw new InvalidDataException("Graph file missing #DIGRAPH section or improperly formatted.");

            return (graph, initialMatching);
        }

    }
}
