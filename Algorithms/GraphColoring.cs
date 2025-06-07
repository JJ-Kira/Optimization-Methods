using OptimizationMethods.Graphs;

namespace OptimizationMethods.Algorithms
{
    /// <summary>
    /// Greedy graph coloring using approximate maximum independent set strategy.
    /// 
    /// 🎨 Goal:
    /// Assign colors to vertices such that no two adjacent vertices share the same color,
    /// using as few colors as possible.
    /// 
    /// 🔁 Approach:
    /// 1. Process isolated vertices (assign unique color 0).
    /// 2. While uncolored vertices remain:
    ///    a. Extract an approximate maximum independent set (no two connected).
    ///    b. Assign the same color to all of them.
    ///    c. Disconnect them from the graph.
    ///    d. Repeat with next color.
    /// 
    /// ⏱ Time complexity: O(V²) in worst-case due to repeated scanning of neighbors.
    /// </summary>
    public static class GraphColoring
    {
        /// <summary>
        /// Represents the result of a graph coloring process.
        /// </summary>
        public record GraphColoringResult(Dictionary<int, int> VertexColors, int NumberOfColorsUsed);

        /// <summary>
        /// Entry point for coloring a graph.
        /// Returns the color assigned to each vertex and the total number of colors used.
        /// </summary>
        public static GraphColoringResult Color(Graph graph)
        {
            const int NotColored = -1;

            // ======== CHECK FOR BIPARTITE FIRST =========
            if (graph.IsBipartite(out var leftPartition))
            {
                var bipartiteColors = graph.Vertices.Keys.ToDictionary(
                    v => v,
                    v => leftPartition.Contains(v) ? 0 : 1
                );

                return new GraphColoringResult(bipartiteColors, 2);
            }

            // ======== FALLBACK: APPROXIMATION ===========
            int colorIndex = 0;
            int totalVertices = graph.VertexCount;

            var colors = graph.Vertices.Keys.ToDictionary(v => v, _ => NotColored);
            var unprocessed = new HashSet<int>();

            // Color isolated vertices immediately
            foreach (var vertex in graph.Vertices.Values)
            {
                if (vertex.Neighbors.Count == 0)
                {
                    colors[vertex.Id] = colorIndex;
                }
                else
                {
                    unprocessed.Add(vertex.Id);
                }
            }

            if (colors.Values.Contains(colorIndex))
                colorIndex++;

            var workingGraph = CloneGraphStructure(graph);

            while (unprocessed.Count > 0)
            {
                var independentSet = FindApproximateIndependentSet(workingGraph, unprocessed);

                foreach (int v in independentSet)
                {
                    colors[v] = colorIndex;
                    DisconnectVertex(workingGraph, v);
                    unprocessed.Remove(v);
                }

                colorIndex++;
            }

            return new GraphColoringResult(colors, colorIndex);
        }

        /// <summary>
        /// Finds an approximate maximum independent set from the given candidates.
        /// Always chooses the vertex with the smallest degree.
        /// </summary>
        private static List<int> FindApproximateIndependentSet(Graph graph, HashSet<int> candidates)
        {
            var result = new List<int>();
            var remaining = new HashSet<int>(candidates);

            while (remaining.Count > 0)
            {
                // Choose vertex with fewest neighbors *within remaining*
                int best = remaining.OrderBy(v => graph.Vertices[v].Neighbors.Count(n => remaining.Contains(n))).First();
                result.Add(best);

                // Remove the vertex and all its neighbors
                remaining.Remove(best);
                foreach (var neighbor in graph.Vertices[best].Neighbors)
                    remaining.Remove(neighbor);
            }

            return result;
        }

        /// <summary>
        /// Removes all outgoing and incoming edges for the given vertex.
        /// </summary>
        private static void DisconnectVertex(Graph graph, int vertexId)
        {
            if (!graph.Vertices.ContainsKey(vertexId))
                return;

            foreach (int neighbor in graph.Vertices[vertexId].Neighbors.ToList())
            {
                graph.Vertices[neighbor].Neighbors.Remove(vertexId);
            }

            graph.Vertices[vertexId].Neighbors.Clear();
        }

        /// <summary>
        /// Creates a deep copy of the graph structure (without cloning weights).
        /// </summary>
        private static Graph CloneGraphStructure(Graph original)
        {
            var clone = new Graph(original.IsDirectedLogical);

            foreach (var v in original.Vertices.Values)
                clone.AddVertex(v.Id);

            foreach (var edge in original.Edges)
                clone.AddEdge(edge.From, edge.To, edge.Weight);

            return clone;
        }
    }
}
