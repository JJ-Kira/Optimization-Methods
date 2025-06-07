using OptimizationMethods.Graphs;

namespace OptimizationMethods.Algorithms
{
    /// <summary>
    /// Znajdowanie maksymalnego skojarzenia w grafie dwudzielnym.
    /// 
    /// 📌 Skojarzenie (ang. matching) w grafie to zbiór krawędzi, które nie mają wspólnych wierzchołków.
    /// 📌 Maksymalne skojarzenie to takie skojarzenie, do którego nie da się dodać więcej krawędzi
    ///     bez naruszenia tej zasady.
    /// 
    /// W grafie dwudzielnym G=(V1 ∪ V2, E), maksymalne skojarzenie znajdziemy przy pomocy
    /// algorytmu bazującego na ścieżkach powiększających.
    /// 
    /// 🔁 Algorytm (złożoność: O(V * E)):
    ///  1. Zaczynamy od (opcjonalnie) podanego początkowego skojarzenia lub pustego.
    ///  2. Szukamy ścieżki powiększającej p względem M.
    ///  3. Jeżeli istnieje, aktualizujemy M := M ⊕ p (suma symetryczna).
    ///  4. Powtarzamy aż nie da się znaleźć więcej ścieżek powiększających.
    /// 
    /// MAKSYMALNE-SKOJARZENIE(G = (V1 ∪ V2,E))
    /// 1) M = podane lub ∅
    /// 2) repeat
    /// 3)     p = ZNAJDŹ-ŚCIEŻKĘ-POWIĘKSZAJĄCĄ(G, M)
    /// 4)     if p ≠ NIL then
    /// 5)         M = M ⊕ p
    /// 6) until p = NIL
    /// 7) return M
    /// </summary>
    public static class MaximumMatching
    {
        private static Dictionary<int, int> match;  // match[x] = y if x is matched with y
        private static HashSet<int> visited;

        /// <summary>
        /// Finds a maximum matching in a bipartite graph.
        /// If leftPartition is not provided, it will be detected automatically.
        /// If initialMatching is provided, starts augmenting from that matching.
        /// </summary>
        public static Dictionary<int, int> FindMaximumMatching(Graph graph, List<int>? leftPartition = null, Dictionary<int, int>? initialMatching = null)
        {
            // === Step 1: Check bipartiteness and get partition ===
            if (!graph.IsBipartite(out var autoPartition))
                throw new InvalidOperationException("Graph is not bipartite.");

            if (leftPartition == null)
            {
                leftPartition = autoPartition.ToList();
                Console.WriteLine("Left partition:");
                Console.WriteLine(string.Join(", ", leftPartition));
            }

            // === Step 2: Initialize matching (M = ∅ or from input) ===
            match = new Dictionary<int, int>();
            foreach (var v in graph.Vertices.Keys)
                match[v] = -1;

            if (initialMatching != null)
            {
                foreach (var kvp in initialMatching)
                    match[kvp.Key] = kvp.Value;
            }

            bool pathFound;

            // === Step 3: Repeat until no augmenting path exists ===
            do
            {
                pathFound = false;

                // Step 3a: For each unmatched vertex in the left partition
                foreach (int u in leftPartition)
                {
                    if (match[u] == -1) // Only attempt DFS if unmatched
                    {
                        var visited = new HashSet<int>();  // fresh visited set for each attempt
                                                           // Step 3b: Try to find an augmenting path starting at u
                        if (FindAugmentingPath(graph, u, visited))
                            pathFound = true;
                    }
                }

            } while (pathFound);  // Step 3c: Repeat if any path was found

            // === Step 4: Return unique match pairs (u < v) ===
            return match.Where(p => p.Key < p.Value)
                        .ToDictionary(p => p.Key, p => p.Value);
        }

        /// <summary>
        /// Szuka ścieżki powiększającej z danego wierzchołka.
        /// </summary>
        /// <summary>
        /// Step 3b: Recursively tries to find an augmenting path starting from u
        /// </summary>
        /// <summary>
        /// Step 3b: Recursively tries to find an augmenting path starting from u
        /// </summary>
        private static bool FindAugmentingPath(Graph graph, int u, HashSet<int> visited)
        {
            if (visited.Contains(u)) return false;
            visited.Add(u); // Moved visited logic here for localized marking

            foreach (int v in graph.Vertices[u].Neighbors)
            {
                // Step 3b(i): If v is unmatched or its current match can be re-matched recursively
                if (match[v] == -1 || FindAugmentingPath(graph, match[v], visited))
                {
                    // Step 3b(ii): Match u and v
                    match[u] = v;
                    match[v] = u;
                    return true;
                }
            }

            // Step 3b(iii): No augmenting path found from u
            return false;
        }

        /// <summary>
        /// Uruchamia algorytm i wypisuje maksymalne skojarzenie.
        /// Opcjonalnie można podać lewą partycję i początkowe skojarzenie.
        /// TODO: brakuje jednej krawędzi w rozwiązaniu
        /// </summary>
        public static void RunAndPrint(Graph graph, List<int>? leftPartition = null, Dictionary<int, int>? initialMatching = null, string? toDotPath = null)
        {
            var result = FindMaximumMatching(graph, leftPartition, initialMatching);

            Console.WriteLine("Maximum matching:");
            foreach (var pair in result)
                Console.WriteLine($"{pair.Key} - {pair.Value}");

            if (toDotPath != null)
                GraphPrinter.ExportWithMatching(graph, result, toDotPath);
        }
    }
}