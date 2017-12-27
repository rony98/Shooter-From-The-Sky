/*
 * Author: Rony Verch
 * File Name: Projectile.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 30, 2015
 * Modified Date: January 20, 2015
 * Description: The projectile is the bullets/knives that the guns shoot or the player throws. The projectiles can vary depending on the gun that shoots them
 *              by moving, faster, having higher damage, etc.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class Projectile
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //The bounds and position of the projectile
        private Rectangle bounds;
        private Vector2 position;

        //The screen width and height
        private int screenWidth;
        private int screenHeight;

        //The rotation angle
        private float rotationAngle;

        //The damage and speed
        public int Damage { get; private set; }
        public float Speed { get; private set; }

        //Static Textures for the drawing of the projectile
        static private Texture2D knifeTexture;
        static private Texture2D bulletTexture;

        //The projectile type
        private ProjectileType projectileType;

        //The origin on the projectile
        private Vector2 origin = new Vector2(0, 0);

        //Boolean for whether the projectile needs destroying
        public bool NeedDestroy { get; set; }

        //Properties for if the projectile is the player's or enemies
        public bool PlayerBullet { get; set; }
        public Enemy BulletOwner { get; set; }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The position, the projectile type, the damage, the speed, the rotation angle, the screen width and height
        //Post: The projectile is created
        //Desc: A constructor for the projectiles
        public Projectile(Vector2 position, ProjectileType projectileType, int damage, float speed, float rotationAngle, int screenWidth, int screenHeight)
        {
            //Sets all the data for the projectile
            this.position = position;
            Damage = damage;
            Speed = speed;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.projectileType = projectileType;
            this.rotationAngle = rotationAngle;
            NeedDestroy = false;
            PlayerBullet = false;
            BulletOwner = null;

            //If the projectile is a bullet
            if (projectileType == ProjectileType.Bullet)
            {
                //Sets the bounds of the projectile
                bounds = new Rectangle((int)position.X, (int)position.Y, bulletTexture.Width, bulletTexture.Height);
            }
            //If the projectile is a knife
            else if (projectileType == ProjectileType.Knife)
            {
                //Sets the bounds of the projectile
                bounds = new Rectangle((int)position.X, (int)position.Y, knifeTexture.Width, knifeTexture.Height);
            }
        }


        //Pre: The gametiles and camera location
        //Post: None
        //Desc: The projectile is updated
        public void Update(Tile[,] gameTiles, Vector2 cameraLocation)
        {
            //Moves the projectile
            MoveObject(gameTiles, cameraLocation);
        }


        //Pre: The sprite batch
        //Post: None
        //Desc: A draw method for the projectile
        public void Draw(SpriteBatch sb)
        {
            //If the projectile is a bullet, a bullet is drawn
            if (projectileType == ProjectileType.Bullet)
            {
                sb.Draw(bulletTexture, position, null, Color.White, rotationAngle, origin, 1, SpriteEffects.None, 0);
            }
            //If the projectile is a knife, a knife is drawn
            else if (projectileType == ProjectileType.Knife)
            {
                sb.Draw(knifeTexture, position, null, Color.White, rotationAngle, origin, 1, SpriteEffects.None, 0);
            }
        }


        //Pre: None
        //Post: The bounds are set
        //Desc: A method for setting the bounds
        private void SetBounds()
        {
            //The bounds for the projectile are set
            bounds.X = (int)position.X;
            bounds.Y = (int)position.Y;
        }


        //Pre: None
        //Post: The bounds are returned
        //Desc: A get method for the bounds
        public Rectangle GetBounds()
        {
            //The bounds are returned
            return bounds;
        }


        //Pre: None
        //Post: The projectile type is returned
        //Desc: A get method for the projectile type
        public ProjectileType GetProjectileType()
        {
            //Returns the projectile type
            return projectileType;
        }


        //Pre: None
        //Post: The rotation angle is returned
        //Desc: A get method for the rotation angle
        public float GetRotationAngle()
        {
            //Returns the rotation angle
            return rotationAngle;
        }


        //Pre: The gametiles and the camera location
        //Post: None
        //Desc: A method for moving the projectile
        public void MoveObject(Tile[,] gameTiles, Vector2 cameraLocation)
        {
            //The direction of the projectile is calculated
            Vector2 direction = new Vector2((float)Math.Cos(rotationAngle), (float)(Math.Sin(rotationAngle)));
            direction.Normalize();

            //The corners of the projectile are calculated
            Vector2 topLeft = position + (direction * Speed);
            Vector2 bottomRight = new Vector2(topLeft.X + bounds.Width, topLeft.Y + bounds.Height);
            Vector2 bottomLeft = new Vector2(topLeft.X, bottomRight.Y);
            Vector2 topRight = new Vector2(bottomRight.X, topLeft.Y);

            //The projectile is checked against the bounds of the screen
            if (topLeft.X >= 0 && bottomRight.X <= cameraLocation.X && topLeft.Y >= 0 && bottomRight.Y <= cameraLocation.Y)
            {
                //The row and column of the top left is calculated
                int topLeftRow = (int)(topLeft.X / Tile.TILE_X_SIZE);
                int topLeftColumn = (int)(topLeft.Y / Tile.TILE_Y_SIZE);

                //The row and column of the bottom right is calculated
                int bottomRightRow = (int)(bottomRight.X / Tile.TILE_X_SIZE);
                int bottomRightColumn = (int)(bottomRight.Y / Tile.TILE_Y_SIZE);

                //The row and column of the bottom left is calculated
                int bottomLeftRow = (int)(bottomLeft.X / Tile.TILE_X_SIZE);
                int bottomLeftColumn = (int)(bottomLeft.Y / Tile.TILE_Y_SIZE);

                //The row and column of the top right is calculated
                int topRightRow = (int)(topRight.X / Tile.TILE_X_SIZE);
                int topRightColumn = (int)(topRight.Y / Tile.TILE_Y_SIZE);

                //If all the corners are not on wall tiles
                if (gameTiles[topLeftRow, topLeftColumn].CurrentTileType != TileType.Wall &&
                    gameTiles[bottomRightRow, bottomRightColumn].CurrentTileType != TileType.Wall &&
                    gameTiles[bottomLeftRow, bottomLeftColumn].CurrentTileType != TileType.Wall &&
                    gameTiles[topRightRow, topRightColumn].CurrentTileType != TileType.Wall)
                {
                    //The position and bounds for the projectile are updated
                    position += direction * Speed;
                    SetBounds();
                }
                //If any of the corners are on walls
                else
                {
                    //The projectile is set to need to be destroyed
                    NeedDestroy = true;
                }
            }
            //If the projectile is not between the screen bounds
            else
            {
                //The projectile is set to need to be destroyed
                NeedDestroy = true;
            }
        }


        //Pre: The content manager
        //Post: The texture data is loaded
        //Desc: A static method which loads all the texture data for the class
        public static void SetTextureData(ContentManager cm)
        {
            knifeTexture = cm.Load<Texture2D>("Projectiles\\Knife");
            bulletTexture = cm.Load<Texture2D>("Projectiles\\Bullet");
        }


        //Pre: None
        //Post: The current texture is returned
        //Desc: A method which returns the current projectile texture
        public Texture2D GetTexture()
        {
            //If the current projectile is a bullet
            if (projectileType == ProjectileType.Bullet)
            {
                //Return the bullet texture
                return bulletTexture;
            }
            //If the current projectile is a knife
            else if (projectileType == ProjectileType.Knife)
            {
                //Return the knife texture
                return knifeTexture;
            }

            //Return null
            return null;
        }
    }
}
