using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace etf.dotsandboxes.cl160127d.Game
{
    class Logic
    {
        public static void CalculateCanvasParameters(int tableSizeX, 
                                                     int tableSizeY, 
                                                     int canvasWidth,
                                                     int canvasHeight,
                                                     out int horizontalSpacingBetweenCircles,
                                                     out int verticalSpacingBetweenCircles,
                                                     out int circleDiameter)
        {
            if (tableSizeX <= 4 && tableSizeY <= 4)
                circleDiameter = 35;
            else if ((tableSizeX > 4 && tableSizeY > 4) && (tableSizeX < 10 && tableSizeY < 10))
                circleDiameter = 25;
            else if (tableSizeX >= 10 && tableSizeY >= 10)
                circleDiameter = 15;
            else
                circleDiameter = 25;

            horizontalSpacingBetweenCircles = (canvasWidth - tableSizeY * circleDiameter) / (tableSizeY + 1);
            verticalSpacingBetweenCircles = (canvasHeight - tableSizeX * circleDiameter) / (tableSizeX + 1);
        }
    }
}
