using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Structure
{
    public class Table
    {
        private List<List<Cell>> cells;
        private List<Box> boxes;

        public Table()
        {
            cells = new List<List<Cell>>();
            boxes = new List<Box>();
        }

        public Table(Table t)
        {
            cells = t.cells;
            boxes = t.boxes;
        }

        /**
         * Setter / Getter methods
         */
        public List<List<Cell>> Cells
        {
            get { return cells; }
            set { cells = value; }
        }

        public List<Box> Boxes
        {
            get { return boxes; }
            set { boxes = value; }
        }
    }
}
