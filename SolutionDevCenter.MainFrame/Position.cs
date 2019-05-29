using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDevCenter.MainFrame
{
    public class Position
    {
        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }
        public int Column { get; private set; }
        public int Row { get; private set; }
    }
}
