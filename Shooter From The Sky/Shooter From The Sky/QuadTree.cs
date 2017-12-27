using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class QuadTree
    {
        private const int MAX_OBJECTS = 4;
        private const int MAX_LEVELS = 4;

        private int level;
        private List<Projectile> objects;
        private Rectangle bounds;
        private QuadTree[] nodes;

        public QuadTree(int level, Rectangle bounds)
        {
            this.level = level;
            objects = new List<Projectile>();
            this.bounds = bounds;
            nodes = new QuadTree[4];
        }

        public void Clear()
        {
            objects.Clear();

            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }

        private void Split()
        {
            int subWidth = bounds.Width / 2;
            int subHeight = bounds.Height / 2;
            int x = bounds.X;
            int y = bounds.Y;

            nodes[0] = new QuadTree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new QuadTree(level + 1, new Rectangle(x, y, subWidth, subHeight));
            nodes[2] = new QuadTree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new QuadTree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        private List<int> GetIndex(Projectile tempProjectile, Human tempHuman)
        {
            List<int> indexes = new List<int>();

            Rectangle objectBounds = new Rectangle();

            if (tempHuman == null)
            {
                objectBounds = tempProjectile.GetBounds();
            }
            else
            {
                objectBounds = tempHuman.GetBounds();
            }

            double verticalMidpoint = bounds.X + (bounds.Width / 2);
            double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

            bool topQuadrant = objectBounds.Y <= horizontalMidpoint;
            bool bottomQuadrant = (objectBounds.Y + objectBounds.Height) >= horizontalMidpoint;
            bool topAndBottomQuadrant = topQuadrant && bottomQuadrant;

            if (topAndBottomQuadrant)
            {
                topQuadrant = false;
                bottomQuadrant = false;
            }

            // Check if the object is in left and right quadrant
            if (objectBounds.X <= verticalMidpoint && objectBounds.X + objectBounds.Width >= verticalMidpoint)
            {
                if (topQuadrant)
                {
                    indexes.Add(0);
                    indexes.Add(1);
                }
                else if (bottomQuadrant)
                {
                    indexes.Add(2);
                    indexes.Add(3);
                }
                else if (topAndBottomQuadrant)
                {
                    indexes.Add(0);
                    indexes.Add(1);
                    indexes.Add(2);
                    indexes.Add(3);
                }
            }
            // Check if the object is only in the right quadrant
            else if (objectBounds.X >= verticalMidpoint && objectBounds.X + objectBounds.Width >= verticalMidpoint)
            {
                if (topQuadrant)
                {
                    indexes.Add(0);
                }
                else if (bottomQuadrant)
                {
                    indexes.Add(3);
                }
                else if (topAndBottomQuadrant)
                {
                    indexes.Add(0);
                    indexes.Add(3);
                }
            }
            // Check if the object is only in the left quadrant
            else if (objectBounds.X <= verticalMidpoint && objectBounds.X + objectBounds.Width <= verticalMidpoint)
            {
                if (topQuadrant)
                {
                    indexes.Add(1);
                }
                else if (bottomQuadrant)
                {
                    indexes.Add(2);
                }
                else if (topAndBottomQuadrant)
                {
                    indexes.Add(1);
                    indexes.Add(2);
                }
            }
            else
            {
                indexes.Add(-1);
            }

            return indexes;
        }

        public void Insert(Projectile tempObject)
        {
            if (nodes[0] != null)
            {
                List<int> indexes = GetIndex(tempObject, null);

                for (int i = 0; i < indexes.Count; i++)
                {
                    if (indexes[i] != -1)
                    {
                        nodes[indexes[i]].Insert(tempObject);
                        return;
                    }
                }
            }

            objects.Add(tempObject);

            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    Split();
                }

                int i = 0;

                while (i < objects.Count)
                {
                    List<int> indexes = GetIndex(objects[i], null);
                    Projectile currObject = objects[i];

                    for (int j = 0; j < indexes.Count; j++)
                    {
                        if (indexes[j] != -1)
                        {
                            nodes[indexes[j]].Insert(currObject);
                            objects.Remove(currObject);
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
            }
        }

        public List<Projectile> Retrieve(List<Projectile> returnObjects, Human tempObject)
        {
            List<int> indexes = GetIndex(null, tempObject);

            for (int i = 0; i < indexes.Count; i++)
            {
                if (indexes[i] != -1 && nodes[0] != null)
                {
                    nodes[indexes[i]].Retrieve(returnObjects, tempObject);
                }

                returnObjects.AddRange(objects);
            }

            return returnObjects;
        }

        public void DrawNodes(SpriteBatch sb, Texture2D blankTexture)
        {
            if (nodes[0] != null)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i] != null)
                    {
                        nodes[i].DrawNodes(sb, blankTexture);
                    }
                }
            }
            else
            {
                sb.Draw(blankTexture, new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2), Color.White);
                sb.Draw(blankTexture, new Rectangle(bounds.X, bounds.Y, bounds.Width, 1), Color.Black);
                sb.Draw(blankTexture, new Rectangle(bounds.X, bounds.Y, 1, bounds.Height), Color.Black);
                sb.Draw(blankTexture, new Rectangle(bounds.X + bounds.Width - 1, bounds.Y, 1, bounds.Height), Color.Black);
                sb.Draw(blankTexture, new Rectangle(bounds.X, bounds.Y + bounds.Height - 1, bounds.Width, 1), Color.Black);
            }
        }
    }
}
