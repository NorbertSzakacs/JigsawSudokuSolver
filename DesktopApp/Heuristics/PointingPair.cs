using DesktopApp.Structure;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Heuristics
{
    class PointingPair : Heuristic
    {
        public PointingPair(Table t)
        {
            this.table = t;
        }

        public override Boolean Apply()
        {
            isChanged = false;

            foreach (var box in table.Boxes)
            {
                bool[] matchBoolColumn = new bool[9];
                bool[] matchBoolRow = new bool[9];
                for (int i = 0; i < 9; i++)
                {
                    matchBoolColumn[i] = true;
                    matchBoolRow[i] = true;
                }
                int[] candPlaceColumn = new int[9];
                int[] candPlaceRow = new int[9];
                int[] candCount = new int[9];

                foreach (var cell in box.Cells)
                    if (cell.Value == 0)
                        foreach (var cand in cell.Candidates)
                        {
                            if (candCount[cand - 1] == 0)
                            {
                                candPlaceColumn[cand - 1] = cell.Y;
                                candPlaceRow[cand - 1] = cell.X;
                            }
                            else
                            {
                                if (candPlaceColumn[cand - 1] != cell.Y) matchBoolColumn[cand - 1] = false;
                                if (candPlaceRow[cand - 1] != cell.X) matchBoolRow[cand - 1] = false;
                            }
                            candCount[cand - 1]++;
                        }

                for (int i = 0; i < 9; i++)
                    if (candCount[i] < 2)
                    {
                        matchBoolColumn[i] = matchBoolRow[i] = false;
                    }

                for (int i = 0; i < 9; i++)
                {
                    if (matchBoolColumn[i])
                        InnerPointingPair(TableSearch.GetRowByIndex(candPlaceColumn[i], table.Cells).ToList(), i + 1, box);
                    if (matchBoolRow[i])
                        InnerPointingPair(TableSearch.GetColumnByIndex(candPlaceRow[i], table.Cells).ToList(), i + 1, box);
                }
            }

            return isChanged;
        }

        private void InnerPointingPair(List<Cell> cells, int a, Box except)
        {
            foreach (var cell in cells)
                if (!cell.Box.Equals(except))
                {
                    if (cell.Candidates.Count > 0 && cell.Candidates.Contains(a))
                    {
                        cell.RemoveCandidateIfExist(a);
                        cell.Candidates.Sort();
                        cell.Panel.Refresh();
                        isChanged = true;
                    }
                    else isChanged = false;
                }
        }
    }
}
