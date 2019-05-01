using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Program
    {
        static Puzzle Solve(Puzzle p)
        {
            Puzzle curPuzzle = p;
            IOrderedEnumerable<KeyValuePair<Tuple<int, int>, List<int>>> curPossibleValues;

            // Loop until the puzzle either has no more empty cells, or has no more empty cells that have only 1 possible valid value
            // Sort by the number of possible values for a given cell so that we can fill in all the cells with only 1 possible value
            while (
                (curPossibleValues = curPuzzle.EmptyCellPossibleValues().OrderBy(kvp => kvp.Value.Count)).Any() &&
                curPossibleValues.First().Value.Count == 1)
            {
                // Fill in the cells that have only one possible valid value
                foreach (KeyValuePair<Tuple<int, int>, List<int>> kvp in curPossibleValues)
                {
                    // If there is only one possible value for this cell, then fill the cell in with that value
                    if (kvp.Value.Count == 1)
                    {
                        // Check that the value is still valid, as the puzzle has changed since the possible values were determined
                        int row = kvp.Key.Item1;
                        int col = kvp.Key.Item2;
                        int value = kvp.Value[0];
                        if (curPuzzle.PossibleValuesForCell(row, col).Contains(value))
                        {
                            curPuzzle = new Puzzle(curPuzzle, kvp.Key, kvp.Value[0]);
                        }
                        // If the value isn't valid then we need to update the curPossibleValues mapping, so break out of the inner loop
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Check if there are empty cells remaining
            if (curPossibleValues.Any())
            {
                // Check if there is a cell with no possible values, that would mean this solution is invalid and null should be returned
                if (curPossibleValues.First().Value.Count == 0)
                {
                    return null;
                }
                // Otherwise, branch for every possible value in the cell with the least number of possible valid values
                foreach (int val in curPossibleValues.First().Value)
                {
                    // Attempt to solve this branch of the puzzle
                    Puzzle attempt = Solve(new Puzzle(curPuzzle, curPossibleValues.First().Key, val));
                    // If a puzzle was returned, then a solution was found (perform an extra verification to catch any potential solver issues)
                    if (attempt != null && attempt.Verify())
                    {
                        return attempt;
                    }
                }
                // All attempts failed so no valid solution exits; return null
                return null;
            }
            // Otherwise, the puzzle is complete and can be returned
            else
            {
                return curPuzzle;
            }
        }

        static void Main(string[] args)
        {
            // Get the puzzle directory from the user
            Console.Write("Please enter the directory with puzzle files: ");
            string puzzleDir = Console.ReadLine();
            
            // Read all the puzzles in the puzzle directory and attempt to solve each of them
            List<Puzzle> allPuzzles = PuzzleReader.ReadAllInDirectory(puzzleDir);

            foreach (Puzzle p in allPuzzles)
            {
                // If Solve returns non-null, then a valid solution was found
                Puzzle solved = Solve(p);
                if (solved != null)
                {
                    Console.WriteLine("Solution for puzzle " + p.GetName());
                    Console.WriteLine(solved);
                    Console.WriteLine();

                    string solutionFilename = Path.GetFileNameWithoutExtension(p.GetName()) + ".sln.txt";
                    string solutionPath = Path.Combine(puzzleDir, solutionFilename);

                    using (StreamWriter writer = new StreamWriter(solutionPath))
                    {
                        writer.WriteLine(solved);
                    }
                }
                // Inform the user that the puzzle could not be solved
                else
                {
                    Console.WriteLine("Could not solve puzzle " + p.GetName());
                }
            }

            Console.WriteLine("Solver done, press any key to continue...");
            Console.ReadKey();
        }
    }
}
