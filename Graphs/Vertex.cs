namespace OptimizationMethods.Graphs
{
    public class Vertex
    {
        /// <summary>
        /// Vertex ID (unique identifier).
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// List of neighboring vertex IDs.
        /// Always stores outgoing edges.
        /// </summary>
        public List<int> Neighbors { get; }

        public Vertex(int id)
        {
            Id = id;
            Neighbors = new List<int>();
        }

        /// <summary>
        /// Adds a neighbor (outgoing edge).
        /// </summary>
        public void AddNeighbor(int neighborId)
        {
            if (!Neighbors.Contains(neighborId))
                Neighbors.Add(neighborId);
        }

        /// <summary>
        /// Returns a readable string of the vertex and its neighbors.
        /// </summary>
        public override string ToString()
        {
            var neighborStr = string.Join(", ", Neighbors);
            return $"{Id} -> [{neighborStr}]";
        }
    }
}
