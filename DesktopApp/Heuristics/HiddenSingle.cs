using DesktopApp.Structure;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Heuristics
{
    class HiddenSingle : Heuristic
    {

        public HiddenSingle(Table t)
        {
            this.table = t;
        }

        public override Boolean Apply()
        {
            isChanged = false;

            for (int num = 0; num < 9; num++)
            {
                InnerHiddenSingle(TableSearch.GetColumnByIndex(num, table.Cells).ToList());
                InnerHiddenSingle(TableSearch.GetRowByIndex(num, table.Cells).ToList());
            }

            foreach (var box in table.Boxes)
                InnerHiddenSingle(box.Cells);

            return isChanged;
        }

        private void InnerHiddenSingle(List<Cell> cells)
        {
            List<Cell> onlyOneCell = new List<Cell>();
            for (int i = 0; i < 9; i++) onlyOneCell.Add(new Cell());

            int[] occurred = new int[9];

            foreach (var cell in cells)
                if (cell.Value == 0)
                    foreach (var cand in cell.Candidates)
                    {
                        if (occurred[cand - 1] == 0) onlyOneCell[cand - 1] = cell;
                        occurred[cand - 1]++;
                    }

            for (int i = 0; i < 9; i++)
                if (occurred[i] == 1)
                {
                    Cell cell = table.Cells.ElementAt(onlyOneCell[i].Y).ElementAt(onlyOneCell[i].X);
                    cell.Candidates.Clear();
                    cell.Value = i + 1;
                    BasicStep(cell);
                    cell.Panel.ChangeCellValue(i + 1);
                    isChanged = true;
                }
        }

    }
}
