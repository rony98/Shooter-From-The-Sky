/*
 * Author: Rony Verch
 * File Name: Enemy.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 21, 2015
 * Modified Date: January 20, 2015
 * Description: Class for the enemy that does all the updating and drawing code when called by the driver
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class Enemy : Human
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //The displacement between the next point in the enemy's path and the current enemy position
        public const int ENEMY_PATH_DISPLACEMENT = 2;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Rifle for the enemy
        private EnemyRifle rifle = new EnemyRifle();

        //Variable for the pathfinding
        private PathFinding pathFinding;

        //List of nodes that the enemy follows for the shortest path
        private List<Vector2> currentPath = new List<Vector2>();

        //Boolean for whether the player is currently in the by the enemy
        private bool playerInView = false;

        //List of projectiles that the enemy shoots
        public List<Projectile> Projectiles { get; set; }

        //Integers for the screen width and height
        private int screenWidth = 0;
        private int screenHeight = 0;


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The game tiles, x and y position fo the enemy, width and height of the enemy, and the width and height of the screen
        //Post: The enemy is created
        //Desc: A constructor for the enemy
        public Enemy(Tile[,] gameTiles, int xPos, int yPos, int enemyWidth, int enemyHeight, int screenWidth, int screenHeight)
        {
            //Sets the pathfinding and frame animation objects
            pathFinding = new PathFinding(gameTiles);
            frameAnimation = new FrameAnimation(rifle.GetTextures(AnimationState.Idle), IDLE_FRAME_DELAY);

            //Sets the human to not moving
            HumanMoving = false;

            //Sets the position and bounds of the enemy
            Position = new Vector2(xPos, yPos);
            bounds = new Rectangle((int)Position.X, (int)Position.Y, enemyWidth, enemyHeight);

            //Creates a new list of projectiles
            Projectiles = new List<Projectile>();

            //Sets the width and height of the screen
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            //Sets the health to full
            Health = 100;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The player, the game tiles, and whether the pathfinding needs to be checked
        //Post: The enemy is updated
        //Desc: An update method for the enemy
        public void Update(Player player, Tile[,] gameTiles, bool pathCheck)
        {
            //Gets whether the player is currently in view
            playerInView = IsPlayerInView(player, gameTiles);

            //If the player is in view and the path needs to be checked for the current enemy at the current frame
            if (playerInView && pathCheck)
            {
                //Clears the current path
                currentPath.Clear();

                //Calculates the row and column of the tile that the enemy and player are on
                Vector2 enemyLocation = new Vector2((float)Math.Round(Position.X / Tile.TILE_X_SIZE), (float)Math.Round(Position.Y / Tile.TILE_Y_SIZE));
                Vector2 playerLocation = player.Position + player.GetWallDisplacement();
                playerLocation = new Vector2(playerLocation.X / Tile.TILE_X_SIZE, playerLocation.Y / Tile.TILE_Y_SIZE);

                //Creates a new pathfinding instance with the gametiles and finds the path
                pathFinding = new PathFinding(gameTiles);
                pathFinding.FindPath(new Node((int)enemyLocation.X, (int)enemyLocation.Y), new Node((int)playerLocation.X, (int)playerLocation.Y));

                //Temporary list of nodes which is used to get the enemy's path 
                List<Node> tempList = new List<Node>();

                //Gets the solution from the path finding
                tempList = pathFinding.GetSolution();

                //If the solution has more then two tile
                if (tempList.Count > 2)
                {
                    //Removes the first and last tile in the list
                    tempList.RemoveAt(tempList.Count - 1);
                    tempList.RemoveAt(0);

                    //Loop for every tile in the list
                    for (int i = tempList.Count - 1; i >= 0; i--)
                    {
                        //Adds the tile x and y values to the enemy's path
                        currentPath.Add(new Vector2(tempList[i].Row * Tile.TILE_X_SIZE, tempList[i].Column * Tile.TILE_Y_SIZE));
                    }
                }
                //If the solution doesn't have more then two tiles, the current path is cleared
                else
                {
                    currentPath.Clear();
                }
            }

            //If the player is in view
            if (playerInView)
            {
                //The rotation angle towards the player is calculated
                CalcRotation(player.Position.X, player.Position.Y, 0, 0);

                //If the enemy is able to shoot
                if (shootDelayCount >= EnemyRifle.SHOT_DELAY && rifle.currentAmmo > 0)
                {
                    //If the enemy is currently not shooting
                    if (frameAnimation.AnimState == AnimationState.Shoot && frameAnimation.ShootingFrameDone || frameAnimation.AnimState != AnimationState.Shoot)
                    {
                        //The enemy is set to shoot and it's textures are changed
                        frameAnimation.AnimState = AnimationState.Shoot;
                        frameAnimation.ResetFrame(rifle.GetTextures(frameAnimation.AnimState), SHOOT_FRAME_DELAY);

                        //The shooting delay is reset
                        shootDelayCount = 0;
                    }
                }
            }
            //If the player is not in view
            else
            {
                //If the player currently has a path its following
                if (currentPath.Count > 0)
                {
                    //Calculates the rotation angle towards its next point in the path
                    CalcRotation(currentPath[0].X, currentPath[0].Y, 0, 0);
                }
            }

            //If the enemy needs to reload
            if (rifle.currentAmmo == 0 && frameAnimation.AnimState != AnimationState.Reload)
            {
                //The enemy is set to reload
                frameAnimation.AnimState = AnimationState.Reload;
                frameAnimation.ResetFrame(rifle.GetTextures(frameAnimation.AnimState), RELOAD_FRAME_DELAY);
            }

            //If the enemy has a path it should follow
            if (currentPath.Count > 0)
            {
                //The enemy is set to move to the next point in its path
                MoveEnemy();
            }
            //If the enemy does not have a path
            else
            {
                //The enemy is set to being idle if it's not doing anything else at the moment such as reloading or shooting
                if (HumanMoving && frameAnimation.AnimState != AnimationState.Reload && frameAnimation.AnimState != AnimationState.Shoot)
                {
                    SetHumanIdle();
                }
            }

            //The frame animation for the enemy is updated
            frameAnimation.Update();

            //If the enemy is done shooting
            if (frameAnimation.AnimState == AnimationState.Shoot && frameAnimation.CurrFrame == SHOOT_FRAME_AMOUNT
                && frameAnimation.Count == SHOOT_FRAME_DELAY - 1)
            {
                //If the enemy is currently moving
                if (HumanMoving)
                {
                    //The frame animation for the enemy is set to moving
                    frameAnimation.AnimState = AnimationState.Move;
                    frameAnimation.ResetFrame(rifle.GetTextures(frameAnimation.AnimState), MOVE_FRAME_DELAY);
                }
                //If the enemy is currently idle
                else
                {
                    //The frame animation for the enemy is set to idle
                    frameAnimation.AnimState = AnimationState.Idle;
                    frameAnimation.ResetFrame(rifle.GetTextures(frameAnimation.AnimState), IDLE_FRAME_DELAY);
                }
            }

            //If the frame animation for the shooting is done and a bullet needs to be created
            if (frameAnimation.ShootingFrameDone)
            {
                //The direction for the bullet is calculated
                Vector2 direction = new Vector2((float)Math.Cos(rotationAngle), (float)Math.Sin(rotationAngle));
                direction.Normalize();

                //The point where the bullet spawns and the point at which the enemy rotates is calculated
                Vector2 pointToRotate = new Vector2(Position.X + EnemyRifle.BULLET_DISPLACEMENT_X, Position.Y + EnemyRifle.BULLET_DISPLACEMENT_Y);
                Vector2 centerPoint = new Vector2(Position.X + rotationOrigin.X, Position.Y + rotationOrigin.Y);

                //The new x and y for where the bullet should spawn is calculated
                int x = (int)(direction.X * (pointToRotate.X - centerPoint.X) - direction.Y * (pointToRotate.Y - centerPoint.Y) + centerPoint.X);
                int y = (int)(direction.Y * (pointToRotate.X - centerPoint.X) + direction.X * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y);

                //The projectile is created with the new spawn point and the ammo is decreased
                Projectiles.Add(new Projectile(new Vector2(x, y), ProjectileType.Bullet, EnemyRifle.BULLET_DAMAGE, EnemyRifle.BULLET_SPEED, rotationAngle, screenWidth, screenHeight));
                rifle.currentAmmo--;

                //The frame animation for the shooting is set to not done so the bullet stop being created
                frameAnimation.ShootingFrameDone = false;
            }
            //If the frame animation for the reloading is done
            else if (frameAnimation.ReloadFrameDone)
            {
                //The ammo for the rifle is reset and the frame animation for the reloading is set to not done
                rifle.currentAmmo = EnemyRifle.AMMO_CAPACITY;
                frameAnimation.ReloadFrameDone = false;

                //If the enemy is moving
                if (HumanMoving)
                {
                    //The frame animation for the enemy is set to moving
                    frameAnimation.AnimState = AnimationState.Move;
                    frameAnimation.ResetFrame(rifle.GetTextures(frameAnimation.AnimState), MOVE_FRAME_DELAY);
                }
                //If the enemy is idle
                else
                {
                    //The frame animation for the enemy is set to idle
                    frameAnimation.AnimState = AnimationState.Idle;
                    frameAnimation.ResetFrame(rifle.GetTextures(frameAnimation.AnimState), IDLE_FRAME_DELAY);
                }
            }

            //For each projecitle in the list of projectiles
            foreach (Projectile projectile in Projectiles)
            {
                //The owner of the bullet is set to the current enemy
                projectile.BulletOwner = this;
            }

            //The shooting delay count is incremented
            shootDelayCount++;
        }


        //Pre: The spritebatch and a blank texture for the health bar
        //Post: The enemy is drawn with its health bar
        //Desc: A method for drawing the enemy and its health bar
        public void Draw(SpriteBatch sb, Texture2D blankTexture, bool drawHealthBar)
        {
            //Creates a float for the scaling of the enemy
            float scale = 1;

            //If the enemy is currently shooting, his texture is rescaled
            if (frameAnimation.AnimState == AnimationState.Reload)
            {
                scale = 1.027f;
            }

            //The current frame for the enemy is drawn
            frameAnimation.Draw(sb, bounds, Position, rotationAngle, rotationOrigin, scale);

            //If the health bar is needed to be drawn, it is drawn
            if (drawHealthBar)
            {
                DrawHealthBar(sb, blankTexture);
            }
        }


        //Pre: None
        //Post: The enemy's path is cleared
        //Desc: A method for clearing the enemy's path
        public void ResetPath()
        {
            //The current path is cleared
            currentPath.Clear();
        }


        //Pre: None
        //Post: The enemy is set to moving
        //Desc: An overriden method for setting the enemy to moving
        public override void SetHumanMoving()
        {
            //Calls the parents set human moving method
            base.SetHumanMoving();

            //The frames that the enemy is currently using are changed to the moving frames
            frameAnimation.ResetFrame(rifle.GetTextures(frameAnimation.AnimState), MOVE_FRAME_DELAY);
        }


        //Pre: None
        //Post: The enemy is set to being idle
        //Desc: An overriden method for setting the enemy to idle
        public override void SetHumanIdle()
        {
            //Calls the parents set human moving method
            base.SetHumanIdle();

            //The frames that the enemy is currently using are changed to the idle frames
            frameAnimation.ResetFrame(rifle.GetTextures(frameAnimation.AnimState), IDLE_FRAME_DELAY);
        }


        //Pre: The player and game tiles
        //Post: A boolean is returned for whether the player is in the enemy's line of sight
        //Desc: A method for determining whether the enemy can see the player
        private bool IsPlayerInView(Player player, Tile[,] gameTiles)
        {
            //Creates vectors for the player and enemy center positions
            Vector2 centerPlayer = new Vector2(player.Position.X + (player.GetBounds().Width / 2), player.Position.Y + (player.GetBounds().Height / 2));
            Vector2 centerEnemy = new Vector2(Position.X + (bounds.Width / 2), Position.Y + (bounds.Height / 2));

            //Variable for the player's line
            RaycastLine playerLine;

            //If the X's and Y's for the enemy and player are not the same
            if (centerPlayer.X != centerEnemy.X && centerPlayer.Y != centerEnemy.Y)
            {
                //Creates the line from the player to the enemy
                playerLine = new RaycastLine(false, false, centerPlayer, centerEnemy);
            }
            //If they are the same Y values (horizontal line)
            else if (centerPlayer.Y == centerEnemy.Y)
            {
                //Creates the line from the player to the enemy
                playerLine = new RaycastLine(false, true, centerPlayer, centerEnemy);
            }
            //If they are the same X values (vertical line)
            else
            {
                //Creates the line from the player to the enemy
                playerLine = new RaycastLine(true, false, centerPlayer, centerEnemy);
            }

            //Calculates the quadrant for the enemy relative to the player
            Quadrant playerQuadrant = GetQuadrant(centerPlayer, centerEnemy);

            //Temp vector for storing the location of the current intersection point
            Vector2 tempVector = new Vector2(0, 0);

            //Boolean for whether the enemy can see the player
            bool playerInView = true;

            //For each tile in the game tiles array
            foreach (Tile tile in gameTiles)
            {
                //If the current tile is a wall tile
                if (tile.CurrentTileType == TileType.Wall)
                {
                    //For each line in the tile
                    foreach (RaycastLine line in tile.Lines)
                    {
                        //Sets the temp vector to the current intersection point
                        tempVector = CalcIntersectionPoint(playerLine, line);

                        //Checks if the quadrant of the  position relative to the player is the same quadrant as the enemy to the player
                        if (GetQuadrant(centerPlayer, tempVector) == playerQuadrant)
                        {
                            //If the temp vector x is between the x's of the player and enemy
                            if (tempVector.X >= centerPlayer.X && tempVector.X <= centerEnemy.X ||
                                tempVector.X >= centerEnemy.X && tempVector.X <= centerPlayer.X)
                            {
                                //If the temp vector y is between teh y's of the player and enemy
                                if (tempVector.Y >= centerPlayer.Y && tempVector.Y <= centerEnemy.Y ||
                                    tempVector.Y >= centerEnemy.Y && tempVector.Y <= centerPlayer.Y)
                                {
                                    //Checks if the intersection is between the bounds of the tile
                                    if (tempVector.X >= tile.PositionRect.X && tempVector.X <= tile.PositionRect.X + tile.PositionRect.Width &&
                                        tempVector.Y >= tile.PositionRect.Y && tempVector.Y <= tile.PositionRect.Y + tile.PositionRect.Height)
                                    {
                                        //Sets the player to not being in the enemies view
                                        playerInView = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //If the player is not in the enemies view
                    if (!playerInView)
                    {
                        //Breaks from the for each loop
                        break;
                    }
                }
            }

            //Returns whether the player is in the enemies view
            return playerInView;
        }


        //Pre: The two lines that the intersection point has to be calculated for
        //Post: The intersection point between the two lines is returned
        //Desc: A method for calculating the intersection point between two lines
        private Vector2 CalcIntersectionPoint(RaycastLine firstLine, RaycastLine secondLine)
        {
            //Variables for the X and Y of the intersection point
            double x = 0;
            double y = 0;

            //If the second line is not a vertical line
            if (!secondLine.VerticalLine)
            {
                //If the first line is not horizontal or vertical
                if (!firstLine.HorizontalLine && !firstLine.VerticalLine)
                {
                    //Calculates the X of the intersection point
                    x = firstLine.LineM - secondLine.LineM;
                    x = (secondLine.LineB - firstLine.LineB) / x;

                    //Calculates the Y of the intersection point
                    y = (firstLine.LineM * x) + firstLine.LineB;
                }
                //If the first line is vertical
                else if (!firstLine.HorizontalLine && firstLine.VerticalLine)
                {
                    //Sets the X and Y of the intersection
                    x = firstLine.LineB;
                    y = secondLine.LineB;
                }
            }
            else
            {
                //If the first line is not horizontal or vertical
                if (!firstLine.HorizontalLine && !firstLine.VerticalLine)
                {
                    //Calculates the Y of the intersection point
                    y = (firstLine.LineM * secondLine.LineB) + firstLine.LineB;

                    //Calculates the X of the intersection point
                    x = (y - firstLine.LineB) / firstLine.LineM;
                }
                //If the first line is horizontal
                else if (firstLine.HorizontalLine && !firstLine.VerticalLine)
                {
                    //Sets the X and Y of the intersection
                    x = secondLine.LineB;
                    y = firstLine.LineB;
                }
            }

            //Returns the intersection point
            return new Vector2((float)x, (float)y);
        }


        //Pre: The first and second position that has to be checked
        //Post: A quadrant is returned for the second position relative to the first position
        //Desc: A method that returns the quadrant of one position relative to the other
        private Quadrant GetQuadrant(Vector2 firstPos, Vector2 secondPos)
        {
            //Checks which quadrant one position is in relative to the other and returns the quadrant that it is in
            if (firstPos.X <= secondPos.X && firstPos.Y >= secondPos.Y)
            {
                return Quadrant.NorthEast;
            }
            else if (firstPos.X <= secondPos.X && firstPos.Y <= secondPos.Y)
            {
                return Quadrant.SouthEast;
            }
            else if (firstPos.X >= secondPos.X && firstPos.Y <= secondPos.Y)
            {
                return Quadrant.SouthWest;
            }
            else if (firstPos.X >= secondPos.X && firstPos.Y >= secondPos.Y)
            {
                return Quadrant.NorthWest;
            }

            //If it's not in any quadrant, it is returned that it is not in any of the quadrants
            return Quadrant.None;
        }


        //Pre: None
        //Post: The enemy is moved
        //Desc: A method for moving the enemy towards its next point in its path
        private void MoveEnemy()
        {
            //If the human wasn't moving, reloading, or shooting before it is set to moving
            if (HumanMoving == false && frameAnimation.AnimState != AnimationState.Reload && frameAnimation.AnimState != AnimationState.Shoot)
            {
                SetHumanMoving();
            }

            //If the current position of the enemy is the first point in the path, the first point is removed
            if (Position.X == currentPath[0].X && Position.Y == currentPath[0].Y)
            {
                currentPath.RemoveAt(0);
            }

            //If a path currently exists for the player to move in
            if (currentPath.Count > 0)
            {
                //If the enemy is currently close enough to the next point in its path that its considered to be on it
                if (Math.Abs(currentPath[0].X - Position.X) < ENEMY_PATH_DISPLACEMENT && Math.Abs(currentPath[0].Y - Position.Y) < ENEMY_PATH_DISPLACEMENT)
                {
                    //The enemy's position is set to be the next point in its path and that point is removed from the enemy's path
                    Position = currentPath[0];
                    currentPath.RemoveAt(0);
                }

                //If the enemy currently has a path to follow
                if (currentPath.Count > 0)
                {
                    //The angle that the enemy needs to move in is calculated
                    float angle = (float)(Math.Atan2(currentPath[0].Y - Position.Y, currentPath[0].X - Position.X));

                    //The direction that the enemy moves in is calculated using the angle
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)(Math.Sin(angle)));
                    direction.Normalize();

                    //The enemy's position is updated using the direction and the speed the enemy moves in
                    Position += direction * HUMAN_SPEED;
                    SetBounds();
                }
            }
        }
    }
}
