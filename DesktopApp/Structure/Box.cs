using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Structure
{
    public class Box
    {
        private List<Cell> cells;

        public Box()
        {
            cells = new List<Cell>();
        }

        public List<Cell> Cells
        {
            get { return cells; }
            set { cells = value; }
        }
    }
}
