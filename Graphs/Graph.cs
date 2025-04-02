namespace OptimizationMethods.Graphs
{
    internal class Graph
    {
        public bool IsDirected { get; }
        public Dictionary<int, Vertex> Vertices { get; }
        public List<Edge> Edges { get; }

        public Graph(bool isDirected)
        {
            IsDirected = isDirected;
            Vertices = new Dictionary<int, Vertex>();
            Edges = new List<Edge>();
        }

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

        public void PrintGraph()
        {
            Console.WriteLine(IsDirected ? "Directed Graph:" : "Undirected Graph:");
            foreach (var v in Vertices.Values)
                Console.WriteLine(v);
        }

        public int VertexCount => Vertices.Count;
        public int EdgeCount => IsDirected ? Edges.Count : Edges.Count / 2;
    }
}
