using ConsoleApp.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Controller
{
    class TableController
    {
        Table table;
        int[,] output;
        List<Box> boxes;

        public TableController()
        {
            InitializeTable();
        }

        private void InitializeTable()
        {
            output = new int[27, 27];
            table = new Table();
            boxes = new List<Box>();

           // for (int i = 0; i < 9; i++) boxes.Add(new Box());          

            for (int y = 0; y < 9; y++)
            {
                List<Cell> row = new List<Cell>();
                Box tempBox = new Box();
                for (int x = 0; x < 9; x++)
                {
                    List<int> candidate = new List<int>();
                    Cell cell = new Cell(x, y, (int)((x + 1) * (y + 1)) % 9);

                    for (int i = 1; i < 10; i++)   candidate.Add(i);

                    cell.Candidates = candidate;
                    row.Add(cell);

                    int boxIndex = (y / 3) + (x / 3) * 3;                        
                    tempBox.Cells.Add(cell);
                }

                foreach (var cell in row)
                {
                    cell.Box = tempBox;
                }

                table.Cells.Add(row);
            }
            CreateOutput();
            Iteraction();
            CreateOutput();
        }

        public void Iteraction()
        {
            List<Cell> refreshNeeded = new List<Cell>();
            //Simulate BasicStep
            table.Cells[3][4].Value = 8;
            refreshNeeded.Add(table.Cells[3][4]);
             
            Heuristic_BasicStep(refreshNeeded);
            Heuristic_NakedSingle();
            Heuristic_HiddenSingle();
            Heuristic_NakedPair();
            refreshNeeded.Clear();
        }

        public void Heuristic_Inner_BasicStep(List<Cell> cells, Cell source)
        {
            foreach (var cell in cells)
            {
                //TODO: mi van ha benne sincs?
                cell.Candidates.Remove(source.Value);
            }
        }

        public void Heuristic_BasicStep(List<Cell> doRefresh)
        {
            foreach (var cell in doRefresh)
            {
                Heuristic_Inner_BasicStep(GetColumnByIndex(cell.Y).ToList(), cell);
                Heuristic_Inner_BasicStep(GetRowByIndex(cell.X).ToList(), cell);
                Heuristic_Inner_BasicStep(cell.Box.Cells, cell);
            }
        }

        public void Heuristic_NakedSingle()
        {
            foreach (var column in table.Cells)
            {
                foreach (var cell in column)
                {
                    if (cell.Candidates.Count == 1)
                    {
                        cell.Value = cell.Candidates[0];
                        cell.Candidates.Clear();
                    }
                }
            }
        }

        public void Heuristic_Inner_HiddenSingle(List<Cell> cells)
        {
            List<Cell> onlyOne_Cell = new List<Cell>();
            for (int i = 0; i < 9; i++) onlyOne_Cell.Add(new Cell());
            List<int> onlyOne_Int = new List<int>();
            List<int> occurred = new List<int>();

            foreach (var cell in cells)
            {
                foreach (var cand in cell.Candidates)
                {
                    if (!occurred.Contains(cand))
                    {
                        onlyOne_Cell[cand - 1] = cell;
                        occurred.Add(cand);
                        onlyOne_Int.Add(cand);
                    }
                    else
                    {
                        //TODO: mi van ha benne sincs?
                        onlyOne_Int.Remove(cand);
                    }
                }
            }

            foreach (var cand in onlyOne_Int)
            {
                Cell fresh = onlyOne_Cell[cand - 1];
                table.Cells[fresh.Y][fresh.X].Candidates.Clear();
                table.Cells[fresh.Y][fresh.X].Candidates.Add(cand);
            }
        }

        public void Heuristic_HiddenSingle()
        {
            for (int num = 0; num < 9; num++)
            {
                Heuristic_Inner_HiddenSingle(GetColumnByIndex(num).ToList());
                Heuristic_Inner_HiddenSingle(GetRowByIndex(num).ToList());
            }

            foreach (var box in boxes)
            {
                Heuristic_Inner_HiddenSingle(box.Cells);
            }
        }

        public void Heuristic_Inner_NakedPair(List<Cell> cells)
        {
            int[,] pairs = new int[5, 2];
            List<int> goodNums = new List<int>();
            bool inPairs = false;

            foreach (var cell in cells)
            {
                if (cell.Candidates.Count == 2)
                {
                    for (int i = 0; i < pairs.Length; i++)
                    {
                        if ((pairs[i, 0] == cell.Candidates.First()) && (pairs[i, 1] == cell.Candidates.Last()))
                        {
                            goodNums.Add(cell.Candidates.First());
                            goodNums.Add(cell.Candidates.Last());
                            inPairs = true;
                        }
                    }
                    if (!inPairs)
                    {
                        int length = pairs.Length;
                        pairs[length, 0] = cell.Candidates.First();
                        pairs[length, 1] = cell.Candidates.Last();
                    }
                }
            }

            if (goodNums.Count > 0)
            {
                foreach (var cell in cells)
                {
                    if (cell.Candidates.Count != 2)
                    {
                        foreach (var good in goodNums)
                        {
                            foreach (var cand in cell.Candidates)
                            {
                                if (cand == good)
                                {
                                    cell.Candidates.Remove(cand);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Heuristic_NakedPair()
        {
            for (int num = 0; num < 9; num++)
            {
                Heuristic_Inner_NakedPair(GetColumnByIndex(num).ToList());
                Heuristic_Inner_NakedPair(GetRowByIndex(num).ToList());
            }
            foreach (var box in boxes)
            {
                Heuristic_Inner_NakedPair(box.Cells);
            }
        }

        public bool CheckTable()
        {
            bool valid = false;
            // Cell by cell
            foreach (var row in table.Cells)
            {
                foreach (var cell in row)
                {
                    valid = CheckCellValidity(cell);
                }
            }
            return valid;
        }

        public bool CheckCellValidity(Cell cell)
        {
            int count = 0;

            // Row
            foreach (var c in GetRowOfCell(cell))
            {
                if (c.Value == cell.Value) count++;
                if (count > 1) return false;
            }

            // Column
            count = 0;
            foreach (var c in GetColumnOfCell(cell))
            {
                if (c.Value == cell.Value) count++;
                if (count > 1) return false;
            }

            // Box
            count = 0;
            foreach (var c in cell.Box.Cells)
            {
                if (c.Value == cell.Value) count++;
                if (count > 1) return false;
            }

            return true;
        }

        public void MakeCandidatesForCell(Cell cell)
        {
            for (int value = 1; value <= 9; value++)
            {
                cell.Value = value; // testing
                if (CheckCellValidity(cell)) cell.Candidates.Add(value);
            }

            if (cell.Candidates.Count() == 1)
            {
                cell.Value = cell.Candidates[0];
                cell.Candidates.Sort();
            }
            else
            {
                cell.Value = 0;
            }
        }

        public void MakeCandidatesForTableCells()
        {
            foreach (var row in table.Cells)
            {
                foreach (var cell in row)
                {
                    MakeCandidatesForCell(cell);
                }
            }
        }

        public void CreateOutput()
        {
            int x = 0;
            int y = 0;

            foreach (var celllist in table.Cells)
            {                
                foreach (var cell in celllist)
                {    
                    for (int c = 0; c < 9; c++)
                    {
                        int innerrow = c / 3;
                        int innercolumn = c % 3;

                        if (cell.Candidates.Contains(c+1))
                            output[innerrow + (x * 3), innercolumn + (y * 3)] = c+1;
                        else
                            output[innerrow + (x * 3), innercolumn + (y * 3)] = 0;
                    }
                    x++;
                }
                y++;
                x = 0;
            }
        } 


        public void WriteToConsole()
        {
            for (int column = 0; column < 27; column++)
            {
                if ((column) % 9 == 0) Console.WriteLine("-------------------------------------------------------------------");
                else
                    if ((column) % 3 == 0) Console.WriteLine();

                for (int row = 0; row < 27; row++)
                {
                    if (row == 0) Console.Write("|");
                    Console.Write(" " + output[column, row]);
                    if (row == 26) Console.Write(" |");
                    else
                        if ((row + 1) % 9 == 0) Console.Write(" |");
                        else
                            if (((row + 1) % 3 == 0) || (row == 26)) Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("-------------------------------------------------------------------");
        }


        /**
         * Utility methods
         */
        private IEnumerable<Cell> GetRowOfCell(Cell cell)
        {
            return GetRowByIndex(cell.Y);
        }

        private IEnumerable<Cell> GetColumnOfCell(Cell cell)
        {
            return GetColumnByIndex(cell.X);
        }

        private IEnumerable<Cell> GetColumnByIndex(int column)
        {
            return table.Cells.Select(innerList => innerList[column]);
        }

        private IEnumerable<Cell> GetRowByIndex(int row)
        {
            return table.Cells.ElementAt(row);
        }
    }
}
