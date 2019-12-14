using etf.dotsandboxes.cl160127d.Game;

using System;
using System.Collections.Generic;

namespace etf.dotsandboxes.cl160127d.AI
{
    class AICommon
    {
        public static int TryClosingBoxes(List<LineBetweenCircles> existingCanvasLines,
                                          CurrentGame currentGame,
                                          List<Box> boxes,
                                          LineBetweenCircles line,
                                          out int[] surroundingEdges,
                                          bool addToList = false)
        {
            int createdBoxes = 0;

            surroundingEdges = new int[2];
            surroundingEdges[0] = surroundingEdges[1] = 0;

            int coordinateFromX = line.CoordinateFrom.Item1;
            int coordinateFromY = line.CoordinateFrom.Item2;
            int coordinateToX = line.CoordinateTo.Item1;
            int coordinateToY = line.CoordinateTo.Item2;

            if (coordinateFromX == coordinateToX)
            {
                // horizontal line

                for (int direction = 0; direction < 2; direction++)
                {
                    // DIRECTION(0) -> ABOVE
                    // DIRECTION(1) -> BELOW

                    // (line where x = 0 doesn't have upper element
                    if (coordinateFromX == 0 && direction == 0)
                        continue;

                    int dx = (direction == 0 ? -1 : 1); // check for clojure both above and below the line

                    Box box = new Box();

                    bool upperLeft = false;
                    bool upperUpper = false;
                    bool upperRight = false;

                    for (int i = 0; i < existingCanvasLines.Count; i++)
                    {
                        // looking for left and right
                        if (coordinateFromY < coordinateToY)
                        {
                            if (direction == 0)
                            {
                                box.BottomLeft = line.From;
                                box.BottomRight = line.To;
                            }
                            else
                            {
                                box.TopLeft = line.From;
                                box.TopRight = line.To;
                            }

                            // 'from' is left edge
                            if (!upperLeft &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY))))
                            {
                                upperLeft = true;
                                box.LeftEdge = existingCanvasLines[i];

                                surroundingEdges[0]++;

                                continue;
                            }

                            // 'to' is right edge
                            if (!upperRight &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX + dx, coordinateToY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX + dx, coordinateToY))))
                            {
                                upperRight = true;
                                box.RightEdge = existingCanvasLines[i];

                                surroundingEdges[0]++;

                                continue;
                            }

