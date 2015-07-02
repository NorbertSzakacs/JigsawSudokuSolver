using DesktopApp.Structure;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Heuristics
{
    class XWing : Heuristic
    {
        public XWing(Table t)
        {
            this.table = t;
        }

        public override Boolean Apply()
        {
            isChanged = false;

            List<List<List<int>>> only2Rows = new List<List<List<int>>>();
            for (int row = 0; row < 9; row++)
            {   
                // [x,0] cand >>>>>VALUE<<<<<, [x,1] first column num [x,2] second column num
                only2Rows.Add(InnerOnly2XWing(TableSearch.GetRowByIndex(row, table.Cells).ToList()));
            }
            // cand_Data: [x.0] cand count, [x.1] first column, [x.2] second column, [x.3] first cand's row number, [x.4] second cand's row number
            int[,] candData = new int[9, 5];
            List<int> candGood = new List<int> ();

            int rowI = 0;
            foreach (var rowGoodCands in only2Rows)
            {
                foreach (var candNumbers in rowGoodCands)
                {
                    if (candNumbers.Count > 0)
                    {
                        int candValue = candNumbers.ElementAt(0);
                        // ha (először) előfordul
                        if (candData[candValue, 0] == 0)
                        {   //mentem az oszlop & sor értékeket
                            candData[candValue, 1] = candNumbers.ElementAt(1);
                            candData[candValue, 2] = candNumbers.ElementAt(2);
                            candData[candValue, 3] = rowI;
                        }
                        //ha másodszor is
                        if (candData[candValue, 0] == 1)
                        {
                            //ha jó oszlopban vannak az értékek
                            if ((candData[candValue, 1] == candNumbers.ElementAt(1)) && (candData[candValue, 2] == candNumbers.ElementAt(2)))
                            {
                                candGood.Add(candValue);
                                candData[candValue, 4] = rowI;
                            }
                        }
                        candData[candValue, 0]++;
                    }
                }
                rowI++;
            }

            for (int i = 0; i < 9; i++)
                if ((candData[i, 0] > 1) && (candGood.Contains(i + 1))) candGood.Remove(i + 1);

            for (int i = 0; i < 9; i++)
            {   // cand_Data: [x.0] cand count, [x.1] first column, [x.2] second column, [x.3] first cand's row number, [x.4] second cand's row number
                if (candGood.Contains(i + 1))
                {
                    InnerCoreXWing(TableSearch.GetColumnByIndex(candData[i, 1], table.Cells).ToList(), i + 1, candData[i, 3], candData[i, 4]);
                    InnerCoreXWing(TableSearch.GetColumnByIndex(candData[i, 2], table.Cells).ToList(), i + 1, candData[i, 3], candData[i, 4]);
                }
            }

            return isChanged;
        }


        private List<List<int>> InnerOnly2XWing(List<Cell> cells)
        {
            // [x,0] cand count [x,1] first column num [x,2] second column num
            int[,] candData = new int[9, 3];
            List<List<int>> only2InRow = new List<List<int>>();

            foreach (var cell in cells)
                if (cell.Value == 0)
                    foreach (var cand in cell.Candidates)
                    {
                        if (candData[cand - 1, 0] == 0) candData[cand - 1, 1] = cell.Y;
                        if (candData[cand - 1, 0] == 1) candData[cand - 1, 2] = cell.Y;
                        candData[cand - 1, 0]++;
                    }

            for (int i = 0; i < 9; i++)
            {
                List<int> candRow = new List<int>();
                if (candData[i, 0] == 2)
                {
                    candRow.Add(i);
                    for (int j = 1; j < 3; j++) candRow.Add(candData[i, j]);
                }
                only2InRow.Add(candRow);
            }
            return only2InRow;
        }

        private void InnerCoreXWing(List<Cell> cells, int cand, int expect1, int expect2)
        {
            foreach (var cell in cells)
                if ((cell.Value == 0) && (cell.Candidates.Contains(cand)) && (cell.X != expect1) && (cell.X != expect2))
                {
                    cell.RemoveCandidateIfExist(cand);
                    cell.Candidates.Sort();
                    cell.Panel.Refresh();
                    isChanged = true;
                }
        }
    }
}
