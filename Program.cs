using OptimizationMethods.Graphs;

class Program
{
    static void Main(string[] args)
    {
        var graph = GraphParser.LoadGraphFromFile("TestData/euler-yes.txt");
        graph.PrintGraph();
    }
}