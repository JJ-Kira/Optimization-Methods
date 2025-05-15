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
        ///TODO: Check why not better results
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
            var start = mstGraph.Vertices.Keys.Last();
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
        /// Approximates a solution to the Traveling Salesman Problem (TSP) using a Genetic Algorithm.
        /// 
        /// 📚 Genetic Algorithm workflow:
        /// 1. Initialize a random population of valid TSP paths (chromosomes).
        /// 2. Evaluate fitness (total path cost).
        /// 3. Repeat for N generations:
        ///     a. Select parent pairs (tournament selection).
        ///     b. Apply crossover (PMX – Partially Mapped Crossover).
        ///     c. Apply mutation (swap two cities with small probability).
        ///     d. Add best solutions to next generation (elitism).
        /// 4. Return the best path found.
        /// 
        /// ✅ Works on undirected graphs with triangle inequality (best performance).
        /// </summary>
        public static void RunGenetic(Graph graph, string? toDotPath = null)
        {
            const int populationSize = 100;
            const int generations = 3;
            const double mutationRate = 0.1;
            const int tournamentSize = 5;
            Random rng = new();

            // === Step 1: Validate graph ===
            if (graph.VertexCount < 3)
            {
                Console.WriteLine("TSP Genetic Algorithm requires at least 3 vertices.");
                return;
            }

            if (!graph.IsConnected() || !graph.AllEdgesPositive())
            {
                Console.WriteLine("Graph must be connected and have positive edge weights.");
                return;
            }

            var cities = graph.Vertices.Keys.ToList();
            int cityCount = cities.Count;

            // === Step 2: Generate initial population ===
            List<List<int>> population = new();
            for (int i = 0; i < populationSize; i++)
            {
                var tour = cities.OrderBy(_ => rng.Next()).ToList();
                population.Add(tour);
            }

            // === Step 3: Evaluate fitness ===
            int GetTourCost(List<int> tour)
            {
                int cost = 0;
                for (int i = 0; i < tour.Count - 1; i++)
                    cost += graph.GetEdgeWeight(tour[i], tour[i + 1]);
                cost += graph.GetEdgeWeight(tour[^1], tour[0]); // return to start
                return cost;
            }

            // === Step 4: Evolution loop ===
            List<int> bestTour = population[0];
            int bestCost = GetTourCost(bestTour);

            for (int gen = 0; gen < generations; gen++)
            {
                List<List<int>> newPopulation = new();

                // === Step 4a: Elitism (keep best) ===
                var sortedPop = population.OrderBy(GetTourCost).ToList();
                newPopulation.Add(new List<int>(sortedPop[0]));
                if (GetTourCost(sortedPop[0]) < bestCost)
                {
                    bestCost = GetTourCost(sortedPop[0]);
                    bestTour = new List<int>(sortedPop[0]);
                }

                // === Step 4b: Selection and Crossover ===
                while (newPopulation.Count < populationSize)
                {
                    var parent1 = TournamentSelect(population, GetTourCost, tournamentSize, rng);
                    var parent2 = TournamentSelect(population, GetTourCost, tournamentSize, rng);
                    var child = PmxCrossover(parent1, parent2, rng);

                    // === Step 4c: Mutation ===
                    if (rng.NextDouble() < mutationRate)
                        SwapMutation(child, rng);

                    newPopulation.Add(child);
                }

                population = newPopulation;
            }

            // === Step 5: Output result ===
            bestTour.Add(bestTour[0]); // complete cycle
            Console.WriteLine($"Best TSP path (Genetic): {string.Join(" -> ", bestTour)}");
            Console.WriteLine($"Total cost: {bestCost}");

            if (toDotPath != null)
                GraphPrinter.ExportWithCycle(graph, bestTour, toDotPath);
        }

        /// <summary>
        /// Selects a parent using tournament selection.
        /// Picks k individuals at random and returns the best.
        /// </summary>
        private static List<int> TournamentSelect(List<List<int>> population, Func<List<int>, int> costFunc, int k, Random rng)
        {
            var candidates = population.OrderBy(_ => rng.Next()).Take(k).ToList();
            return candidates.OrderBy(costFunc).First();
        }

        /// <summary>
        /// Applies Partially Mapped Crossover (PMX) to two parents to create a child.
        /// Preserves relative order and positions.
        /// </summary>
        private static List<int> PmxCrossover(List<int> parent1, List<int> parent2, Random rng)
        {
            int size = parent1.Count;
            int start = rng.Next(size);
            int end = rng.Next(start + 1, size + 1);

            var child = new int[size];
            Array.Fill(child, -1);

            // === Step 1: Copy slice from parent1 ===
            for (int i = start; i < end; i++)
                child[i] = parent1[i];

            // === Step 2: Map remaining genes from parent2 ===
            for (int i = start; i < end; i++)
            {
                if (!child.Contains(parent2[i]))
                {
                    int pos = i;
                    while (child[pos] != -1)
                        pos = parent2.IndexOf(parent1[pos]);
                    child[pos] = parent2[i];
                }
            }

            // === Step 3: Fill remaining positions from parent2 ===
            for (int i = 0; i < size; i++)
                if (child[i] == -1)
                    child[i] = parent2[i];

            return child.ToList();
        }

        /// <summary>
        /// Applies swap mutation to a tour: randomly swaps two cities.
        /// </summary>
        private static void SwapMutation(List<int> tour, Random rng)
        {
            int i = rng.Next(tour.Count);
            int j = rng.Next(tour.Count);
            (tour[i], tour[j]) = (tour[j], tour[i]);
        }

    }
}