                            if (!upperUpper &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY + 1)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY + 1))))
                            {
                                upperUpper = true;
                                if (direction == 0)
                                    box.UpperEdge = existingCanvasLines[i];
                                else
                                    box.BottomEdge = existingCanvasLines[i];

                                surroundingEdges[0]++;

                                continue;
                            }
                        }
                        else
                        {
                            if (direction == 0)
                            {
                                box.BottomLeft = line.To;
                                box.BottomRight = line.From;
                            }
                            else
                            {
                                box.TopLeft = line.To;
                                box.TopRight = line.From;
                            }

                            // 'from' is right edge
                            if (!upperRight &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY))))
                            {
                                upperRight = true;
                                box.RightEdge = existingCanvasLines[i];

                                surroundingEdges[0]++;

                                continue;
                            }

                            // 'to' is left edge
                            if (!upperLeft &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX + dx, coordinateToY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX + dx, coordinateToY))))
                            {
                                upperLeft = true;
                                box.LeftEdge = existingCanvasLines[i];

                                surroundingEdges[0]++;

                                continue;
                            }

                            if (!upperUpper &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX + dx, coordinateToY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX + dx, coordinateToY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY))))
                            {
                                upperUpper = true;
                                if (direction == 0)
                                    box.UpperEdge = existingCanvasLines[i];
                                else
                                    box.BottomEdge = existingCanvasLines[i];

                                surroundingEdges[0]++;

                                continue;
                            }
                        }

                        if (upperLeft && upperUpper && upperRight)
                            break;
                    }

                    // add to list of boxes
                    if (upperLeft && upperUpper && upperRight)
                    {
                        if (currentGame != null)
                            box.ClosingPlayer = currentGame.Turn;

                        if (direction == 0)
                        {
                            box.BottomEdge = line;

                            box.TopLeft = (box.UpperEdge.From.X < box.UpperEdge.To.X ? box.UpperEdge.From : box.UpperEdge.To);
                            box.TopRight = (box.UpperEdge.From.X > box.UpperEdge.To.X ? box.UpperEdge.From : box.UpperEdge.To);
                        }
                        else
                        {
                            box.UpperEdge = line;
                            box.BottomLeft = (box.BottomEdge.From.X < box.BottomEdge.To.X ? box.BottomEdge.From : box.BottomEdge.To);
                            box.BottomRight = (box.BottomEdge.From.X > box.BottomEdge.To.X ? box.BottomEdge.From : box.BottomEdge.To);
                        }

                        if (addToList)
                            boxes.Add(box);
                        createdBoxes++;
                    }
                }
            }
            else if (coordinateFromY == coordinateToY)
            {
                // vectical line

                for (int direction = 0; direction < 2; direction++)
                {
                    // DIRECTION(0) -> LEFT
                    // DIRECTION(1) -> RIGHT

                    if (coordinateFromY == 0 && direction == 0)
                        continue;

                    int dy = (direction == 0 ? -1 : 1);

                    Box box = new Box();

                    bool upper = false;
                    bool left = false;
                    bool bottom = false;

                    for (int i = 0; i < existingCanvasLines.Count; i++)
                    {
                        if (coordinateFromX < coordinateToX)
                        {
                            if (direction == 0)
                            {
                                box.TopRight = line.From;
                                box.BottomRight = line.To;
                            }
                            else
                            {
                                box.TopLeft = line.From;
                                box.BottomLeft = line.To;
                            }

                            // 'from' is up
                            // 'to' is down
                            if (!bottom &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY))))
                            {
                                bottom = true;
                                box.BottomEdge = existingCanvasLines[i];

                                surroundingEdges[1]++;

                                continue;
                            }

                            if (!upper &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY))))
                            {
                                upper = true;
                                box.UpperEdge = existingCanvasLines[i];

                                surroundingEdges[1]++;

                                continue;
                            }

                            if (!left &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy))))
                            {
                                left = true;
                                if (direction == 0)
                                    box.LeftEdge = existingCanvasLines[i];
                                else
                                    box.RightEdge = existingCanvasLines[i];

                                surroundingEdges[1]++;

                                continue;
                            }
                        }
                        else
                        {
                            if (direction == 0)
                            {
                                box.TopRight = line.To;
                                box.BottomRight = line.From;
                            }
                            else
                            {
                                box.TopLeft = line.To;
                                box.BottomLeft = line.From;
                            }

                            // 'from' is down
                            // 'to' is up
                            if (!bottom &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY))))
                            {
                                bottom = true;
                                box.BottomEdge = existingCanvasLines[i];

                                surroundingEdges[1]++;

                                continue;
                            }

                            if (!upper &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY))))
                            {
                                upper = true;
                                box.UpperEdge = existingCanvasLines[i];

                                surroundingEdges[1]++;

                                continue;
                            }

                            if (!left &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY + dy)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY + dy))))
                            {
                                left = true;
                                if (direction == 0)
                                    box.LeftEdge = existingCanvasLines[i];
                                else
                                    box.RightEdge = existingCanvasLines[i];

                                surroundingEdges[1]++;

                                continue;
                            }
                        }
                    }

                    if (upper && left && bottom)
                    {
                        if (currentGame != null)
                            box.ClosingPlayer = currentGame.Turn;

                        if (direction == 0)
                        {
                            box.RightEdge = line;

                            box.TopLeft = (box.UpperEdge.From.X < box.UpperEdge.To.X ? box.UpperEdge.From : box.UpperEdge.To);
                            box.BottomLeft = (box.BottomEdge.From.X < box.BottomEdge.To.X ? box.BottomEdge.From : box.BottomEdge.To);
                        }
                        else
                        {
                            box.LeftEdge = line;

                            box.TopRight = (box.UpperEdge.From.X > box.UpperEdge.To.X ? box.UpperEdge.From : box.UpperEdge.To);
                            box.BottomRight = (box.BottomEdge.From.X > box.BottomEdge.To.X ? box.BottomEdge.From : box.BottomEdge.To);
                        }

                        if (addToList)
                            boxes.Add(box);
                        createdBoxes++;
                    }
                }
            }
            else
                throw new Exception("Diagonal connections now allowed.");

            return createdBoxes;
        }

        public static LineBetweenCircles FindBoxClosingEdge(List<LineBetweenCircles> existingCanvasLines,
            List<LineBetweenCircles> nonExistingCanvasLines,
            CurrentGame currentGame,
            List<Box> boxes)
        {
            foreach (LineBetweenCircles line in nonExistingCanvasLines)
                if (TryClosingBoxes(existingCanvasLines, currentGame, boxes, line, out int[] notUsed) > 0)
                    return line;

            return null;
        }
    }
}