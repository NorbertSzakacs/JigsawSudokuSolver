using DesktopApp.Structure;
using DesktopApp.Utility;
using DesktopApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Heuristics
{
    public abstract class Heuristic
    {
        protected Table table;
        protected CellPanel cellpanels;
        protected Boolean isChanged;
        protected Boolean isEnabled;


        public Heuristic()
        {
            isEnabled = true;
        }

        public Heuristic(Table t)
        {
            this.table = t;
        }

        abstract public Boolean Apply();


        /**
         * Erase the new cell value from the same Column, Row and Box
         */
        protected void BasicStep(Cell cell)
        {
            InnerBasicStep(TableSearch.GetColumnByIndex(cell.X, table.Cells).ToList(), cell.Value);
            InnerBasicStep(TableSearch.GetRowByIndex(cell.Y, table.Cells).ToList(), cell.Value);
            InnerBasicStep(cell.Box.Cells, cell.Value);
        }

        private void InnerBasicStep(List<Cell> cells, int cand)
        {
            foreach (var cell in cells)
                if (cell.Value != cand)
                {
                    cell.RemoveCandidateIfExist(cand);
                    cell.Candidates.Sort();
                    cell.Panel.Refresh();
                }
                else
                {
                    cell.Candidates.Clear();
                    cell.Panel.Refresh();
                }
        }

        public Boolean Enabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }
       
    }
}
