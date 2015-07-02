using DesktopApp.Structure;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Heuristics
{
    class BoxLineReduction : Heuristic
    {

        public BoxLineReduction(Table t)
        {
            this.table = t;
        }

        public override Boolean Apply()
        {
            isChanged = false;

            for (int i = 0; i < 9; i++)
            {
                InnerShellBoxLineReduction(TableSearch.GetColumnByIndex(i, table.Cells).ToList());
                InnerShellBoxLineReduction(TableSearch.GetRowByIndex(i, table.Cells).ToList());
            }

            return isChanged;
        }

        public void InnerShellBoxLineReduction(List<Cell> cells)
        {
            bool[] matchBool = new bool[9];
            for (int i = 0; i < 9; i++) matchBool[i] = true;
            List<Box> candPlace = new List<Box>();
            for (int i = 0; i < 9; i++)
            {
                Box temp = new Box();
                candPlace.Add(temp);
            }
            int[] candCount = new int[9];

            foreach (var cell in cells)
                if (cell.Value == 0)
                    foreach (var cand in cell.Candidates)
                    {
                        if (candCount[cand - 1] == 0)
                        {
                            candPlace[cand - 1] = cell.Box;
                        }
                        else
                        {
                            if (!candPlace[cand - 1].Equals(cell.Box)) matchBool[cand - 1] = false;
                        }
                        candCount[cand - 1]++;
                    }

            for (int i = 0; i < 9; i++)
                if (candCount[i] < 2) matchBool[i] = false;

            for (int i = 0; i < 9; i++)
                if (matchBool[i])
                    InnerCoreBoxLineReduction(candPlace[i].Cells, i + 1, cells);
        }

        private void InnerCoreBoxLineReduction(List<Cell> cells, int candidate, List<Cell> expect)
        {
            foreach (var cell in cells)
                if (!expect.Contains(cell) && (cell.Value == 0))
                {
                    if (cell.Candidates.Count > 0 && cell.Candidates.Contains(candidate))
                    {
                        cell.RemoveCandidateIfExist(candidate);
                        cell.Candidates.Sort();
                        cell.Panel.Refresh();
                        isChanged = true;
                    }
                    else isChanged = false;
                }
        }
    }
}
