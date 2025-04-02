namespace OptimizationMethods.Graphs
{
    internal class Vertex
    {
        public int Id { get; }
        public List<int> Neighbors { get; }

        public Vertex(int id)
        {
            Id = id;
            Neighbors = new List<int>();
        }

        public void AddNeighbor(int neighborId)
        {
            if (!Neighbors.Contains(neighborId))
                Neighbors.Add(neighborId);
        }

        public override string ToString()
        {
            var neighborStr = string.Join(", ", Neighbors);
            return $"{Id} -> [{neighborStr}]";
        }
    }
}
