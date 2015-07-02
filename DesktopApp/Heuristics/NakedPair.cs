using DesktopApp.Structure;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Heuristics
{
    class NakedPair : Heuristic
    {
        public NakedPair(Table t)
        {
            this.table = t;
        }

        public override Boolean Apply()
        {
            isChanged = false;

            for (int i = 0; i < 9; i++)
            {
                HeuristicInnerNakedPair(TableSearch.GetColumnByIndex(i, table.Cells).ToList());
                HeuristicInnerNakedPair(TableSearch.GetRowByIndex(i, table.Cells).ToList());
            }
            foreach (var box in table.Boxes)
            {
                HeuristicInnerNakedPair(box.Cells);
            }

            return isChanged;
        }

        private void HeuristicInnerNakedPair(List<Cell> cells)
        {
            int[,] pairs = new int[10, 2];
            int pairCount = 0;
            List<Cell> goodCells = new List<Cell>();

            foreach (var cell in cells)
            {
                bool inPairs = false;
                if (cell.Candidates.Count == 2)
                {
                    for (int i = 0; i < pairCount; i++)
                        if ((pairs[i, 0] == cell.Candidates.First()) && (pairs[i, 1] == cell.Candidates.Last()))
                        {
                            goodCells.Add(cell);
                            inPairs = true;
                        }

                    if (!inPairs)
                    {
                        pairs[pairCount, 0] = cell.Candidates.First();
                        pairs[pairCount, 1] = cell.Candidates.Last();
                        pairCount++;
                    }
                }
            }

            foreach (var goodCell in goodCells)
                foreach (var cell in cells)
                    if (cell.Candidates.Count == 2 &&
                        cell.Candidates.Contains(goodCell.Candidates.First()) &&
                        cell.Candidates.Contains(goodCell.Candidates.Last()))
                    {
                        foreach (var neighCell in cells)
                            if (neighCell.Candidates.Count > 2)
                                foreach (var good in goodCell.Candidates)
                                    if (neighCell.Candidates.Contains(good))
                                    {
                                        neighCell.RemoveCandidateIfExist(good);
                                        neighCell.Candidates.Sort();
                                        neighCell.Panel.Refresh();
                                        isChanged = true;
                                    }
                    }
        }
    }
}
