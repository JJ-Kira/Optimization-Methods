using OptimizationMethods.Graphs;
using System.Diagnostics;
using System.Net;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Threading.Channels;

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
    /// algorytmu bazującego na ścieżkach powiększających. Opiera się on na:
    ///  - Twierdzeniu Berge'a: skojarzenie M jest maksymalne ⟺ nie istnieje ścieżka powiększająca.
    /// 
    /// 🔁 Algorytm (złożoność: O(V * E)):
    ///  1. Zaczynamy od pustego skojarzenia M.
    ///  2. Szukamy ścieżki powiększającej p względem M.
    ///  3. Jeżeli istnieje, aktualizujemy M := M ⊕ p (suma symetryczna).
    ///  4. Powtarzamy aż nie da się znaleźć więcej ścieżek powiększających.
    ///  
    /// MAKSYMALNE-SKOJARZENIE(G = (V1 ∪ V2,E))
    /// 1) M = ∅
    /// 2) repeat
    /// 3)     p = ZNAJDŹ-ŚCIEŻKĘ-POWIĘKSZAJĄCĄ(G, M)
    /// 4)     if p ≠ NIL then
    /// 5)         M = M ⊕ p
    /// 6) until p = NIL
    /// 7) return M
    ///
    /// Although both algorithms rely on finding augmenting paths, 
    /// we can't directly reuse MaximumMatching because:
    /// Hungarian uses an equality graph Gl, where edges must satisfy l(u) + l(v) == c(u, v). 
    /// This graph changes after each label update and is not part of the original graph.
    /// Hungarian requires tighter control: 
    /// it builds alternating trees, tracks labels, and modifies matchings dynamically
    /// — beyond what MaximumMatching supports.
    /// While conceptually similar, Hungarian needs a customized DFS and matching logic 
    /// tailored to label conditions and equality constraints.
    /// </summary>
    public static class MaximumMatching
    {
        private static Dictionary<int, int> match;  // match[x] = y if x is matched with y
        private static HashSet<int> visited;

        /// <summary>
        /// Główna funkcja: znajduje maksymalne skojarzenie w grafie dwudzielnym.
        /// Jeśli leftPartition nie zostanie podane, zostanie wyznaczone automatycznie.
        /// </summary>
        public static Dictionary<int, int> FindMaximumMatching(Graph graph, List<int>? leftPartition = null)
        {
            // === Step 1: Check bipartiteness and determine partition if needed ===
            if (!graph.IsBipartite(out var autoPartition))
                throw new InvalidOperationException("Graph is not bipartite.");

            if (leftPartition == null)
            {
                leftPartition = autoPartition.ToList();
                Console.WriteLine("Left partition:");
                Console.WriteLine(string.Join(", ", leftPartition));
            }

            match = new Dictionary<int, int>();

            // Inicjalizacja: wszyscy nieskojarzeni
            foreach (var v in graph.Vertices.Keys)
                match[v] = -1;

            bool pathFound;

            // === Step 2: Repeat while augmenting path is found ===
            do
            {
                pathFound = false;
                visited = new HashSet<int>();

                // Próbuj dla każdego wolnego wierzchołka z lewej strony
                foreach (int u in leftPartition)
                {
                    if (match[u] == -1 && FindAugmentingPath(graph, u))
                        pathFound = true;
                }

            } while (pathFound);

            // Zwracamy tylko unikalne pary (np. 1-5, bez duplikatu 5-1)
            return match.Where(p => p.Key < p.Value)
                        .ToDictionary(p => p.Key, p => p.Value);
        }

        /// <summary>
        /// Szuka ścieżki powiększającej z danego wierzchołka.
        /// </summary>
        private static bool FindAugmentingPath(Graph graph, int u)
        {
            if (visited.Contains(u)) return false;
            visited.Add(u);

            foreach (int v in graph.Vertices[u].Neighbors)
            {
                if (match[v] == -1 || FindAugmentingPath(graph, match[v]))
                {
                    match[u] = v;
                    match[v] = u;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Uruchamia algorytm i wypisuje maksymalne skojarzenie.
        /// </summary>
        public static void RunAndPrint(Graph graph, List<int>? leftPartition = null)
        {
            var result = FindMaximumMatching(graph, leftPartition);

            Console.WriteLine("Maksymalne skojarzenie:");
            foreach (var pair in result)
                Console.WriteLine($"{pair.Key} - {pair.Value}");
        }
    }
}
