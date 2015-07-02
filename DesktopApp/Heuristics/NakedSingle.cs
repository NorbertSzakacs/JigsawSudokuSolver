using DesktopApp.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Heuristics
{
    class NakedSingle : Heuristic
    {
        public NakedSingle(Table t)
        {
            this.table = t;
        }

        /**
         * If only one candidate exist, then make it for the cell value, and call BasicStep
         */
        public override Boolean Apply()
        {
            isChanged = false;

            foreach (var column in table.Cells)
                foreach (var cell in column)
                    if ((cell.Candidates.Count == 1) && (cell.Value == 0))
                    {
                        cell.Value = cell.Candidates.First();
                        cell.Panel.ChangeCellValue(cell.Candidates.First());
                        cell.Candidates.Clear();
                        BasicStep(cell);
                        isChanged = true;
                    }

            return isChanged;
        }
    }
}
