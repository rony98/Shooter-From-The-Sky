/*
 * Author: Rony Verch
 * File Name: RaycastLine.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 29, 2015
 * Modified Date: January 20, 2015
 * Description: A class for a line which is used for raycasting to check intersections between multiple different lines. This intersections can be used to check whether one object would
 *              have clear view to another object (an enemy having clear view to a player).
 */

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class RaycastLine
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //The m and b values for the y = mx + b equation
        public double LineM { get; private set; }
        public double LineB { get; private set; }

        //Booleans for whether its a certical line or horizontal line
        public bool VerticalLine { get; private set; }
        public bool HorizontalLine { get; private set; }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: A booleans for whether the line is a vertical line, or the line is a horizontal line, the first position of the line, and the second position of the line
        //Post: The line is created for the raycasting
        //Desc: A constructor for creating the line used for raycasting
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
