namespace OptimizationMethods.Graphs
{
    /// <summary>
    /// Represents a graph using an adjacency list.
    /// Internally always directed.
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// Indicates whether the graph was logically intended as directed.
        /// Only affects printing and external representations.
        /// </summary>
        public bool IsDirectedLogical { get; }

        /// <summary>
        /// Dictionary of vertices, indexed by vertex ID.
        /// </summary>
        public Dictionary<int, Vertex> Vertices { get; }

        /// <summary>
        /// List of all edges (always stored as directed).
        /// </summary>
        public List<Edge> Edges { get; }

        /// <summary>
        /// Initializes a new graph as logically directed or undirected.
        /// Internally, graphs are always stored as directed.
        /// </summary>
        /// <param name="isDirectedLogical">True if the graph is logically directed.</param>
        public Graph(bool isDirectedLogical)
        {
            IsDirectedLogical = isDirectedLogical;
            Vertices = new Dictionary<int, Vertex>();
            Edges = new List<Edge>();
        }

        /// <summary>
        /// Adds an edge to the graph. Creates vertices if necessary.
        /// For logically undirected graphs, adds two directed edges.
        /// </summary>
        public void AddEdge(int from, int to, int weight = 1)
        {
            if (!Vertices.ContainsKey(from))
                Vertices[from] = new Vertex(from);
            if (!Vertices.ContainsKey(to))
                Vertices[to] = new Vertex(to);

            Vertices[from].AddNeighbor(to);
            Edges.Add(new Edge(from, to, weight));

            if (!IsDirectedLogical)
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
            Console.WriteLine(IsDirectedLogical ? "Directed Graph:" : "Undirected Graph:");
            foreach (var v in Vertices.Values)
                Console.WriteLine(v);
        }

        /// <summary>
        /// Returns the total number of vertices.
        /// </summary>
        public int VertexCount => Vertices.Count;

        /// <summary>
        /// Returns the number of logical edges (for undirected graphs, counts half).
        /// </summary>
        public int EdgeCount => IsDirectedLogical ? Edges.Count : Edges.Count / 2;

        /// <summary>
        /// Checks if the graph is connected using DFS traversal.
        /// Ignores isolated vertices (those without neighbors).
        /// </summary>
        public bool IsConnected()
        {
            var visited = new HashSet<int>();
            int start = Vertices.Values.FirstOrDefault(v => v.Neighbors.Count > 0)?.Id ?? -1;

            if (start == -1)
                return true; // No edges, trivially connected

            DFS(start, visited);

            return Vertices.Values
                           .Where(v => v.Neighbors.Count > 0)
                           .All(v => visited.Contains(v.Id));
        }

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
        /// Checks if all vertices have even degree.
        /// For logically undirected graphs.
        /// </summary>
        public bool AllVerticesHaveEvenDegree()
        {
            if (IsDirectedLogical)
                throw new InvalidOperationException("Even degree check only applies to undirected graphs.");

            return Vertices.Values.All(v => v.Neighbors.Count % 2 == 0);
        }

