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
    }
}