//#define EULER
//#define MM
//#define HUNGARY
//#define TSP
#define COLOR


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
        var graphM = GraphParser.LoadGraphFromFile("TestData/maksymalne_skojarzenie2.txt");
        graphM.PrintGraph();
        MaximumMatching.RunAndPrint(graphM, toDotPath: "max-output2.dot");

        var initialMatching = new Dictionary<int, int>
        {
            { 1, 5 },
            { 5, 1 },
            { 2, 6 },
            { 6, 2 }
        };

        // === Call with initial matching ===
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
        var graphTSP = GraphParser.LoadGraphFromFile("TestData/komiwojazer1.txt");
        graphTSP.PrintGraph();
        TravelingSalesmanProblem.RunBranchAndBound(graphTSP, "komiwojazer-bb-output.dot");
        TravelingSalesmanProblem.RunGenetic(graphTSP, "komiwojazer-ga1-output.dot");

        Console.WriteLine("----");

        graphTSP = GraphParser.LoadGraphFromFile("TestData/komiwojazer2.txt");
        graphTSP.PrintGraph();
        TravelingSalesmanProblem.RunMstApproximation(graphTSP, "komiwojazer-mst-output.dot");
        TravelingSalesmanProblem.RunGenetic(graphTSP, "komiwojazer-ga2-output.dot");
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
    }
}