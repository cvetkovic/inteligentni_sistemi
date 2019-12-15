using etf.dotsandboxes.cl160127d.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace etf.dotsandboxes.cl160127d.Game
{
    class LineBetweenCircles
    {
        public Point From { get; set; }
        public Point To { get; set; }

        public VTuple<int, int> CoordinateFrom { get; set; }
        public VTuple<int, int> CoordinateTo { get; set; }

        public Player WhoDrew { get; set; }

        public override string ToString()
        {
            string s = "";

            if (From.X == To.X) // vertical line
                s = string.Format("{0}{1}", Miscellaneous.TranslateAxisToLetter(CoordinateFrom.Item1 < CoordinateTo.Item1 ? CoordinateFrom.Item1 : CoordinateTo.Item1), (CoordinateFrom.Item2 < CoordinateTo.Item2 ? CoordinateFrom.Item2 : CoordinateTo.Item2));
            else                        // horizontal line
                s = string.Format("{0}{1}", (CoordinateFrom.Item1 < CoordinateTo.Item1 ? CoordinateFrom.Item1 : CoordinateTo.Item1), Miscellaneous.TranslateAxisToLetter(CoordinateFrom.Item2 < CoordinateTo.Item2 ? CoordinateFrom.Item2 : CoordinateTo.Item2));

            return s;
        }
    }
}