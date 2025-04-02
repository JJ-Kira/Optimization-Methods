using System.IO;

namespace OptimizationMethods.Graphs
{
    internal class GraphParser
    {
        public static Graph LoadGraphFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            bool? isDirected = null;
            var graph = (Graph)null;

            var lines = File.ReadAllLines(filePath).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();

                if (line.StartsWith("#DIGRAPH", StringComparison.OrdinalIgnoreCase))
                {
                    isDirected = bool.Parse(lines[++i].Trim());
                    graph = new Graph(isDirected.Value);
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
                throw new InvalidDataException("Graph file missing #DIGRAPH or improperly formatted.");

            return graph;
        }
    }
}
