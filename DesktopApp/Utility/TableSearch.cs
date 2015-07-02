using DesktopApp.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Utility
{
    public static class TableSearch
    {
        public static IEnumerable<Cell> GetRowOfCell(Cell cell, List<List<Cell>> cells)
        {
            return GetRowByIndex(cell.Y, cells);
        }

        public static IEnumerable<Cell> GetColumnOfCell(Cell cell, List<List<Cell>> cells)
        {
            return GetColumnByIndex(cell.X, cells);
        }

        public static IEnumerable<Cell> GetColumnByIndex(int column, List<List<Cell>> cells)
        {
            return cells.Select(innerList => innerList[column]);
        }

        public static IEnumerable<Cell> GetRowByIndex(int row, List<List<Cell>> cells)
        {
            return cells.ElementAt(row);
        }
    }
}
