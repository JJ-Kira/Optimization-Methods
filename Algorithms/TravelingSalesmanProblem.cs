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
    /// Metoda podziału i ograniczeń (Branch & Bound):
    /// 1. Dzielimy przestrzeń rozwiązań na dwa podzbiory:
    ///    - zawierający dany łuk (i, j)
    ///    - nie zawierający tego łuku
    /// 2. Dla każdego podzbioru wyznaczamy dolne ograniczenie (lower bound)
    /// 3. Kontynuujemy eksplorację tylko tych podzbiorów, które mają najniższe ograniczenie
    /// 4. Powtarzamy, aż znajdziemy pełny cykl Hamiltona
    ///
    /// Pseudokod (z wykładu):
    /// 
    /// Metoda podziału i ograniczeń:
    /// P
    /// ├─ P1 (zawierający łuk)
    /// └─ P2 (niezawierający łuku)
    /// 
    /// while(istnieją podzbiory do rozwinięcia)
    ///     wybierz podzbiór z najniższym ograniczeniem LB
    ///     podziel go dalej lub zaakceptuj jeśli to cykl Hamiltona
    ///
    /// Note: Currently implemented as DFS with cost pruning (simplified B&B)
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

        public static void RunBranchAndBound(Graph graph, string? toDotPath = null)
        {
            // === Step 0: Validate input graph ===
            if (graph.IsDirected)
            {
                Console.WriteLine("TSP requires an undirected graph.");
                return;
            }

            if (graph.VertexCount < 3)
            {
                Console.WriteLine("TSP requires at least 3 vertices.");
                return;
            }

            if (!graph.IsConnected())
            {
                Console.WriteLine("TSP requires the graph to be fully connected.");
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
                bestPath.Add(start); // close the cycle
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
                        bestPath = new List<int>(current.Path);
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

        // Placeholder for MST-based solution
        public static void RunMstApproximation(Graph graph, string? toDotPath = null)
        {
            Console.WriteLine("MST-based TSP not yet implemented.");
        }

        // Placeholder for genetic algorithm
        public static void RunGenetic(Graph graph, string? toDotPath = null)
        {
            Console.WriteLine("Genetic TSP not yet implemented.");
        }
    }
}
