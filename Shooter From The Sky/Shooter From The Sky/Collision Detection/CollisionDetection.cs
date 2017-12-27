/*
 * Author: Rony Verch
 * File Name: CollisionDetection.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 21, 2015
 * Modified Date: January 20, 2015
 * Description: Collision detection class that uses both bounding box collision and pixel perfect
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class CollisionDetection
    {
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The two rectangles that have to be checked for collision and the textures that correspond to them
        //Post: A boolean is returned for whether the two objects that are being checked are colliding
        //Desc: A method for checking whether two objects are colliding to each other
        static public bool Collision(Rectangle firstRect, Rectangle secondRect, Texture2D firstTexture, Texture2D secondTexture)
        {
            //If the two rectangles are colliding with each other
            if (firstRect.X < secondRect.X + secondRect.Width && firstRect.X + firstRect.Width > secondRect.X && firstRect.Y < secondRect.Y + secondRect.Height && firstRect.Y + firstRect.Height > secondRect.Y)
            {
                //Check whether they are colliding with pixel perfect and return that
                return PixelPerfect(firstRect, secondRect, firstTexture, secondTexture);
            }

            //Return false if there was no collision
            return false;
        }


        //Pre: The two rectangles that have to be checked for collision and the texture that correspond to them
        //Post: A boolean is returned for whether the two objects that are being checked are colliding with pixel perfect collision
        //Desc: A method for checking whether two objects are colliding with pixel perfect percision
        static private bool PixelPerfect(Rectangle firstRect, Rectangle secondRect, Texture2D firstTexture, Texture2D secondTexture)
        {
            //Sets the first colour array
            Color[] firstColours = new Color[firstRect.Width * firstRect.Height];
            firstTexture.GetData(firstColours);

            //Sets the second colours array
            Color[] secondColours = new Color[secondRect.Width * secondRect.Height];
            secondTexture.GetData(secondColours);

            //Calculates the rectangle that represents the points where the two rectangles are intersecting at
            int x1 = Math.Max(firstRect.X, secondRect.X);
            int x2 = Math.Min(firstRect.X + firstRect.Width, secondRect.X + secondRect.Width);
            int y1 = Math.Max(firstRect.Y, secondRect.Y);
            int y2 = Math.Min(firstRect.Y + firstRect.Height, secondRect.Y + secondRect.Height);

            //Creates two colours that will be both be checked
            Color firstColour;
            Color secondColour;

            //Loop for all the x's of the intersecting rectangle
            for (int x = x1; x < x2; x++)
            {
                //Loop for all the y's of the intersecting rectangle
                for (int y = y1; y < y2; y++)
                {
                    //Gets the first and second colour that needs to be checked
                    firstColour = firstColours[(x - firstRect.X) + (y - firstRect.Y) * firstRect.Width];
                    secondColour = secondColours[(x - secondRect.X) + (y - secondRect.Y) * secondRect.Width];

                    //If both the colours are not transparent, return that a pixel perfect collision was found
                    if (firstColour.A != 0 && secondColour.A != 0)
                    {
                        return true;
                    }
                }
            }

            //Returns that a pixel perfect collision was not found
            return false;
        }
    }
}
