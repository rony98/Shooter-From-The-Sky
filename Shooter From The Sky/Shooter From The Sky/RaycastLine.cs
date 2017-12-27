using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class RaycastLine
    {
        //The m and b values for the y = mx + b equation
        public double LineM { get; private set; }
        public double LineB { get; private set; }

        //Booleans for whether its a certical line or horizontal line
        public bool VerticalLine { get; private set; }
        public bool HorizontalLine { get; private set; }

        public RaycastLine(bool verticalLine, bool horizontalLine, Vector2 firstPos, Vector2 secondPos)
        {
            //Sets the vertical and horizontal line variables
            VerticalLine = verticalLine;
            HorizontalLine = horizontalLine;

            //If the current line is a horizontal line, calculate it's equation
            if (horizontalLine)
            {
                LineM = 0;
                LineB = firstPos.Y;
            }
            //If the current line is a vertical line, calculate it's equation
            else if (verticalLine)
            {
                LineM = double.PositiveInfinity;
                LineB = firstPos.X;
            }
            //If it's neither vertical or horizontal, calculate it's equation
            else
            {
                LineM = (secondPos.Y - firstPos.Y) / (secondPos.X - firstPos.X);
                LineB = firstPos.Y - (LineM * firstPos.X);
            }
        }
    }
}
