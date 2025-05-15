//#define EULER
#define MM
//#define HUNGARY
//#define TSP
//#define COLOR
//#define KNAPSACK

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
        // Load graph and infer initial matching from weight > 1 edges
        var (graphM, initialMatching) = GraphParser.LoadGraphAndInitialMatching("TestData/maksymalne_skojarzenie2.txt");
        // Print for inspection
        graphM.PrintGraph();
        // Run without initial matching (pure algorithm)
        MaximumMatching.RunAndPrint(graphM, toDotPath: "max-output2.dot");
        // Run with initial matching from file weights (e.g. weight == 2)
        MaximumMatching.RunAndPrint(graphM, initialMatching: initialMatching, toDotPath: "max-output.dot");

        Console.WriteLine("-----------------");
#endif
#if HUNGARY
        var graphH = GraphParser.LoadGraphFromFile("TestData/algorytm_wegierski1.txt");
        graphH.PrintGraph();
        GraphPrinter.WriteDotFile(graphH, "hungarian-input.dot");
        HungarianAlgorithm.Run(graphH, "hungarian-output.dot");

        Console.WriteLine("-----------------");
#endif
#if TSP
        var graphTSP = GraphParser.LoadGraphFromFile("TestData/komiwojazer.txt");
        graphTSP.PrintGraph();
        TravelingSalesmanProblem.RunBranchAndBound(graphTSP, "komiwojazer-bb-output.dot");
        TravelingSalesmanProblem.RunMstApproximation(graphTSP, "komiwojazer-mst-output.dot");
        TravelingSalesmanProblem.RunGenetic(graphTSP, "komiwojazer-ga1-output.dot");
#endif
#if COLOR
        var graphC = GraphParser.LoadGraphFromFile("TestData/graph_coloring.txt");
        graphC.PrintGraph();
        var coloringResult = GraphColoring.Color(graphC);
        Console.WriteLine($"Used {coloringResult.NumberOfColorsUsed} colors.");

        foreach (var kv in coloringResult.VertexColors.OrderBy(k => k.Key))
            Console.WriteLine($"Vertex {kv.Key} -> Color {kv.Value}");

        GraphPrinter.ExportWithColoring(graphC, coloringResult.VertexColors, "graph-coloring-output.dot");

        Console.WriteLine("-----------------");
#endif
#if KNAPSACK
        var values = new List<float> { 2f, 7f, 6f, 3f, 4f, 5f };
        var weights = new List<float> { 1f, 2f, 3f, 5f, 1f, 3f };
        float capacity = 5f;

        var selected = HorowitzSahniKnapsack.Solve(values, weights, capacity);
        Console.WriteLine("Selected item indices:");
        foreach (var index in selected)
            Console.WriteLine($"Item {index} -> Value: {values[index]}, Weight: {weights[index]}");

        float total = selected.Sum(i => values[i]);
        Console.WriteLine($"Total value: {total}");

        Console.WriteLine("-----------------");
#endif
    }
}