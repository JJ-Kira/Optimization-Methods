using OptimizationMethods.Graphs;

namespace OptimizationMethods.Algorithms
{
    /// <summary>
    /// Hungarian (Kuhn-Munkres) algorithm for finding a minimum-cost perfect matching
    /// in a complete bipartite directed weighted graph.
    /// 
    /// 📚 Based on lecture pseudocode and definition from "Metody optymalizacji":
    /// Step-by-step:
    /// 1. Initialize labels:
    ///    - l(x) = max weight(x, y) for each x in V1
    ///    - l(y) = 0 for all y in V2
    /// 2. Build the equality graph Gl:
    ///    - Includes edges where l(x) + l(y) == c(x,y)
    /// 3. Find a maximum matching in Gl using DFS
    /// 4. If matching is not perfect, build alternating tree and update labels
    /// 5. Repeat until perfect matching found
    /// 
    /// Algorytm węgierski (Kuhn-Munkres) do wyznaczania skojarzenia o minimalnym koszcie
    /// w dwudzielnym grafie ważonym G=(V1 ∪ V2, E, C).
    ///
    /// 🧠 Idea:
    /// Problem przypisania: mamy N pracowników i N zadań. Koszty przypisania każdego
    /// pracownika do każdego zadania są znane. Chcemy przypisać każdemu pracownikowi jedno
    /// zadanie tak, aby suma kosztów była minimalna.
    ///
    /// 📊 Reprezentacja:
    /// - Wierzchołki z lewej strony (V1) reprezentują pracowników.
    /// - Wierzchołki z prawej strony (V2) reprezentują zadania.
    /// - Krawędź (u, v) ma wagę c(u,v) — koszt przypisania u → v.
    ///
    /// 📐 Założenia:
    /// - Graf dwudzielny z N wierzchołkami po obu stronach (pełna dwudzielność).
    /// - Koszty są nieujemne.
    /// - Krawędzie istnieją między każdą parą (u ∈ V1, v ∈ V2).
    ///
    /// 🔁 Kroki algorytmu (oparte na materiałach z wykładu):
    /// 1. Zainicjuj etykiety:
    ///    - l(y) = 0 dla y ∈ V2
    ///    - l(x) = max(c(x, y)) dla x ∈ V1
    /// 2. Twórz graf równy Gl, w którym: l(x) + l(y) == c(x, y)
    /// 3. Znajdź pełne skojarzenie w Gl
    /// 4. Jeśli brak pełnego skojarzenia:
    ///    a. Buduj drzewo naprzemienne (S, T)
    ///    b. Oblicz α = min { l(x)+l(y)-c(x,y) } dla x∈S, y∉T
    ///    c. Aktualizuj etykiety:
    ///       - l(x) -= α dla x ∈ S
    ///       - l(y) += α dla y ∈ T
    ///    d. Powtórz aż znajdziesz ścieżkę powiększającą
    /// 5. Gdy znajdziesz pełne skojarzenie M w Gl, zwróć je jako optymalne
    ///
    /// 💡 Gwarancja: Algorytm zwraca skojarzenie o minimalnym koszcie (optymalne).
    /// 🕒 Złożoność czasowa: O(V³)
    /// </summary>

    public static class HungarianAlgorithm
    {
        private static Dictionary<int, int> label;
        private static Dictionary<int, int> match;
        private static Dictionary<int, int> parent;
        private static HashSet<int> visited;
        private static HashSet<int> S, T;

        public static void Run(Graph graph)
        {
            // === Step 0: Validate graph ===
            if (!graph.IsDirected)
            {
                Console.WriteLine("Algorithm faile. \nGraph must be directed.");
                return;
            }

            if (!graph.IsBipartite(out var leftPartition))
            {
                Console.WriteLine("Algorithm faile. \nGraph is not bipartite.");
                return;
            }

            var rightPartition = graph.Vertices.Keys.Except(leftPartition).ToList();

            if (leftPartition.Count != rightPartition.Count)
            {
                Console.WriteLine("Algorithm faile. \nPartitions must be of equal size.");
                return;
            }

            if (!AllEdgesHaveWeights(graph))
            {
                Console.WriteLine("Algorithm faile. \nAll edges must have weights.");
                return;
            }

            Console.WriteLine("Input OK. Starting label initialization...");

            // === Step 1: Initialize labels ===
            label = new Dictionary<int, int>();
            foreach (int u in leftPartition)
                label[u] = graph.Vertices[u].Neighbors.Max(v => graph.GetEdgeWeight(u, v));
            foreach (int v in rightPartition)
                label[v] = 0;

            // === Step 2: Initialize empty matching ===
            match = new Dictionary<int, int>();
            foreach (int v in graph.Vertices.Keys)
                match[v] = -1;

            // === Step 3: Main loop for each unmatched vertex in V1 ===
            foreach (int u in leftPartition)
            {
                if (match[u] != -1) continue;

                bool pathFound;
                do
                {
                    visited = new HashSet<int>();
                    parent = new Dictionary<int, int>();
                    S = new HashSet<int> { u };
                    T = new HashSet<int>();

                    pathFound = DFSEqualityGraph(graph, u, leftPartition.ToList<int>(), rightPartition);

                    if (!pathFound)
                    {
                        // === Step 4: Update labels ===
                        int alpha = int.MaxValue;
                        foreach (int x in S)
                        {
                            foreach (int y in rightPartition.Except(T))
                            {
                                int cost = graph.GetEdgeWeight(x, y);
                                alpha = Math.Min(alpha, label[x] + label[y] - cost);
                            }
                        }

                        foreach (int x in S)
                            label[x] -= alpha;
                        foreach (int y in T)
                            label[y] += alpha;
                    }

                } while (!pathFound);
            }

            // === Step 5: Print final matching ===
            Console.WriteLine("Minimum-cost perfect matching found:");
            foreach (var kv in match.Where(kv => leftPartition.Contains(kv.Key)))
                Console.WriteLine($"{kv.Key} -> {kv.Value} (Cost: {graph.GetEdgeWeight(kv.Key, kv.Value)})");

            int totalCost = match.Where(kv => leftPartition.Contains(kv.Key))
                                 .Sum(kv => graph.GetEdgeWeight(kv.Key, kv.Value));
            Console.WriteLine($"Total cost: {totalCost}");
        }

        /// <summary>
        /// DFS on the equality graph to find augmenting paths.
        /// </summary>
        private static bool DFSEqualityGraph(Graph graph, int u, List<int> leftPartition, List<int> rightPartition)
        {
            visited.Add(u);

            foreach (int v in graph.Vertices[u].Neighbors)
            {
                if (label[u] + label[v] != graph.GetEdgeWeight(u, v))
                    continue;

                T.Add(v);

                if (match[v] == -1)
                {
                    // Augmenting path found
                    Augment(u, v);
                    return true;
                }
                else if (!visited.Contains(match[v]))
                {
                    parent[match[v]] = u;
                    if (DFSEqualityGraph(graph, match[v], leftPartition, rightPartition))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reconstructs the augmenting path and updates the matching.
        /// </summary>
        private static void Augment(int u, int v)
        {
            while (true)
            {
                int prev = parent.GetValueOrDefault(u, -1);
                match[u] = v;
                match[v] = u;

                if (prev == -1)
                    break;

                v = prev;
                u = parent[prev];
            }
        }

        private static bool AllEdgesHaveWeights(Graph graph)
        {
            return graph.Edges.All(e => e.Weight > 0);
        }
    }
}
