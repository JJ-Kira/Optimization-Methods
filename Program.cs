#define EULER
#define MM
#define HUNGARY
#define TSP

using OptimizationMethods.Algorithms;
using OptimizationMethods.Graphs;

class Program
{
    static void Main(string[] args)
    {
        // Euler Cycle
#if EULER
        var graphE = GraphParser.LoadGraphFromFile("TestData/euler-yes.txt");
        graphE.PrintGraph();
        EulerCycle.RunAndPrint(graphE, "euler-output.dot");
        Console.WriteLine("----");
        graphE = GraphParser.LoadGraphFromFile("TestData/euler-no.txt");
        graphE.PrintGraph();
        EulerCycle.RunAndPrint(graphE);

        Console.WriteLine("-----------------");
#endif
#if MM
        var graphM = GraphParser.LoadGraphFromFile("TestData/mm-graph.txt");
        graphM.PrintGraph();
        MaximumMatching.RunAndPrint(graphM, toDotPath: "max-output.dot");

        Console.WriteLine("-----------------");
#endif
#if HUNGARY
        var graphH = GraphParser.LoadGraphFromFile("TestData/hungarian-graph.txt");
        graphH.PrintGraph();
        GraphPrinter.WriteDotFile(graphH, "hungarian-input.dot");
        HungarianAlgorithm.Run(graphH, "hungarian-output.dot");

        Console.WriteLine("-----------------");
#endif
#if TSP
        var graph = GraphParser.LoadGraphFromFile("TestData/tsp-graph.txt");
        graph.PrintGraph();
        TravelingSalesmanProblem.RunBranchAndBound(graph, "tsp-output.dot");

#endif
    }
}