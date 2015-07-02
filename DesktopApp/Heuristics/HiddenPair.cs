using DesktopApp.Structure;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Heuristics
{
    class HiddenPair : Heuristic
    {
        public HiddenPair(Table t)
        {
            this.table = t;
        }

        public override Boolean Apply()
        {
            isChanged = false;

            foreach (var column in table.Cells)
                foreach (var cell in column)
                    if (cell.Value == 0 && cell.Candidates.Count > 1)
                        for (int i = 1; i < 9; i++)
                            for (int j = i+1; j < 10; j++)
                                if (cell.Candidates.Contains(i) && cell.Candidates.Contains(j))
                                {
                                    InnerHiddenPair(TableSearch.GetColumnByIndex(cell.Y, table.Cells).ToList(), i, j);
                                    InnerHiddenPair(TableSearch.GetRowByIndex(cell.X, table.Cells).ToList(), i, j);
                                    InnerHiddenPair(cell.Box.Cells, i, j);
                                }
            return isChanged;
        }

        private void InnerHiddenPair(List<Cell> cells, int a, int b)
        {
            int countA = 0;
            int countB = 0;
            List<Cell> goodCells = new List<Cell>();

            foreach (var cell in cells)
                if (cell.Value == 0)
                {
                    if (cell.Candidates.Contains(a)) countA++;
                    if (cell.Candidates.Contains(b)) countB++;
                    if (cell.Candidates.Contains(a) && cell.Candidates.Contains(b))
                        goodCells.Add(cell);
                }

            if ((goodCells.Count == 2) && (countA == 2) && (countB == 2))
                foreach (var cell in cells)
                    if (goodCells.Contains(cell))
                    {
                        if (cell.Candidates.Count == 2 && ((a == cell.Candidates[0] && b == cell.Candidates[1]) || (b == cell.Candidates[0] && a == cell.Candidates[1]))) {
                            isChanged = false;
                        } else {                         
                            cell.Candidates.Clear();
                            cell.Candidates.Add(a);
                            cell.Candidates.Add(b);
                            cell.Candidates.Sort();
                            cell.Panel.Refresh();
                            isChanged = true;
                        }
                    }
        }
    }
}
