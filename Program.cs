using OptimizationMethods.Algorithms;
using OptimizationMethods.Graphs;

class Program
{
    static void Main(string[] args)
    {
        // Euler Cycle
        var graph = GraphParser.LoadGraphFromFile("TestData/euler-yes.txt");
        graph.PrintGraph();
        EulerCycle.RunAndPrint(graph);
        Console.WriteLine("----");
        graph = GraphParser.LoadGraphFromFile("TestData/euler-no.txt");
        graph.PrintGraph();
        EulerCycle.RunAndPrint(graph);

        Console.WriteLine("-----------------");

        graph = GraphParser.LoadGraphFromFile("TestData/mm-graph.txt");
        graph.PrintGraph();
        MaximumMatching.RunAndPrint(graph);

        Console.WriteLine("-----------------");

        graph = GraphParser.LoadGraphFromFile("TestData/hungarian-graph.txt");
        graph.PrintGraph();
        HungarianAlgorithm.Run(graph);
    }
}