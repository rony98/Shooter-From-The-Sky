/*
 * Author: Rony Verch
 * File Name: QuadTree.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 20, 2015
 * Modified Date: January 20, 2015
 * Description: Quadtree class for collision detection
 */

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
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constants for the maximum amount of objects in one node of the quadtree and the maximum levels the quad tree can split into
        private const int MAX_OBJECTS = 4;
        private const int MAX_LEVELS = 4;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Integer for the current level
        private int level;

        //List of projectiles that exist within the current node
        private List<Projectile> objects;

        //The bounds of the current node and the nodes within this node
        private Rectangle bounds;
        private QuadTree[] nodes;


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The level of the current node and the bounds for the node
        //Post: A Quadtree node is created 
        //Desc: A constructor for the current node of the quad tree
        public QuadTree(int level, Rectangle bounds)
        {
            //Sets the level and bounds
            this.level = level;
            this.bounds = bounds;

            //Creates a new list for the objects within the node
            objects = new List<Projectile>();

            //Creates an array of 4 nodes that are within the current node
            nodes = new QuadTree[4];
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: The objects within the quad tree are completely cleared and all the nodes are cleared as well
        //Desc: A method which clears all the objects within the quadtree and resets all the nodes
        public void Clear()
        {
            //Clears the objects from this node
            objects.Clear();

            //Loop for every node within this node
            for (int i = 0; i < nodes.Length; i++)
            {
                //If the current node is not null
                if (nodes[i] != null)
                {
                    //Clears the node and sets it to null
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }


        //Pre: None
        //Post: The current node is split
        //Desc: A method which splits the quadtree node
        private void Split()
        {
            //Calculates the width, height, x and y of the split nodes
            int subWidth = bounds.Width / 2;
            int subHeight = bounds.Height / 2;
            int x = bounds.X;
            int y = bounds.Y;

            //Creates 4 new nodes using the width, height, x and y positions that were calculated
            nodes[0] = new QuadTree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new QuadTree(level + 1, new Rectangle(x, y, subWidth, subHeight));
            nodes[2] = new QuadTree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new QuadTree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }


        //Pre: The projectile and human that could be checked
        //Post: The indexes for the current projectile or human are calculated
        //Desc: A method which calculates the node index's of the projecitle or human
        private List<int> GetIndex(Projectile tempProjectile, Human tempHuman)
        {
            //List of indexes
            List<int> indexes = new List<int>();

            //Rectangle for the bounds of the object
            Rectangle objectBounds = new Rectangle();

            //If the object is not a human
            if (tempHuman == null)
            {
                //Sets the bounds of the projectile
                objectBounds = tempProjectile.GetBounds();
            }
            //If the object is a human
            else
            {
                //Sets teh bounds of the human
                objectBounds = tempHuman.GetBounds();
            }

            //Calculates the midpoints of the node
            double verticalMidpoint = bounds.X + (bounds.Width / 2);
            double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

            //Calculates which quadrants the object that is being checked is in
            bool topQuadrant = objectBounds.Y <= horizontalMidpoint;
            bool bottomQuadrant = (objectBounds.Y + objectBounds.Height) >= horizontalMidpoint;
            bool topAndBottomQuadrant = topQuadrant && bottomQuadrant;

            //If the object is in both the top and bottom quadrant
            if (topAndBottomQuadrant)
            {
                //Sets the seperate top and bottom booleans to false
                topQuadrant = false;
                bottomQuadrant = false;
            }

            //If the object is in left and right quadrant
            if (objectBounds.X <= verticalMidpoint && objectBounds.X + objectBounds.Width >= verticalMidpoint)
            {
                //If the object is in the top quadrant, adds the corresponding indexes
                if (topQuadrant)
                {
                    indexes.Add(0);
                    indexes.Add(1);
                }
                //If the object is in the bottom quadrant, adds the corresponding indexes
                else if (bottomQuadrant)
                {
                    indexes.Add(2);
                    indexes.Add(3);
                }
                //If the object is in the top and bottom quadrant, adds the corresponding indexes
                else if (topAndBottomQuadrant)
                {
                    indexes.Add(0);
                    indexes.Add(1);
                    indexes.Add(2);
                    indexes.Add(3);
                }
            }
            //If the object is only in the right quadrant
            else if (objectBounds.X >= verticalMidpoint && objectBounds.X + objectBounds.Width >= verticalMidpoint)
            {
                //If the object is in the top quadrant, adds the corresponding indexes
                if (topQuadrant)
                {
                    indexes.Add(0);
                }
                //If the object is in the bottom quadrant, adds the corresponding indexes
                else if (bottomQuadrant)
                {
                    indexes.Add(3);
                }
                //If the object is in the top and bottom quadrant, adds the corresponding indexes
                else if (topAndBottomQuadrant)
                {
                    indexes.Add(0);
                    indexes.Add(3);
                }
            }
            //If the object is only in the left quadrant
            else if (objectBounds.X <= verticalMidpoint && objectBounds.X + objectBounds.Width <= verticalMidpoint)
            {
                //If the object is in the top quadrant, adds the corresponding indexes
                if (topQuadrant)
                {
                    indexes.Add(1);
                }
                //If the object is in the bottom quadrant, adds the corresponding indexes
                else if (bottomQuadrant)
                {
                    indexes.Add(2);
                }
                //If the object is in the top and bottom quadrant, adds the corresponding indexes
                else if (topAndBottomQuadrant)
                {
                    indexes.Add(1);
                    indexes.Add(2);
                }
            }
            //If the object is not in any quadrant
            else
            {
                //Sets the index to -1 which is an impossible quadrant index
                indexes.Add(-1);
            }

            //Returns the indexes
            return indexes;
        }


        //Pre: The current projectile
        //Post: The projectile is inserted into the quad tree
        //Desc: A method which inserts the projectile into the quadtree where it needs to be
        public void Insert(Projectile tempObject)
        {
            //If the current node has nodes inside of it
            if (nodes[0] != null)
            {
                //Gets the indexes the object is in
                List<int> indexes = GetIndex(tempObject, null);

                //Loop for every index
                for (int i = 0; i < indexes.Count; i++)
                {
                    //If the index is a valid index
                    if (indexes[i] != -1)
                    {
                        //Inserts the object into the current node
                        nodes[indexes[i]].Insert(tempObject);
                        return;
                    }
                }
            }

            //Adds the object into the current node
            objects.Add(tempObject);

            //If the amount of objects in the node is greater then the max objects and the current level is smaller then the max level
            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
            {
                //If the node hasn't been split yet
                if (nodes[0] == null)
                {
                    //Splits the node
                    Split();
                }

                //Temp variable for a count
                int count = 0;

                //While the count is less then the amount of objects
                while (count < objects.Count)
                {
                    //Gets the indexes of the current object and sets the current object to a variable
                    List<int> indexes = GetIndex(objects[count], null);
                    Projectile currObject = objects[count];

                    //Loop for every index
                    for (int j = 0; j < indexes.Count; j++)
                    {
                        //If the current index is valid
                        if (indexes[j] != -1)
                        {
                            //Inserts the object into the node and removes it from the list of objects in the current node
                            nodes[indexes[j]].Insert(currObject);
                            objects.Remove(currObject);
                        }
                        //If the current index is invalid, adds to the count
                        else
                        {
                            count++;
                        }
                    }
                }
            }
        }


        //Pre: The list of objects that need to be returned and the human that is being checked
        //Post: A list of objects in the node that the human that is being checked is in is returned
        //Desc: A method which returns a list of objects which correspond to the same node that a human is in
        public List<Projectile> Retrieve(List<Projectile> returnObjects, Human tempObject)
        {
            //Gets the indexes of the human
            List<int> indexes = GetIndex(null, tempObject);

            //Loop for every index of the human
            for (int i = 0; i < indexes.Count; i++)
            {
                //If the index is valid and the current node is not null
                if (indexes[i] != -1 && nodes[0] != null)
                {
                    //Retrieves the objects from the new node
                    nodes[indexes[i]].Retrieve(returnObjects, tempObject);
                }

                //Adds the current objects to the list of objects
                returnObjects.AddRange(objects);
            }

            //Returns the list of objects
            return returnObjects;
        }
    }
}
