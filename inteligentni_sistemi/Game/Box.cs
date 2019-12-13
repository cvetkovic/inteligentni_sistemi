using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace etf.dotsandboxes.cl160127d.Game
{
    class Box
    {
        public LineBetweenCircles UpperEdge { get; set; }
        public LineBetweenCircles BottomEdge { get; set; }
        public LineBetweenCircles LeftEdge { get; set; }
        public LineBetweenCircles RightEdge { get; set; }

        public Player ClosingPlayer { get; set; }

        public Point TopLeft { get; set; }
        public Point TopRight { get; set; }
        public Point BottomLeft { get; set; }
        public Point BottomRight { get; set; }
    }
}