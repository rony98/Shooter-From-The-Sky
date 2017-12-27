/*
 * Author: Rony Verch
 * File Name: PathFinding.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 23, 2015
 * Modified Date: January 20, 2015
 * Description: The pathfinding clas which uses an A* algorithm to calculate the shortest path from point A to point B which an enemy can follow
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class PathFinding
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //The open, closed and solution list for the path finding
        private List<Node> openList = new List<Node>();
        private List<Node> closeList = new List<Node>();
        private List<Node> solutionList = new List<Node>();

        //Array for all of the tiles for the game
        private Tile[,] gameTiles;

        //List for the nodes that surround the current node that is being checked
        private List<Node> nodesAround = new List<Node>();

        //Whether a node was found in the open list
        private Node nodeFoundOpen = null;
        private int indexNodeFoundOpen = -1;

        //Wether a node was found in the closed list
        private Node nodeFoundClose = null;
        private int indexNodeFoundClose = -1;


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The game tiles
        //Post: The Pathfinding object is created
        //Desc: A constructor for the pathfinding object which only takes in the game tiles
        public PathFinding(Tile[,] gameTiles)
        {
            //Sets the game tiles
            this.gameTiles = gameTiles;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The solution list
        //Post: The solution list is returned
        //Desc: A method which returns the solution list 
        public List<Node> GetSolution()
        {
            //Returns the solution list
            return solutionList;
        }


        //Pre: The start node and the goal node
        //Post: The path is found if its possible to get from the start node to the goal node
        //Desc: A method which finds a path using the A* algorithm from the first node to the second without corner cutting
        public bool FindPath(Node startNode, Node goalNode)
        {
            //Adds the start node to the open list
            openList.Add(startNode);

            //Loop that goes as long as the amount of nodes in the open list is greater then 0
            while (openList.Count > 0)
            {
                //Gets the node with the lowest cost from the open list and removes it
                Node currentNode = LowestCostNode(openList);
                openList.Remove(currentNode);

                //If the current node is the goal node
                if (currentNode.Row == goalNode.Row && currentNode.Column == goalNode.Column)
                {
                    //Loop through all the nodes to add them to the solutions list
                    while (currentNode != null)
                    {
                        solutionList.Add(currentNode);
                        currentNode = currentNode.ParentNode;
                    }

                    //Returns that a path was found
                    return true;
                }

                //Gets the nodes that are around the current node 
                AddNodesAround(currentNode, goalNode);

                //Loop for each of the nodes that are around
                foreach (Node node in nodesAround)
                {
                    // Test if the new node is on the open list
                    ContainsNode(openList, node);

                    //If the new node is in the open list and the TotalCost is higher, we can skip it
                    if (nodeFoundOpen != null && node.FValue > nodeFoundOpen.FValue)
                    {
                        continue;
                    }

                    // Test if the new node is on the closed list
                    ContainsNode(closeList, node);

                    //If new node is in the closed list and the TotalCost is higher, we can skip it
                    if (nodeFoundClose != null && node.FValue > nodeFoundClose.FValue)
                    {
                        continue;
                    }

                    //If a node was found in the open list, remove it
                    if (indexNodeFoundOpen != -1)
                    {
                        openList.RemoveAt(indexNodeFoundOpen);
                    }

                    //If a node was found in the closed list, remove it
                    if (indexNodeFoundClose != -1)
                    {
                        closeList.RemoveAt(indexNodeFoundClose);
                    }

                    //Adds the node to the open list
                    openList.Add(node);
                }

                //Adds the current node to the closed list
                closeList.Add(currentNode);
            }

            //Returns that a path was not found
            return false;
        }


        //Pre: The list of nodes that is being checked
        //Post: The node with the lowest cost is returned
        //Desc: A method which returns the node with the lowest cost from the list that is provided
        private Node LowestCostNode(List<Node> nodeList)
        {
            //Temp variables for the lowest cost and the current node
            int lowestCost = 0;
            Node currentNode = null;

            //Loop through every node from the list
            for (int i = 0; i < nodeList.Count; i++)
            {
                //If the current lowest cost is 0 (which is not possible unless its the first node), it sets the current node to the lowes
                if (lowestCost == 0)
                {
                    currentNode = nodeList[i];
                    lowestCost = nodeList[i].FValue;
                }
                //If the previous lowest cost is greater then the current nodes lowest cost, it updates the lowest cost
                else if (lowestCost > nodeList[i].FValue)
                {
                    currentNode = nodeList[i];
                    lowestCost = nodeList[i].FValue;
                }
            }

            //Returns the current node
            return currentNode;
        }


        //Pre: The list of nodes that is going to be checked and the node that needs to be found
        //Post: A boolean is returned for whether the node was found or not
        //Desc: A method which returns whether a list of nodes contains the node that needs to be found
        private bool ContainsNode(List<Node> nodeList, Node currentNode)
        {
            //Loops through every node in the list of nodes
            for (int i = 0; i < nodeList.Count; i++)
            {
                //If the node that needs to be found is found
                if (currentNode.Row == nodeList[i].Row && currentNode.Column == nodeList[i].Column)
                {
                    //If the current list is the open list, add the node to the open list variables
                    if (nodeList == openList)
                    {
                        indexNodeFoundOpen = i;
                        nodeFoundOpen = nodeList[i];
                    }
                    //If the current list is the closed list, add the node to the closed list variables
                    else
                    {
                        indexNodeFoundClose = i;
                        nodeFoundClose = nodeList[i];
                    }

                    //Returns that the node was found
                    return true;
                }
            }

            //If the current list is the open list, set the open list variables to the node not being found
            if (nodeList == openList)
            {
                indexNodeFoundOpen = -1;
                nodeFoundOpen = null;
            }

            //If the current list is the closed list, set the closed list variables to the node not being found
            else
            {
                indexNodeFoundClose = -1;
                nodeFoundClose = null;
            }

            //Returns that the node was not found
            return false;
        }


        //Pre: The current node and the new node that needs to be checked for corner cutting
        //Post: A boolean is returned for whether the current node is corner cutting or not
        //Desc: A method which returns a boolean for whether moving to a new node would be considered corner cutting from the current node
        private bool CheckCuttingCorner(Node currentNode, Node newNode)
        {
            //Temp variables for checking if the nodes that are around the current node in different scenario's are wall nodes or not
            int temp1 = 0;
            int temp2 = 0;
            int temp3 = 0;
            int temp4 = 0;

            //If the row of the current node is less then the new node
            if (currentNode.Row < newNode.Row)
            {
                //If the column of the current node is greater then the column of the new node
                if (currentNode.Column > newNode.Column)
                {
                    //Sets the temp variables
                    temp1 = 1;
                    temp2 = 0;
                    temp3 = 0;
                    temp4 = -1;
                }
                //If the column of the current node is less then the column of the greater node
                else if (currentNode.Column < newNode.Column)
                {
                    //Sets the temp variables
                    temp1 = 1;
                    temp2 = 0;
                    temp3 = 0;
                    temp4 = 1;
                }
                //If the column is the same for both nodes
                else
                {
                    //Returns that the current node is not corner cutting
                    return false;
                }
            }
            //If the row of the current node is greater then the row of the new node
            else if (currentNode.Row > newNode.Row)
            {
                //If the column of the current node is greater then the column of the new node
                if (currentNode.Column > newNode.Column)
                {
                    //Set the temp variables
                    temp1 = 0;
                    temp2 = -1;
                    temp3 = -1;
                    temp4 = 0;
                }
                //If the column of the current node is less then the column of the greater node
                else if (currentNode.Column < newNode.Column)
                {
                    //Set the temp variables
                    temp1 = 0;
                    temp2 = 1;
                    temp3 = -1;
                    temp4 = 0;
                }
                //If the column is the same for both nodes
                else
                {
                    //Returns that the current node is not corner cutting
                    return false;
                }
            }
            //If the current row is the same for both nodes
            else
            {
                //Returns that the current node is not corner cutting
                return false;
            }

            //Checks the tiles around the current node that can't be a wall for the current node to not be considered corner cutting. If the 
            //tiles aren't walls, then return that the new node is not corner cutting, otherwise return that it is
            if (gameTiles[currentNode.Row + temp1, currentNode.Column + temp2].CurrentTileType != TileType.Wall &&
                gameTiles[currentNode.Row + temp3, currentNode.Column + temp4].CurrentTileType != TileType.Wall)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        //Pre: The current node that the nodes around have to be found for and the goal node for calculation the F values
        //Post: The nodes around the current node are calculated
        //Desc: A method which calculates the ndoes around the current node and only adds them if they aren't walls
        private void AddNodesAround(Node currentNode, Node goalNode)
        {
            //Clears the nodes that were around the last node
            nodesAround.Clear();

            //Loop for the rows that are checked for nodes
            for (int row = currentNode.Row - 1; row <= currentNode.Row + 1; row++)
            {
                //Loop for the columns that are checked for nodes
                for (int column = currentNode.Column - 1; column <= currentNode.Column + 1; column++)
                {
                    //If the current row and column are in the game tiles
                    if (row < gameTiles.GetLength(0) && row >= 0 && column < gameTiles.GetLength(1) && column >= 0)
                    {
                        //If the current row and column is the same as the row and column of the node that is being checked,
                        //continue to the next node
                        if (row == currentNode.Row && column == currentNode.Column)
                        {
                            continue;
                        }

                        //If the current tile is not a wall
                        if (gameTiles[row, column].CurrentTileType != TileType.Wall)
                        {
                            //Creates the new node
                            Node temp = new Node(row, column, currentNode);

                            //If the current node is not corner cutting
                            if (!CheckCuttingCorner(currentNode, temp))
                            {
                                //Adds the node
                                nodesAround.Add(temp);
                            }
                        }
                    }
                }
            }

            //Calculates the F values for all the new nodes
            CalculateFValues(nodesAround, currentNode, goalNode);
        }


        //Pre: The nodes that need to be checked, the current node (parent node), and all the goal node
        //Post: The F values are calculated for all the nodes that need to be checked
        //Desc: A method which calculates the F values for all the nodes that need the F values to be calcualted for
        private void CalculateFValues(List<Node> nodesToCheck, Node currentNode, Node goalNode)
        {
            //Loop through every node that needs to be checked
            for (int i = 0; i < nodesToCheck.Count; i++)
            {
                //Calculate the F value of the current node
                nodesToCheck[i].CalculateValues(goalNode);
            }
        }
    }
}
