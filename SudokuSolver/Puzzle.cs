using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Puzzle
    {
        public static readonly int GRID_SIZE = 9;
        public static readonly int SUBGRID_SIZE = 3;
        public static readonly int GRID_SIZE_SUBGRID = (int)(GRID_SIZE / SUBGRID_SIZE);

        // Grid data array
        private int[,] data;
        private string name;

        /// <summary>
        /// Creates a puzzle from a dictionary of cell positions to their values
        /// </summary>
        /// <param name="filledCells">The mapping of cell position to value of that cell
        /// e.g. (1,2) to 5 means the cell at row 1, column 2 would have a value of 5</param>
        public Puzzle(string puzzleName, Dictionary<Tuple<int, int>, int> filledCells)
        {
            name = puzzleName;
            // Initialize the grid data, where 0 (the default value of a int) represents an empty cell
            data = new int[GRID_SIZE, GRID_SIZE];

            // Fill in the given cells
            foreach (KeyValuePair<Tuple<int, int>, int> kvp in filledCells)
            {
                int row = kvp.Key.Item1;
                int col = kvp.Key.Item2;
                int value = kvp.Value;

                data[row, col] = value;
            }
        }

        /// <summary>
        /// Creates a copy of a puzzle with a singular modified cell
        /// </summary>
        /// <param name="p">The puzzle to copy</param>
        /// <param name="cellPos">The position of the cell to modify</param>
        /// <param name="cellValue">The new value of the modified cell</param>
        public Puzzle(Puzzle p, Tuple<int, int> cellPos, int cellValue)
        {
            // Copy the data from the given puzzle
            data = p.data.Clone() as int[,];
            name = p.name;

            // Modify the given cell
            data[cellPos.Item1, cellPos.Item2] = cellValue;
        }

        public bool Verify()
        {
            // Check all rows
            for (int row = 0; row < GRID_SIZE; row++)
            {
                // Check that every value exists in this row
                List<int> presentValues = new List<int>();
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    if (this[row, col] != 0)
                    {
                        if (presentValues.Contains(this[row, col]))
                        {
                            return false;
                        }
                        presentValues.Add(this[row, col]);
                    }
                }
            }

            // Check all columns
            for (int col = 0; col < GRID_SIZE; col++)
            {
                // Check that every value exists in this column
                List<int> presentValues = new List<int>();
                for (int row = 0; row < GRID_SIZE; row++)
                {
                    if (this[row, col] != 0)
                    {
                        if (presentValues.Contains(this[row, col]))
                        {
                            return false;
                        }
                        presentValues.Add(this[row, col]);
                    }
                }
            }

            // Check all subgrids
            for (int sRow = 0; sRow < GRID_SIZE_SUBGRID; sRow++)
            {
                for (int sCol = 0; sCol < GRID_SIZE_SUBGRID; sCol++)
                {
                    List<int> presentValues = new List<int>();
                    for (int rowOffset = 0; rowOffset < SUBGRID_SIZE; rowOffset++)
                    {
                        for (int colOffset = 0; colOffset < SUBGRID_SIZE; colOffset++)
                        {
                            int row = sRow * SUBGRID_SIZE + rowOffset;
                            int col = sCol * SUBGRID_SIZE + colOffset;
                            if (this[row, col] != 0)
                            {
                                if (presentValues.Contains(this[row, col]))
                                {
                                    return false;
                                }
                                presentValues.Add(this[row, col]);
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the value in the given row, column of the puzzle's grid
        /// </summary>
        public int this[int row, int column]
        {
            get
            {
                if (row >= 0 && row < GRID_SIZE && column >= 0 && column < GRID_SIZE)
                {
                    return data[row,column];
                }
                else
                {
                    return 0;
                }
            }
        }

        public string GetName()
        {
            return name;
        }

        public override string ToString()
        {
            string retVal = "";
            for (int row = 0; row < GRID_SIZE; row++)
            {
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    retVal += this[row, col].ToString();
                }
                if (row < GRID_SIZE - 1)
                {
                    retVal += "\n";
                }
            }
            return retVal;
        }

        /// <summary>
        /// Returns a list containing all the values missing in a given row
        /// </summary>
        public List<int> MissingValuesInRow(int row)
        {
            // Create a list containing all possible values (1 to GRID_SIZE, inclusive) to record their presences
            List<int> MissingValues = Enumerable.Range(1, GRID_SIZE).ToList();

            // Reject invalid row indices
            if (row < 0 || row >= GRID_SIZE) return MissingValues;

            // Check the contents of the row
            for (int col = 0; col < GRID_SIZE; col++)
            {
                // Get the value at this grid cell
                int val = this[row, col];
                if (val > 0 && val <= GRID_SIZE)
                {
                    // Convert the 1-indexed value in the cell to a 0-indexed value and remove it from the missing values
                    MissingValues.Remove(val);
                }
            }

            return MissingValues;
        }

        /// <summary>
        /// Returns whether a given row has been completed
        /// </summary>
        public bool IsRowComplete(int row)
        {
            return !MissingValuesInRow(row).Any();
        }

        /// <summary>
        /// Returns a list containing all the values missing in a given column
        /// </summary>
        public List<int> MissingValuesInColumn(int col)
        {
            // Create a list containing all possible values (1 to GRID_SIZE, inclusive) to record their presences
            List<int> MissingValues = Enumerable.Range(1, GRID_SIZE).ToList();

            // Reject invalid row indices
            if (col < 0 || col >= GRID_SIZE) return MissingValues;

            // Check the contents of the row
            for (int row = 0; row < GRID_SIZE; row++)
            {
                // Get the value at this grid cell
                int val = this[row, col];
                if (val > 0 && val <= GRID_SIZE)
                {
                    // Convert the 1-indexed value in the cell to a 0-indexed value and remove it from the missing values
                    MissingValues.Remove(val);
                }
            }

            return MissingValues;
        }

        /// <summary>
        /// Returns whether a given column has been completed
        /// </summary>
        public bool IsColumnComplete(int col)
        {
            return !MissingValuesInColumn(col).Any();
        }

        /// <summary>
        /// Returns a list containing all the values missing in a given subgrid (3x3 region)
        /// </summary>
        public List<int> MissingValuesInSubgrid(int subgridRow, int subgridCol)
        {
            // Create a list containing all possible values (1 to GRID_SIZE, inclusive) to record their presences
            List<int> MissingValues = Enumerable.Range(1, GRID_SIZE).ToList();
            
            // Reject invalid subgrid positions
            if (subgridRow < 0 || subgridRow >= GRID_SIZE_SUBGRID || subgridCol < 0 || subgridCol >= GRID_SIZE_SUBGRID) return MissingValues;

            // Check the contents of the subgrid
            for (int rowOffset = 0; rowOffset < SUBGRID_SIZE; rowOffset++)
            {
                for (int colOffset = 0; colOffset < SUBGRID_SIZE; colOffset++)
                {
                    // Calculate the position of the grid given the position in the selected subgrid
                    int row = (int)(rowOffset + subgridRow * SUBGRID_SIZE);
                    int col = (int)(colOffset + subgridCol * SUBGRID_SIZE);
                    // Get the value at this grid cell
                    int val = this[row, col];
                    if (val > 0 && val <= GRID_SIZE)
                    {
                        // Convert the 1-indexed value in the cell to a 0-indexed value and record its presence
                        MissingValues.Remove(val);
                    }
                }
            }

            return MissingValues;
        }

        /// <summary>
        /// Returns whether a given subgrid (3x3 region) is complete
        /// </summary>
        public bool IsSubgridComplete(int subgridRow, int subgridCol)
        {
            return !MissingValuesInSubgrid(subgridRow, subgridCol).Any();
        }

        /// <summary>
        /// Returns a list of the possible valid values for a given cell
        /// </summary>
        public List<int> PossibleValuesForCell(int row, int col)
        {
            // Reject invalid cell positions, return an empty list (no valid values for an invalid position)
            if (row < 0 || row >= GRID_SIZE || col < 0 || col >= GRID_SIZE)
                return Enumerable.Empty<int>().ToList();

            List<int> possibleRowValues = MissingValuesInRow(row);
            List<int> possibleColValues = MissingValuesInColumn(col);
            List<int> possibleSubValues = MissingValuesInSubgrid(row / SUBGRID_SIZE, col / SUBGRID_SIZE);

            // Return the intersection of possible values for the row, column, and subgrid that this cell is in
            // This will be any valid values for the cell
            return possibleRowValues.Intersect(possibleColValues).Intersect(possibleSubValues).ToList();
        }

        /// <summary>
        /// Returns a list of 2-Tuples of positions of empty cells (cells with a value of 0) in the grid
        /// </summary>
        public List<Tuple<int, int>> EmptyCellPositions()
        {
            // Keep list to record empty positions in
            List<Tuple<int, int>> emptyPositions = new List<Tuple<int, int>>();

            // Iterate over grid and record empty positions
            for (int row = 0; row < GRID_SIZE; row++)
            {
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    if (this[row,col] == 0)
                    {
                        emptyPositions.Add(new Tuple<int, int>(row, col));
                    }
                }
            }

            return emptyPositions;
        }

        /// <summary>
        /// Returns a dictionary mapping empty cell positions to a list of possible values for those given cells
        /// </summary>
        public Dictionary<Tuple<int, int>, List<int>> EmptyCellPossibleValues()
        {
            // Creates and returns a dictionary with empty cell positions as the key, and the possible values for that cell as the 
            return EmptyCellPositions().ToDictionary(
                t => t,
                t => PossibleValuesForCell(t.Item1, t.Item2));
        }
    }
}
