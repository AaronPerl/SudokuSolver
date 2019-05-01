using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class PuzzleReader
    {
        public static Puzzle ReadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Could not open puzzle file: " + filePath);
            }

            // Keep track of filled cells
            Dictionary<Tuple<int, int>, int> filledCells = new Dictionary<Tuple<int, int>, int>();

            // Read all lines in the file (this is safe to do because the files are very short
            // In longer files we would use File.ReadLines and enumerate over them to prevent loading the entire file into memory
            string[] fileLines = File.ReadAllLines(filePath);

            // Iterate over the lines of the file, each line representing a row
            for (int row = 0; row < fileLines.Length; row++)
            {
                string curLine = fileLines[row];
                // Iterate over every character in the line, each representing a single cell
                for (int col = 0; col < curLine.Length; col++)
                {
                    // If the character is not an 'X' then it contains the numeric value in the cell
                    char curChar = curLine[col];
                    if (curChar != 'X')
                    {
                        int cellVal = int.Parse(curChar.ToString());
                        filledCells.Add(new Tuple<int, int>(row, col), cellVal);
                    }
                }
            }

            return new Puzzle(Path.GetFileName(filePath), filledCells);
        }

        public static List<Puzzle> ReadAllInDirectory(string path)
        {
            // Verify that the given path is a valid directory
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Could not access directory: " + path);
            }

            List<Puzzle> puzzles = new List<Puzzle>();
            string[] childFiles = Directory.GetFiles(path);

            foreach (string curFile in childFiles)
            {
                // Ensure that the file is a .txt file and not a puzzle solution
                if (Path.GetExtension(curFile) == ".txt" && !curFile.EndsWith(".sln.txt"))
                {
                    puzzles.Add(ReadFromFile(curFile));
                }
            }

            return puzzles;
        }
    }
}
