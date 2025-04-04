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
            string graphType = graph.IsDirected ? "digraph" : "graph";
            string connector = graph.IsDirected ? "->" : "--";

            sb.AppendLine($"{graphType} G {{");

            // Vertices
            foreach (var vertex in graph.Vertices.Values)
                sb.AppendLine($"    {vertex.Id};");

            // Edges (avoid duplicates in undirected)
            var added = new HashSet<string>();
            foreach (var edge in graph.Edges)
            {
                string key = graph.IsDirected
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
        /// Exports the graph to a .dot file for use with Graphviz.
        /// </summary>
        public static void WriteDotFile(Graph graph, string path)
        {
            File.WriteAllText(path, ToDot(graph));
            Console.WriteLine($"DOT file written to: {Path.GetFullPath(path)}");
        }

        public static void ExportWithMatching(Graph graph, Dictionary<int, int> matching, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine(graph.IsDirected ? "digraph G {" : "graph G {");
            string conn = graph.IsDirected ? "->" : "--";

            // Normalize matched edge keys to avoid duplicate undirected entries
            var matchedEdges = new HashSet<string>(
                matching
                    .Where(kv => kv.Key < kv.Value) // only one direction
                    .Select(kv => graph.IsDirected
                        ? $"{kv.Key}->{kv.Value}"
                        : string.Join("--", new[] { kv.Key, kv.Value }.OrderBy(x => x)))
            );

            // Declare nodes
            foreach (var vertex in graph.Vertices.Values)
                sb.AppendLine($"    {vertex.Id};");

            // Declare edges with deduplication
            var added = new HashSet<string>();
            foreach (var edge in graph.Edges)
            {
                string key = graph.IsDirected
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

        public static void ExportWithCycle(Graph graph, List<int> cycle, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine(graph.IsDirected ? "digraph G {" : "graph G {");
            string conn = graph.IsDirected ? "->" : "--";
            var added = new HashSet<string>();

            // Add node definitions
            foreach (var vertex in graph.Vertices.Values)
                sb.AppendLine($"    {vertex.Id};");

            // Highlight edges in traversal order with index
            for (int i = 0; i < cycle.Count - 1; i++)
            {
                int from = cycle[i];
                int to = cycle[i + 1];

                string key = graph.IsDirected
                    ? $"{from}->{to}"
                    : string.Join("--", new[] { from, to }.OrderBy(x => x));

                if (added.Contains(key)) continue;
                added.Add(key);

                sb.AppendLine($"    {from} {conn} {to} [label=\"{i}\", color=blue, penwidth=2.0];");
            }

            // Add other (non-cycle) edges if needed
            foreach (var edge in graph.Edges)
            {
                string key = graph.IsDirected
                    ? $"{edge.From}->{edge.To}"
                    : string.Join("--", new[] { edge.From, edge.To }.OrderBy(x => x));

                if (added.Contains(key))
                    continue;

                sb.AppendLine($"    {edge.From} {conn} {edge.To} [label=\"{edge.Weight}\"];");
            }

            sb.AppendLine("}");
            File.WriteAllText(path, sb.ToString());
            Console.WriteLine($"DOT output with ordered cycle saved to: {Path.GetFullPath(path)}");
        }

    }
}
