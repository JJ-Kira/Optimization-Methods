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
        /// This method uses helper checks defined in the Graph class:
        /// - Graph.IsConnected()
        /// - Graph.AllVerticesHaveEvenDegree()
        /// </summary>
        public static bool HasEulerianCycle(Graph graph)
        {
            if (graph.IsDirected)
                throw new NotSupportedException("This implementation supports only undirected graphs.");

            return graph.IsConnected() && graph.AllVerticesHaveEvenDegree();
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
        public static List<int> FindEulerianCycle(Graph graph)
        {
            if (!HasEulerianCycle(graph))
                return null;

            // Clone the graph’s adjacency list to modify during traversal (removing used edges)
            var localAdj = graph.Vertices.ToDictionary(
                v => v.Key,
                v => new List<int>(v.Value.Neighbors)
            );

            var stack = new Stack<int>();
            var cycle = new List<int>();

            // === Step 1 ===
            // Start from any vertex and push onto stack (push(STOS,u))
            // Step 1: Start at any vertex
            int start = graph.Vertices.Keys.First();
            stack.Push(start);

            // === Step 2 ===
            // While STOS ≠ ∅
            // Steps 2–6: Build and merge cycles
            while (stack.Count > 0)
            {
                int current = stack.Peek(); // v := top(STOS);

                if (localAdj[current].Count == 0)
                {
                    // Step 3: No more unused edges — add vertex to cycle
                    // if S[v] = ∅ then
                    cycle.Add(current);  // LISTA ← v
                    stack.Pop();         // pop(STOS)

                }
                else
                {
                    // Step 2: Traverse and remove the edge
                    int neighbor = localAdj[current][0];   // w := pop(S[v])
                    localAdj[current].Remove(neighbor);
                    localAdj[neighbor].Remove(current);    // Usuń krawędź
                    stack.Push(neighbor);                  // push(STOS, w)
                }
            }

            return cycle;
        }

        /// <summary>
        /// Runs the Eulerian cycle algorithm on the given graph and prints the result.
        /// </summary>
        public static void RunAndPrint(Graph graph)
        {
            Console.WriteLine("Checking for Eulerian cycle...");

            if (!HasEulerianCycle(graph))
            {
                Console.WriteLine("No Eulerian cycle exists in the graph.");
                return;
            }

            var cycle = FindEulerianCycle(graph);
            Console.WriteLine("Eulerian cycle found:");
            Console.WriteLine(string.Join(" -> ", cycle));
        }
    }
}
