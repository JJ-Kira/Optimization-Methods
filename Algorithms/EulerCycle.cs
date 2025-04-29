using OptimizationMethods.Graphs;

namespace OptimizationMethods.Algorithms
{
    public static class EulerCycle
    {
        /// <summary>
        /// Determines whether a given undirected graph has an Eulerian cycle.
        /// 
        /// An Eulerian cycle (or Eulerian circuit) is a cycle that visits every edge of the graph exactly once,
        /// starting and ending at the same vertex. It can only exist in an undirected graph where:
        /// - The graph is connected (excluding isolated vertices).
        /// - All vertices have even degrees.
        /// 
        /// /// This method uses helper checks defined in the Graph class:
        /// - Graph.IsConnected()
        /// - Graph.AllVerticesHaveEvenDegree()
        ///
        /// This method returns true if the graph satisfies both conditions.
        /// If not, it provides an explanation via the out parameter `failureReason`.
        /// 
        /// For logically undirected graphs:
        ///   - The graph must be connected.
        ///   - All vertices must have even degrees.
        /// 
        /// For logically directed graphs:
        ///   - The graph must be strongly connected.
        ///   - For every vertex, in-degree == out-degree.
        /// </summary>
        public static bool HasEulerianCycle(Graph graph, out string? failureReason)
        {
            failureReason = null;

            if (graph.IsDirectedLogical)
            {
                if (!graph.IsStronglyConnected())
                {
                    failureReason = "The directed graph is not strongly connected.";
                    return false;
                }

                if (!graph.AllVerticesBalancedDirected())
                {
                    failureReason = "Not all vertices have equal in-degree and out-degree.";
                    return false;
                }
            }
            else
            {
                if (!graph.IsConnected())
                {
                    failureReason = "The undirected graph is not connected.";
                    return false;
                }

                if (!graph.AllVerticesHaveEvenDegree())
                {
                    failureReason = "Not all vertices have even degree — violates Eulerian cycle conditions.";
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Constructs an Eulerian cycle using Hierholzer’s Algorithm.
        /// 
        /// Algorithm Steps:
        /// 1. Start from any vertex with at least one edge.
        /// 2. Follow a trail of unused edges, removing each edge as it's used.
        /// 3. When you return to the starting vertex and the trail cannot be extended, store the cycle.
        /// 4. If any unused edges remain in the graph, start a new trail from any vertex in the cycle that has unused edges.
        /// 5. Merge this new cycle into the main cycle.
        /// 6. Repeat until all edges are used and the full Eulerian cycle is built.
        /// 
        /// /// Based on the lecture pseudocode:
        /// procedure CyklEulera(G, u)
        ///     push(STOS,u);
        ///     while STOS ≠ ∅
        ///         v := top(STOS);
        ///         if S[v] = ∅ then
        ///             pop(STOS);
        ///             LISTA ← v;
        ///         else
        ///             w := pop(S[v]);
        ///             push(STOS, w);
        ///             Usuń krawędź
        /// </summary>
        public static List<int>? FindEulerianCycle(Graph graph)
        {
            if (!HasEulerianCycle(graph, out string? reason))
            {
                Console.WriteLine("No Eulerian cycle exists in the graph.");
                if (!string.IsNullOrWhiteSpace(reason))
                    Console.WriteLine(reason);
                return null;
            }

            // Clone the adjacency list to modify during traversal (removing used edges)
            var localAdj = graph.Vertices.ToDictionary(
                v => v.Key,
                v => new List<int>(v.Value.Neighbors)
            );

            var stack = new Stack<int>();
            var cycle = new List<int>();

            // === Step 1 ===
            // Start from any vertex
            int start = graph.Vertices.Keys.First();
            stack.Push(start);

            // === Steps 2–6 ===
            // Build the Eulerian cycle
            while (stack.Count > 0)
            {
                int current = stack.Peek();

                if (localAdj[current].Count == 0)
                {
                    // No more outgoing edges — add to final cycle
                    cycle.Add(current);
                    stack.Pop();
                }
                else
                {
                    int neighbor = localAdj[current][0];
                    localAdj[current].Remove(neighbor);

                    if (!graph.IsDirectedLogical)
                        localAdj[neighbor].Remove(current); // Remove reverse edge for undirected graph

                    stack.Push(neighbor);
                }
            }

            return cycle;
        }

        /// <summary>
        /// Runs the Eulerian cycle algorithm on the given graph and prints the result.
        /// </summary>
        public static void RunAndPrint(Graph graph, string? toDotPath = null)
        {
            Console.WriteLine("Checking for Eulerian cycle...");

            var cycle = FindEulerianCycle(graph);
            if (cycle != null)
            {
                Console.WriteLine("Eulerian cycle found:");
                Console.WriteLine(string.Join(" -> ", cycle));

                if (toDotPath != null)
                    GraphPrinter.ExportWithCycle(graph, cycle, toDotPath);
            }
        }
    }
}
