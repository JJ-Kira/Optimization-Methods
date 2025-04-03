namespace OptimizationMethods.Graphs
{
    public class Edge
    {
        public int From { get; }
        public int To { get; }
        public int Weight { get; }

        public Edge(int from, int to, int weight = 1)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        public override string ToString() => $"{From} -> {To} (Weight: {Weight})";
    }
}
