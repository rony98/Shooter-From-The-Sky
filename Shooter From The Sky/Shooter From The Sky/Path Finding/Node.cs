/*
 * Author: Rony Verch
 * File Name: Node.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 23, 2015
 * Modified Date: January 20, 2015
 * Description: A node that is used with the pathfinding class to calculate a path for the enemies to follow.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class Node
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Properties for the F, G, and H values for the current node
        public int FValue { get; private set; }
        public int GValue { get; private set; }
        public int HValue { get; private set; }

        //Property for the parent node of the current node
        public Node ParentNode { get; set; }

        //Properties for the row and column of the current node
        public int Row { get; private set; }
        public int Column { get; private set; }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTORS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The row and column of the node
        //Post: The Node is created without a value for the parent node
        //Desc: A constructor for the Node that does not take a value for the parent node
        public Node(int row, int column)
        {
            //Sets the row and column
            Row = row;
            Column = column;

            //Sets the parent node to null
            ParentNode = null;
        }


        //Pre: The row, column, and the parent node of the current node
        //Post: The Node is created with a row, column, and a parent node
        //Desc: A constructor for the Node that takes in a value parent node as well and sets it
        public Node(int row, int column, Node parentNode)
        {
            //Sets the row, column, and parent node
            Row = row;
            Column = column;
            ParentNode = parentNode;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The goal node
        //Post: The F, G, and H values are calculated using the parent node and goal node
        //Desc: A method which calculates the F, G, and H values using the parent node, and goal node
        public void CalculateValues(Node goalNode)
        {
            //If the current node is diagonal from the parent
            if (Math.Abs(Row - ParentNode.Row) == 1 && Math.Abs(Column - ParentNode.Column) == 1)
            {
                //Calculates the G Value
                GValue = ParentNode.GValue + 14;
            }
            //If the current node is not diagonal from the parent
            else
            {
                //Calculates the G Value
                GValue = ParentNode.GValue + 10;
            }

            //Calculates the H value using the distance from the current node to the goal node
            HValue = Math.Abs(Row - goalNode.Row) + Math.Abs(Column - goalNode.Column);

            //Calculates the F value by adding the G and H values together
            FValue = GValue + HValue;
        }
    }
}
