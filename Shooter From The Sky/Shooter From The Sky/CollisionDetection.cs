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
        public CollisionDetection()
        {
        }

        public bool Collision(Rectangle firstRect, Rectangle secondRect, Texture2D firstTexture, Texture2D secondTexture)
        {
            if (firstRect.X < secondRect.X + secondRect.Width && firstRect.X + firstRect.Width > secondRect.X && firstRect.Y < secondRect.Y + secondRect.Height && firstRect.Y + firstRect.Height > secondRect.Y)
            {
                return PixelPerfect(firstRect, secondRect, firstTexture, secondTexture);
            }

            return false;
        }

        private bool PixelPerfect(Rectangle firstRect, Rectangle secondRect, Texture2D firstTexture, Texture2D secondTexture)
        {
            Color[] firstColours = new Color[firstRect.Width * firstRect.Height];
            firstTexture.GetData(firstColours);

            Color[] secondColours = new Color[secondRect.Width * secondRect.Height];
            secondTexture.GetData(secondColours);

            int x1 = Math.Max(firstRect.X, secondRect.X);
            int x2 = Math.Min(firstRect.X + firstRect.Width, secondRect.X + secondRect.Width);

            int y1 = Math.Max(firstRect.Y, secondRect.Y);
            int y2 = Math.Min(firstRect.Y + firstRect.Height, secondRect.Y + secondRect.Height);

            Color firstColour;
            Color secondColour;

            for (int x = x1; x < x2; x++)
            {
                for (int y = y1; y < y2; y++)
                {
                    firstColour = firstColours[(x - firstRect.X) + (y - firstRect.Y) * firstRect.Width];
                    secondColour = secondColours[(x - secondRect.X) + (y - secondRect.Y) * secondRect.Width];

                    if (firstColour.A != 0 && secondColour.A != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
