using System.Text;

namespace OptimizationMethods.Graphs
{
    public static class GraphPrinter
    {
        /// <summary>
        /// Exports the current graph to DOT (Graphviz) format.
        /// </summary>
        public static string ToDot(Graph graph)
        {
            var sb = new StringBuilder();
            string graphType = graph.IsDirectedLogical ? "digraph" : "graph";
            string connector = graph.IsDirectedLogical ? "->" : "--";

            sb.AppendLine($"{graphType} G {{");

            // Vertices
            foreach (var vertex in graph.Vertices.Values)
                sb.AppendLine($"    {vertex.Id};");

            // Edges (deduplicate if logically undirected)
            var added = new HashSet<string>();
            foreach (var edge in graph.Edges)
            {
                string key = graph.IsDirectedLogical
                    ? $"{edge.From}->{edge.To}"
                    : string.Join("--", new[] { edge.From, edge.To }.OrderBy(x => x));

                if (!added.Contains(key))
                {
                    added.Add(key);
                    sb.AppendLine($"    {edge.From} {connector} {edge.To} [label=\"{edge.Weight}\"];");
                }
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// Exports the graph to a DOT file for Graphviz visualization.
        /// </summary>
        public static void WriteDotFile(Graph graph, string path)
        {
            File.WriteAllText(path, ToDot(graph));
            Console.WriteLine($"DOT file written to: {Path.GetFullPath(path)}");
        }

        /// <summary>
        /// Exports the graph highlighting a matching.
        /// </summary>
        public static void ExportWithMatching(Graph graph, Dictionary<int, int> matching, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine(graph.IsDirectedLogical ? "digraph G {" : "graph G {");
            string conn = graph.IsDirectedLogical ? "->" : "--";

            var matchedEdges = new HashSet<string>(
                matching
                    .Where(kv => kv.Key < kv.Value)
                    .Select(kv => graph.IsDirectedLogical
                        ? $"{kv.Key}->{kv.Value}"
                        : string.Join("--", new[] { kv.Key, kv.Value }.OrderBy(x => x)))
            );

            foreach (var vertex in graph.Vertices.Values)
                sb.AppendLine($"    {vertex.Id};");

            var added = new HashSet<string>();
            foreach (var edge in graph.Edges)
            {
                string key = graph.IsDirectedLogical
                    ? $"{edge.From}->{edge.To}"
                    : string.Join("--", new[] { edge.From, edge.To }.OrderBy(x => x));

                if (added.Contains(key)) continue;
                added.Add(key);

                string label = $"[label=\"{edge.Weight}\"";
                if (matchedEdges.Contains(key))
                    label += ", color=red, penwidth=2.0";
                label += "]";

                sb.AppendLine($"    {edge.From} {conn} {edge.To} {label};");
            }

            sb.AppendLine("}");
            File.WriteAllText(path, sb.ToString());
            Console.WriteLine($"DOT file written to: {Path.GetFullPath(path)}");
        }

        /// <summary>
        /// Exports the graph highlighting a cycle (e.g., Eulerian cycle).
        /// </summary>
        public static void ExportWithCycle(Graph graph, List<int> cycle, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine(graph.IsDirectedLogical ? "digraph G {" : "graph G {");
            string conn = graph.IsDirectedLogical ? "->" : "--";

            var added = new HashSet<string>();

            foreach (var vertex in graph.Vertices.Values)
                sb.AppendLine($"    {vertex.Id};");

            // Highlight cycle edges
            for (int i = 0; i < cycle.Count - 1; i++)
            {
                int from = cycle[i];
                int to = cycle[i + 1];

                string key = graph.IsDirectedLogical
                    ? $"{from}->{to}"
                    : string.Join("--", new[] { from, to }.OrderBy(x => x));

                if (added.Contains(key)) continue;
                added.Add(key);

                sb.AppendLine($"    {from} {conn} {to} [label=\"{i}\", color=blue, penwidth=2.0];");
            }

            // Remaining non-cycle edges
            foreach (var edge in graph.Edges)
            {
                string key = graph.IsDirectedLogical
                    ? $"{edge.From}->{edge.To}"
                    : string.Join("--", new[] { edge.From, edge.To }.OrderBy(x => x));

                if (added.Contains(key)) continue;
                added.Add(key);

                sb.AppendLine($"    {edge.From} {conn} {edge.To} [label=\"{edge.Weight}\"];");
            }

            sb.AppendLine("}");
            File.WriteAllText(path, sb.ToString());
            Console.WriteLine($"DOT output with cycle saved to: {Path.GetFullPath(path)}");
        }
    }
}