        /// <summary>
        /// Checks if the graph is bipartite using BFS 2-coloring.
        /// If yes, returns true and optionally outputs the left partition.
        /// </summary>
        public bool IsBipartite(out HashSet<int> leftPartition)
        {
            var color = new Dictionary<int, int>();
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
                                leftPartition = null;
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the weight of an edge from one node to another.
        /// </summary>
        public int GetEdgeWeight(int from, int to)
        {
            var edge = Edges.FirstOrDefault(e => e.From == from && e.To == to);
            if (edge == null)
                throw new InvalidOperationException($"Edge {from} → {to} not found.");
            return edge.Weight;
        }

        /// <summary>
        /// Checks if all edge weights are positive.
        /// </summary>
        public bool AllEdgesPositive()
        {
            return Edges.All(e => e.Weight >= 0);
        }

        /// <summary>
        /// Checks if the graph is strongly connected.
        /// Only meaningful for logically directed graphs.
        /// A directed graph is strongly connected if every vertex is reachable from every other vertex.
        /// 
        /// (Two DFS traversals: once normally, once on the reversed graph.)
        /// </summary>
        public bool IsStronglyConnected()
        {
            if (!IsDirectedLogical)
                throw new InvalidOperationException("Strong connectivity check is only valid for logically directed graphs.");

            int start = Vertices.Keys.FirstOrDefault();
            if (start == 0 && Vertices.Count == 0)
                return true; // Empty graph considered trivially strongly connected

            var visited = new HashSet<int>();
            DFS(start, visited);
            if (visited.Count != VertexCount)
                return false;

            visited.Clear();
            DFSReverse(start, visited);
            return visited.Count == VertexCount;
        }

        /// <summary>
        /// DFS traversal following reversed edges.
        /// </summary>
        private void DFSReverse(int current, HashSet<int> visited)
        {
            visited.Add(current);
            foreach (var vertex in Vertices.Values)
            {
                if (vertex.Neighbors.Contains(current) && !visited.Contains(vertex.Id))
                    DFSReverse(vertex.Id, visited);
            }
        }

        /// <summary>
        /// Checks if every vertex has equal in-degree and out-degree.
        /// </summary>
        public bool AllVerticesBalancedDirected()
        {
            var inDegrees = new Dictionary<int, int>();
            var outDegrees = new Dictionary<int, int>();

            foreach (var vertex in Vertices.Keys)
            {
                inDegrees[vertex] = 0;
                outDegrees[vertex] = 0;
            }

            foreach (var edge in Edges)
            {
                outDegrees[edge.From]++;
                inDegrees[edge.To]++;
            }

            return Vertices.Keys.All(v => inDegrees[v] == outDegrees[v]);
        }

        /// <summary>
        /// Adds a vertex to the graph by ID.
        /// Does nothing if the vertex already exists.
        /// </summary>
        public void AddVertex(int id)
        {
            if (!Vertices.ContainsKey(id))
                Vertices[id] = new Vertex(id);
        }

        /// <summary>
        /// Computes the Minimum Spanning Tree (MST) using Kruskal's algorithm.
        ///
        /// 👣 Step-by-step:
        /// 1. Initialize each vertex as its own disjoint set (Union-Find structure).
        /// 2. Sort all edges in increasing order of weight.
        /// 3. Traverse edges in that order:
        ///     - If the edge connects two different components (i.e., no cycle),
        ///       add it to the MST and union their sets.
        ///     - If it would create a cycle, skip it.
        /// 4. Stop once MST contains exactly (V - 1) edges.
        /// 
        /// 📌 Works only for logically undirected graphs with positive weights.
        /// 📌 Returns the MST as a list of edges.
        /// </summary>
        public List<Edge> MinimumSpanningTree()
        {
            if (IsDirectedLogical)
                throw new InvalidOperationException("MST is only valid for undirected graphs.");
            if (!AllEdgesPositive())
                throw new InvalidOperationException("MST requires all edge weights to be non-negative.");

            var result = new List<Edge>();

            // === Step 1: MakeSet — each vertex is its own parent ===
            var parent = Vertices.Keys.ToDictionary(v => v, v => v);

            // Finds the root of the component containing x (with path compression)
            int Find(int x)
            {
                if (parent[x] != x)
                    parent[x] = Find(parent[x]);
                return parent[x];
            }

            // Unites the components of x and y
            void Union(int x, int y)
            {
                int rootX = Find(x);
                int rootY = Find(y);
                if (rootX != rootY)
                    parent[rootX] = rootY;
            }

            // === Step 2: Sort all edges by weight (ascending) ===
            // To avoid adding both directions in undirected graphs, only keep one direction: From < To
            var sortedEdges = Edges
                .Where(e => e.From < e.To)
                .OrderBy(e => e.Weight);

            // === Step 3: Traverse sorted edges and build MST ===
            foreach (var edge in sortedEdges)
            {
                if (Find(edge.From) != Find(edge.To))
                {
                    result.Add(edge);      // Step 3a: Add edge to MST
                    Union(edge.From, edge.To); // Step 3b: Merge components
                }

                // Early stop: A valid MST has exactly (V - 1) edges
                if (result.Count == Vertices.Count - 1)
                    break;
            }

            return result;
        }

    }
}