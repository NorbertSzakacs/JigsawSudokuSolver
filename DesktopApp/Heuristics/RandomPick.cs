using DesktopApp.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Heuristics
{
    class RandomPick : Heuristic
    {
        public RandomPick(Table t)
        {
            this.table = t;
        }

        public override Boolean Apply()
        {
            int min = 10;
            Cell minCand = null;
            foreach (var column in table.Cells)
                foreach (var cell in column)
                    if (cell.Candidates.Count < min && cell.Candidates.Count > 0)
                    {
                        min = cell.Candidates.Count;
                        minCand = cell;
                    }

            if (minCand != null)
            {
                Random rand = new Random();
                int id = rand.Next(minCand.Candidates.Count);
                minCand.Value = minCand.Candidates[id];
                minCand.Panel.ChangeCellValue(minCand.Candidates[id]);
                minCand.Candidates.Clear();
                BasicStep(minCand);
            }

            return true;
        }

    }
}
