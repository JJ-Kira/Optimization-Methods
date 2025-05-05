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
        /// Approximates a solution to the Traveling Salesman Problem (TSP) using the MST-based heuristic.
        /// 
        /// 📚 Algorithm based on lecture material:
        /// 1. Construct a Minimum Spanning Tree (MST) of the input graph.
        /// 2. Perform a preorder traversal (DFS) of the MST starting from an arbitrary vertex.
        /// 3. Traverse the graph in the preorder order, skipping already-visited vertices (shortcutting).
        /// 4. Return to the starting vertex to complete the cycle.
        /// 
        /// 💡 This algorithm works on undirected graphs and guarantees an approximation within 2×OPT 
        ///     if the graph satisfies the triangle inequality.
        /// 📘 Minimalne Drzewo Rozpinające (MST)
        /// MST to podzbiór krawędzi grafu nieskierowanego, który:
        /// - łączy wszystkie wierzchołki grafu (tworzy spójny podgraf),
        /// - nie zawiera cykli (jest drzewem),
        /// - ma możliwie najmniejszą sumaryczną wagę.
        ///
        /// MST składa się zawsze z dokładnie (V - 1) krawędzi, gdzie V to liczba wierzchołków.
        /// W grafie z unikalnymi wagami krawędzi MST jest jednoznaczne.

        /// </summary>
        public static void RunMstApproximation(Graph graph, string? toDotPath = null)
        {
            // === Step 0: Validation ===
            if (graph.VertexCount < 3)
            {
                Console.WriteLine("TSP approximation requires at least 3 vertices.");
                return;
            }

            if (!graph.IsConnected())
            {
                Console.WriteLine("Graph must be connected.");
                return;
            }

            if (!graph.AllEdgesPositive())
            {
                Console.WriteLine("All edge weights must be positive.");
                return;
            }

            Console.WriteLine("Running MST-based approximation for TSP...");

            // === Step 1: Build Minimum Spanning Tree (MST) ===
            // This gives us a tree that connects all nodes with minimal total weight.
            var mstEdges = graph.MinimumSpanningTree();

            // Build a new graph from the MST edges
            var mstGraph = new Graph(isDirectedLogical: false);
            foreach (var vertex in graph.Vertices.Values)
                mstGraph.AddVertex(vertex.Id);
            foreach (var edge in mstEdges)
                mstGraph.AddEdge(edge.From, edge.To, edge.Weight);

            // === Step 2: Preorder traversal of MST (DFS) ===
            // This simulates a walk around the tree that visits each node.
            var start = mstGraph.Vertices.Keys.First();
            var preorder = new List<int>();
            var visited = new HashSet<int>();

            void Dfs(int v)
            {
                preorder.Add(v);
                visited.Add(v);

                foreach (var neighbor in mstGraph.Vertices[v].Neighbors)
                {
                    if (!visited.Contains(neighbor))
                        Dfs(neighbor);
                }
            }

            Dfs(start); // start DFS from any vertex

            // === Step 3: Shortcutting — skip repeated vertices ===
            // This builds a Hamiltonian path visiting all nodes once.
            var tspPath = preorder.Distinct().ToList();
            tspPath.Add(start); // return to start to form a cycle

            // === Step 4: Compute total cost ===
            int totalCost = 0;
            for (int i = 0; i < tspPath.Count - 1; i++)
            {
                totalCost += graph.GetEdgeWeight(tspPath[i], tspPath[i + 1]);
            }

            // === Step 5: Output ===
            Console.WriteLine($"Approximate TSP path: {string.Join(" -> ", tspPath)}");
            Console.WriteLine($"Total cost: {totalCost}");

            if (toDotPath != null)
                GraphPrinter.ExportWithCycle(graph, tspPath, toDotPath);
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
