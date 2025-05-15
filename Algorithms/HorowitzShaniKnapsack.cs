namespace OptimizationMethods.Algorithms
{
    /// <summary>
    /// Solves the 0/1 Knapsack Problem using the Horowitz and Sahni Branch-and-Bound method.
    /// 
    /// 🎯 Problem:
    /// Given n items with values and weights, and a maximum knapsack capacity,
    /// choose a subset of items such that total weight ≤ capacity and total value is maximized.
    /// 
    /// 🧠 Approach:
    /// - Construct a binary decision tree where each node represents a partial selection.
    /// - Left child: includes item at current depth.
    /// - Right child: excludes item.
    /// - At each leaf (depth == n), check if solution is better than the current best.
    /// - Prune (stop expanding) branches that exceed capacity or can't improve best value.
    /// 
    /// ⏱ Time Complexity: Worst-case O(2^n), but effective pruning reduces average runtime.
    /// </summary>
    public static class HorowitzSahniKnapsack
    {
        public class Node
        {
            public float TotalWeight { get; set; }
            public float TotalValue { get; set; }
            public int Depth { get; set; } // Index in items
            public Node? Parent { get; set; }
            public Node? Left { get; set; } // include
            public Node? Right { get; set; } // exclude
            public bool CanExpand { get; set; } = true;
        }

        /// <summary>
        /// Solves the knapsack problem and returns the indexes of selected items.
        /// </summary>
        public static List<int> Solve(List<float> values, List<float> weights, float capacity)
        {
            if (values.Count != weights.Count)
                throw new ArgumentException("Values and weights must match.");

            float bestValue = -1;
            var bestSolution = new List<int>();

            var root = new Node
            {
                TotalWeight = 0,
                TotalValue = 0,
                Depth = 0
            };

            var current = root;

            while (root.CanExpand)
            {
                current = Step(current, values, weights, capacity, ref bestValue, bestSolution);
            }

            return bestSolution;
        }

        /// <summary>
        /// Explores one step of the binary decision tree.
        /// </summary>
        private static Node Step(Node current, List<float> values, List<float> weights, float capacity,
                                 ref float bestValue, List<int> bestSolution)
        {
            // === Step 1: Try going right if already created and valid ===
            if (current.Right?.CanExpand == true)
            {
                return current.Right;
            }

            // === Step 2: At a leaf node — check if complete solution ===
            if (current.Left == null && current.Right == null)
            {
                if (current.Depth == values.Count)
                {
                    if (current.TotalValue > bestValue)
                    {
                        bestValue = current.TotalValue;
                        ExtractSolution(current, bestSolution);
                    }

                    current.CanExpand = false;
                    return current.Parent!;
                }

                // === Step 3: Try to create left child (include item) ===
                float w = weights[current.Depth];
                float v = values[current.Depth];

                if (current.TotalWeight + w <= capacity)
                {
                    current.Left = new Node
                    {
                        TotalWeight = current.TotalWeight + w,
                        TotalValue = current.TotalValue + v,
                        Depth = current.Depth + 1,
                        Parent = current
                    };
                }

                // === Step 4: Always create right child (exclude item) ===
                current.Right = new Node
                {
                    TotalWeight = current.TotalWeight,
                    TotalValue = current.TotalValue,
                    Depth = current.Depth + 1,
                    Parent = current
                };

                return current.Left ?? current.Right!;
            }

            // === Step 5: Backtrack ===
            current.Left = null;
            current.Right = null;
            current.CanExpand = false;
            return current.Parent!;
        }

        /// <summary>
        /// Walks back from leaf to root and collects indexes of included items.
        /// </summary>
        private static void ExtractSolution(Node leaf, List<int> result)
        {
            result.Clear();
            var current = leaf;

            while (current?.Parent != null)
            {
                if (current == current.Parent.Left)
                {
                    result.Add(current.Parent.Depth);
                }
                current = current.Parent;
            }

            result.Reverse(); // To get increasing order of item indexes
        }

        public static Node SolveWithRoot(List<float> values, List<float> weights, float capacity, out List<int> selected)
        {
            selected = new List<int>();
            float bestValue = -1;
            var root = new Node { TotalWeight = 0, TotalValue = 0, Depth = 0 };
            var current = root;

            while (root.CanExpand)
            {
                current = Step(current, values, weights, capacity, ref bestValue, selected);
            }

            return root;
        }

    }
}
