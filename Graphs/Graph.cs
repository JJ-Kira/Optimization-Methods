using System.Text;

namespace OptimizationMethods.Graphs
{
    /// <summary>
    /// Represents a graph using an adjacency list.
    /// Supports both directed and undirected graphs.
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// Indicates whether the graph is directed.
        /// </summary>
        public bool IsDirected { get; }

        /// <summary>
        /// Dictionary of vertices, indexed by vertex ID.
        /// </summary>
        public Dictionary<int, Vertex> Vertices { get; }

        /// <summary>
        /// List of all edges in the graph.
        /// </summary>
        public List<Edge> Edges { get; }

        /// <summary>
        /// Initializes a new graph as directed or undirected.
        /// </summary>
        /// <param name="isDirected">True if the graph is directed.</param>
        public Graph(bool isDirected)
        {
            IsDirected = isDirected;
            Vertices = new Dictionary<int, Vertex>();
            Edges = new List<Edge>();
        }

        /// <summary>
        /// Adds an edge to the graph. Also ensures the vertices are created and linked.
        /// For undirected graphs, adds the reverse edge as well.
        /// </summary>
        public void AddEdge(int from, int to, int weight = 1)
        {
            if (!Vertices.ContainsKey(from))
                Vertices[from] = new Vertex(from);
            if (!Vertices.ContainsKey(to))
                Vertices[to] = new Vertex(to);

            Vertices[from].AddNeighbor(to);
            Edges.Add(new Edge(from, to, weight));

            if (!IsDirected)
            {
                Vertices[to].AddNeighbor(from);
                Edges.Add(new Edge(to, from, weight));
            }
        }

        /// <summary>
        /// Prints the graph to the console in a readable adjacency list format.
        /// </summary>
        public void PrintGraph()
        {
            Console.WriteLine(IsDirected ? "Directed Graph:" : "Undirected Graph:");
            foreach (var v in Vertices.Values)
                Console.WriteLine(v);
        }

        /// <summary>
        /// Returns the total number of vertices in the graph.
        /// </summary>
        public int VertexCount => Vertices.Count;

        /// <summary>
        /// Returns the number of edges in the graph (undirected counted only once).
        /// </summary>
        public int EdgeCount => IsDirected ? Edges.Count : Edges.Count / 2;

        /// <summary>
        /// Checks if the graph is connected using DFS traversal.
        /// Ignores isolated vertices (those with no neighbors).
        /// 
        /// Useful for algorithms such as Eulerian cycle detection.
        /// </summary>
        public bool IsConnected()
        {
            var visited = new HashSet<int>();
            int start = Vertices.Values.FirstOrDefault(v => v.Neighbors.Count > 0)?.Id ?? -1;

            if (start == -1)
                return true; // No edges, considered trivially connected

            DFS(start, visited);

            // Ensure all vertices with edges are reachable
            return Vertices.Values
                           .Where(v => v.Neighbors.Count > 0)
                           .All(v => visited.Contains(v.Id));
        }

        /// <summary>
        /// Helper method to perform depth-first search.
        /// </summary>
        private void DFS(int current, HashSet<int> visited)
        {
            visited.Add(current);
            foreach (var neighbor in Vertices[current].Neighbors)
            {
                if (!visited.Contains(neighbor))
                    DFS(neighbor, visited);
            }
        }

        /// <summary>
        /// Checks if all vertices in the graph have even degrees.
        /// 
        /// This is a necessary condition for an Eulerian cycle in an undirected graph.
        /// </summary>
        public bool AllVerticesHaveEvenDegree()
        {
            return Vertices.Values.All(v => v.Neighbors.Count % 2 == 0);
        }

        /// <summary>
        /// Checks if the graph is bipartite using BFS 2-coloring.
        /// If yes, returns true and optionally outputs the partition groups.
        /// 
        /// Sprawdza, czy graf jest dwudzielny (bipartite) oraz zwraca wykrytą partycję lewą (V1),
        /// jeśli podział na dwa zbiory jest możliwy.
        ///
        /// Graf dwudzielny to taki, którego zbiór wierzchołków można podzielić na dwa rozłączne zbiory
        /// V1 i V2, tak aby każda krawędź łączyła wierzchołek z V1 z wierzchołkiem z V2.
        ///
        /// 🔎 Algorytm opiera się na kolorowaniu wierzchołków przy pomocy BFS:
        /// 1. Dla każdego nieodwiedzonego wierzchołka v przypisujemy kolor 0 i dodajemy do kolejki.
        /// 2. Iterujemy po kolejce, przypisując sąsiadom przeciwny kolor (1 - kolor rodzica).
        /// 3. Jeśli trafimy na sąsiada o tym samym kolorze → graf nie jest dwudzielny.
        /// 4. Jeśli uda się poprawnie pokolorować cały graf, to graf jest dwudzielny.
        /// 5. Wszystkie wierzchołki z kolorem 0 trafiają do zbioru `leftPartition` (czyli V1).
        ///
        /// Złożoność czasowa: O(V + E)
        /// </summary>
        /// <param name="leftPartition">Zbiór wykrytych wierzchołków należących do lewej partycji (V1).</param>
        /// <returns>True, jeśli graf jest dwudzielny; false w przeciwnym razie.</returns>
        public bool IsBipartite(out HashSet<int> leftPartition)
        {
            var color = new Dictionary<int, int>(); // 0 or 1
            leftPartition = new HashSet<int>();

            foreach (var v in Vertices.Keys)
            {
                if (!color.ContainsKey(v))
                {
                    var queue = new Queue<int>();
                    queue.Enqueue(v);
                    color[v] = 0;
                    leftPartition.Add(v);

                    while (queue.Count > 0)
                    {
                        int u = queue.Dequeue();
                        foreach (int neighbor in Vertices[u].Neighbors)
                        {
                            if (!color.ContainsKey(neighbor))
                            {
                                color[neighbor] = 1 - color[u];
                                queue.Enqueue(neighbor);

                                if (color[neighbor] == 0)
                                    leftPartition.Add(neighbor);
                            }
                            else if (color[neighbor] == color[u])
                            {
                                // Two adjacent vertices with same color → not bipartite
                                leftPartition = null;
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

    }
}
