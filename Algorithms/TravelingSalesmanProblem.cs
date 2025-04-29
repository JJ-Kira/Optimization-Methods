using OptimizationMethods.Graphs;

namespace OptimizationMethods.Algorithms
{
    /// <summary>
    /// Traveling Salesman Problem (TSP)
    ///
    /// Problem definition:
    /// Komiwojażer ma odwiedzić dokładnie raz każdą z n wybranych miejscowości
    /// i powrócić do miejscowości z której rozpoczął swoją podróż.
    /// Problem polega na znalezieniu najkrótszego cyklu Hamiltona w grafie pełnym.
    ///
    /// To taki cykl w grafie, który przechodzi przez każdy wierzchołek dokładnie raz 
    /// i wraca do punktu początkowego.
    /// 
    /// Metoda podziału i ograniczeń (Branch & Bound):
    /// 1. Dzielimy przestrzeń rozwiązań na dwa podzbiory:
    ///    - zawierający dany łuk (i, j)
    ///    - nie zawierający tego łuku
    /// 2. Dla każdego podzbioru wyznaczamy dolne ograniczenie (lower bound)
    /// 3. Kontynuujemy eksplorację tylko tych podzbiorów, które mają najniższe ograniczenie
    /// 4. Powtarzamy, aż znajdziemy pełny cykl Hamiltona
    ///
    /// Note: Implemented as DFS with cost pruning (simplified Branch and Bound).
    /// </summary>
    public static class TravelingSalesmanProblem
    {
        private class State
        {
            public List<int> Path { get; set; }
            public int Cost { get; set; }
            public HashSet<int> Visited { get; set; }

            public State(List<int> path, int cost, HashSet<int> visited)
            {
                Path = path;
                Cost = cost;
                Visited = visited;
            }

            public int Last => Path.Last();
        }

        private static int bestCost = int.MaxValue;
        private static List<int>? bestPath = null;

        /// <summary>
        /// Runs TSP Branch & Bound method.
        /// Supports both directed and undirected graphs.
        /// </summary>
        public static void RunBranchAndBound(Graph graph, string? toDotPath = null)
        {
            // === Step 0: Validate input graph ===
            if (graph.VertexCount < 3)
            {
                Console.WriteLine("TSP requires at least 3 vertices.");
                return;
            }

            if (!graph.IsConnected())
            {
                Console.WriteLine("TSP requires the graph to be connected.");
                return;
            }

            if (!graph.AllEdgesPositive())
            {
                Console.WriteLine("TSP requires all edge weights to be positive.");
                return;
            }

            // === Step 1: Start search ===
            Console.WriteLine("Running Branch and Bound for TSP...");
            bestCost = int.MaxValue;
            bestPath = null;

            int start = graph.Vertices.Keys.First();
            var initial = new State(
                new List<int> { start },
                0,
                new HashSet<int> { start }
            );

            Search(graph, initial);

            // === Step 5: Output result ===
            if (bestPath != null)
            {
                Console.WriteLine($"Optimal TSP path: {string.Join(" -> ", bestPath)}");
                Console.WriteLine($"Total cost: {bestCost}");

                if (toDotPath != null)
                    GraphPrinter.ExportWithCycle(graph, bestPath, toDotPath);
            }
            else
            {
                Console.WriteLine("No valid TSP cycle found.");
            }
        }

        /// <summary>
        /// Recursive DFS + pruning for Branch and Bound search.
        /// </summary>
        private static void Search(Graph graph, State current)
        {
            // === Step 2: Base case — complete tour ===
            if (current.Visited.Count == graph.VertexCount)
            {
                if (graph.Vertices[current.Last].Neighbors.Contains(current.Path[0]))
                {
                    int totalCost = current.Cost + graph.GetEdgeWeight(current.Last, current.Path[0]);
                    if (totalCost < bestCost)
                    {
                        bestCost = totalCost;
                        bestPath = new List<int>(current.Path) { current.Path[0] }; // close cycle when saving
                    }
                }
                return;
            }

            // === Step 3: Expand current state to neighbors ===
            foreach (int neighbor in graph.Vertices[current.Last].Neighbors)
            {
                if (current.Visited.Contains(neighbor))
                    continue;

                int nextCost = current.Cost + graph.GetEdgeWeight(current.Last, neighbor);
                if (nextCost >= bestCost)
                    continue; // prune this path

                var nextVisited = new HashSet<int>(current.Visited) { neighbor };
                var nextPath = new List<int>(current.Path) { neighbor };
                var nextState = new State(nextPath, nextCost, nextVisited);
                Search(graph, nextState);
            }
        }

        /// <summary>
        /// Placeholder for MST-based TSP approximation (not implemented).
        /// </summary>
        public static void RunMstApproximation(Graph graph, string? toDotPath = null)
        {
            Console.WriteLine("MST-based TSP not yet implemented.");
        }

        /// <summary>
        /// Placeholder for genetic TSP algorithm (not implemented).
        /// </summary>
        public static void RunGenetic(Graph graph, string? toDotPath = null)
        {
            Console.WriteLine("Genetic TSP not yet implemented.");
        }
    }
}
