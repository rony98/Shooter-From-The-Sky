/*
 * Author: Rony Verch
 * File Name: Player.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 21, 2015
 * Modified Date: January 20, 2015
 * Description: Class for the player that does all the updating and drawing code when called by the driver
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class Player : Human
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Creates all the weapons for the player
        private Handgun handgun = new Handgun();
        private PlayerRifle rifle = new PlayerRifle();
        private Shotgun shotgun = new Shotgun();
        private ThrowingKnife knife = new ThrowingKnife();

        //Variable for the current weapon
        private Weapon currentWeapon;

        //Properties that represent whether certain actions have to be taken by the player such as shooting or reloading
        public bool NeedShooting { get; set; }
        public bool NeedReload { get; set; }

        //Private variables for the screen width and height
        private int screenWidth = 0;
        private int screenHeight = 0;

        //Displacement for when checking if the player would be going into a wall
        private Vector2 wallDisplacement = new Vector2(5, 5);

        //List of projectiles that the player shoots
        public List<Projectile> Projectiles { get; set; }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The x and y position of the player, the width and height of the player, and the screen width and height
        //Post: The player is created
        //Desc: A constructor for the player
        public Player(int xPos, int yPos, int playerWidth, int playerHeight, int screenWidth, int screenHeight)
        {
            //Sets the current weapon of the player and its textures
            currentWeapon = handgun;
            frameAnimation = new FrameAnimation(currentWeapon.GetTextures(AnimationState.Idle), IDLE_FRAME_DELAY);

            //Sets the player to neither need to shoot or reload
            NeedShooting = false;
            NeedReload = false;

            //Sets the human to not moving
            HumanMoving = false;

            //Updates the position of the player
            Position = new Vector2(xPos, yPos);
            bounds = new Rectangle((int)Position.X, (int)Position.Y, playerWidth, playerHeight);

            //Sets the screen width and height
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            //Creates a list of projectiles for the enemy
            Projectiles = new List<Projectile>();

            //Sets the health to full
            Health = 100;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: The player is updated
        //Desc: A method for updating the player
        public void Update()
        {
            //Updates the current shot delay
            int currentShotDelay = CurrentShotDelay();

            //Checks if the player needs to shoot
            if (NeedShooting)
            {
                //If the player's current weapon is a knife
                if (currentWeapon == knife)
                {
                    //If the player can shoot, the player sets its animation to shooting and begins throwing a knife
                    if (shootDelayCount >= currentShotDelay)
                    {
                        if (frameAnimation.AnimState == AnimationState.Shoot && frameAnimation.ShootingFrameDone || frameAnimation.AnimState != AnimationState.Shoot)
                        {
                            frameAnimation.AnimState = AnimationState.Shoot;

                            frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), KNIFE_THROW_FRAME_DELAY);

                            shootDelayCount = 0;
                        }
                    }
                }
                //If the player's current weapon is not a knife
                else
                {
                    //If the player can shoot it's current weapon, the player sets its animation to shooting and beings to shoot a bullet
                    if (currentWeapon.currentAmmo > 0 && shootDelayCount >= currentShotDelay)
                    {
                        if (frameAnimation.AnimState == AnimationState.Shoot && frameAnimation.ShootingFrameDone || frameAnimation.AnimState != AnimationState.Shoot)
                        {
                            frameAnimation.AnimState = AnimationState.Shoot;

                            frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), SHOOT_FRAME_DELAY);

                            shootDelayCount = 0;
                        }
                    }
                }

                //Sets the player to not need the shooting anymore
                NeedShooting = false;
            }
            //If the player needs to reload
            else if (NeedReload)
            {
                //If the ammo for the gun that the player is currently holding has full ammo, the player can't reload and so it's needing reload is disabled
                if (currentWeapon == rifle && currentWeapon.currentAmmo == PlayerRifle.AMMO_CAPACITY)
                {
                    NeedReload = false;
                }
                else if (currentWeapon == shotgun && currentWeapon.currentAmmo == Shotgun.AMMO_CAPACITY)
                {
                    NeedReload = false;
                }
                else if (currentWeapon == handgun && currentWeapon.currentAmmo == Handgun.AMMO_CAPACITY)
                {
                    NeedReload = false;
                }

                //If the player still needs a reload
                if (NeedReload)
                {
                    //If the current weapon is not a knife, the player is set to beginning reloading its current weapon
                    if (currentWeapon != knife)
                    {
                        frameAnimation.AnimState = AnimationState.Reload;

                        frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), RELOAD_FRAME_DELAY);
                    }
                }

                //Sets the player to not needing a reload
                NeedReload = false;
            }

            //Updates the frame animation
            frameAnimation.Update();

            //If the player is done shooting or throwing a knife
            if (frameAnimation.AnimState == AnimationState.Shoot && frameAnimation.CurrFrame == SHOOT_FRAME_AMOUNT
                && frameAnimation.Count == SHOOT_FRAME_DELAY - 1 && currentWeapon != knife ||
                frameAnimation.AnimState == AnimationState.Shoot && currentWeapon == knife &&
                frameAnimation.CurrFrame == KNIFE_THROW_FRAME_AMOUNT && frameAnimation.Count == KNIFE_THROW_FRAME_DELAY - 1)
            {
                //If the player iis currently moving, its frames are set to the moving frames
                if (HumanMoving)
                {
                    frameAnimation.AnimState = AnimationState.Move;
                    frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), MOVE_FRAME_DELAY);
                }
                //If the player is currently not moving, its frames are set to the idle frames
                else
                {
                    frameAnimation.AnimState = AnimationState.Idle;
                    frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), IDLE_FRAME_DELAY);
                }
            }

            //If the player shooting frame is passed and the player needs to create a bullet
            if (frameAnimation.ShootingFrameDone)
            {
                //The center point that the bullet rotates around around and the direction are calculated
                Vector2 direction = new Vector2((float)Math.Cos(rotationAngle), (float)Math.Sin(rotationAngle));
                Vector2 centerPoint = new Vector2(Position.X + rotationOrigin.X, Position.Y + rotationOrigin.Y);

                //A Vector2 is created for the rotated point that the bullet spawns on
                Vector2 pointToRotate = new Vector2(0, 0);

                //Temp X and Y variables 
                int x = 0;
                int y = 0;

                //If the current weapon is a knife
                if (currentWeapon == knife)
                {
                    //Points are calculated for where the projectile starts
                    pointToRotate = new Vector2(Position.X + ThrowingKnife.KNIFE_DISPLACEMENT_X, Position.Y + ThrowingKnife.KNIFE_DISPLACEMENT_Y);
                    x = (int)(direction.X * (pointToRotate.X - centerPoint.X) - direction.Y * (pointToRotate.Y - centerPoint.Y) + centerPoint.X);
                    y = (int)(direction.Y * (pointToRotate.X - centerPoint.X) + direction.X * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y);

                    //The projectile is created 
                    Projectiles.Add(new Projectile(new Vector2(x, y), ProjectileType.Knife, ThrowingKnife.KNIFE_DAMAGE, ThrowingKnife.KNIFE_SPEED,
                        rotationAngle, screenWidth, screenHeight));
                }
                //If the current weapon is a rifle
                else if (currentWeapon == rifle)
                {
                    //Points are calculated for where the projectile starts
                    pointToRotate = new Vector2(Position.X + PlayerRifle.BULLET_DISPLACEMENT_X, Position.Y + PlayerRifle.BULLET_DISPLACEMENT_Y);
                    x = (int)(direction.X * (pointToRotate.X - centerPoint.X) - direction.Y * (pointToRotate.Y - centerPoint.Y) + centerPoint.X);
                    y = (int)(direction.Y * (pointToRotate.X - centerPoint.X) + direction.X * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y);

                    //The projectile is created 
                    Projectiles.Add(new Projectile(new Vector2(x, y), ProjectileType.Bullet, PlayerRifle.BULLET_DAMAGE, PlayerRifle.BULLET_SPEED,
                        rotationAngle, screenWidth, screenHeight));
                }
                //If the current weapon is a shotgun
                else if (currentWeapon == shotgun)
                {
                    //Points are calculated for where the projectile starts
                    pointToRotate = new Vector2(Position.X + Shotgun.BULLET_DISPLACEMENT_X, Position.Y + Shotgun.BULLET_DISPLACEMENT_Y);
                    x = (int)(direction.X * (pointToRotate.X - centerPoint.X) - direction.Y * (pointToRotate.Y - centerPoint.Y) + centerPoint.X);
                    y = (int)(direction.Y * (pointToRotate.X - centerPoint.X) + direction.X * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y);

                    //The three projectiles for the shotgun are created with a displacement in the rotation angle for the last two so the projectiles move away from each other
                    Projectiles.Add(new Projectile(new Vector2(x, y), ProjectileType.Bullet, Shotgun.BULLET_DAMAGE, Shotgun.BULLET_SPEED,
                        rotationAngle, screenWidth, screenHeight));
                    Projectiles.Add(new Projectile(new Vector2(x, y), ProjectileType.Bullet, Shotgun.BULLET_DAMAGE, Shotgun.BULLET_SPEED,
                        (float)(rotationAngle + 0.15), screenWidth, screenHeight));
                    Projectiles.Add(new Projectile(new Vector2(x, y), ProjectileType.Bullet, Shotgun.BULLET_DAMAGE, Shotgun.BULLET_SPEED,
                        (float)(rotationAngle - 0.15), screenWidth, screenHeight));
                }
                //If the current weapon is a handgun
                else if (currentWeapon == handgun)
                {
                    //Points are calculated for where the projectile starts
                    pointToRotate = new Vector2(Position.X + Handgun.BULLET_DISPLACEMENT_X, Position.Y + Handgun.BULLET_DISPLACEMENT_Y);
                    x = (int)(direction.X * (pointToRotate.X - centerPoint.X) - direction.Y * (pointToRotate.Y - centerPoint.Y) + centerPoint.X);
                    y = (int)(direction.Y * (pointToRotate.X - centerPoint.X) + direction.X * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y);

                    //The projectile is created 
                    Projectiles.Add(new Projectile(new Vector2(x, y), ProjectileType.Bullet, Handgun.BULLET_DAMAGE, Handgun.BULLET_SPEED,
                        rotationAngle, screenWidth, screenHeight));
                }

                //For each projectile in the projectiles list
                for (int i = 0; i < Projectiles.Count; i++)
                {
                    //The projectile is set to being a player's projectile
                    Projectiles[i].PlayerBullet = true;
                }

                //The ammo is decreased for the current weapon
                currentWeapon.currentAmmo--;

                //The frame animation's shooting frame done is set to false so more projectiles aren't created and the shooting process is reset
                frameAnimation.ShootingFrameDone = false;
            }
            //If the current weapon reload frames are done
            else if (frameAnimation.ReloadFrameDone)
            {
                //If the current weapon is not a knife
                if (currentWeapon != knife)
                {
                    //If the current weapon is a rifle, its ammo is reset
                    if (currentWeapon == rifle)
                    {
                        currentWeapon.currentAmmo = PlayerRifle.AMMO_CAPACITY;
                    }
                    //If the current weapon is a shotgun, its ammo is reset
                    else if (currentWeapon == shotgun)
                    {
                        currentWeapon.currentAmmo = Shotgun.AMMO_CAPACITY;
                    }
                    //If the current weapon is a handgun, its ammo is reset
                    else if (currentWeapon == handgun)
                    {
                        currentWeapon.currentAmmo = Handgun.AMMO_CAPACITY;
                    }

                    //The frame animation's reload frames are set to undone so the reloading proces can be reset
                    frameAnimation.ReloadFrameDone = false;

                    //If the human is currently moving, it's animation is set to moving animation
                    if (HumanMoving)
                    {
                        frameAnimation.AnimState = AnimationState.Move;
                        frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), MOVE_FRAME_DELAY);
                    }
                    //If the human is currently not moving, it's animation is set to idle animation
                    else
                    {
                        frameAnimation.AnimState = AnimationState.Idle;
                        frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), IDLE_FRAME_DELAY);
                    }
                }
            }

            //Adds to the shooting delay count
            shootDelayCount++;
        }


        //Pre: The spritebatch and a blank texture for drawing the health bar
        //Post: The player is drawn with it's health bar
        //Desc: A method for drawing the player with it's health bar
        public void Draw(SpriteBatch sb, Texture2D blankTexture)
        {
            //Creates a float for the scale of the player
            float scale = 1;

            //If the player is currently throwing a knife, their texture scale is changed
            if (currentWeapon == knife && frameAnimation.AnimState == AnimationState.Shoot)
            {
                scale = 1.17f;
            }
            //If the player is currently reloading a rifle, their texture scale is changed
            else if (currentWeapon == rifle && frameAnimation.AnimState == AnimationState.Reload)
            {
                scale = 1.027f;
            }
            //If the player is currenty reloading a shotgun, their texture scale is changed
            else if (currentWeapon == shotgun && frameAnimation.AnimState == AnimationState.Reload)
            {
                scale = 1.041f;
            }
            //If the player is currently reloading a handgun, their texture is changed
            else if (currentWeapon == handgun && frameAnimation.AnimState == AnimationState.Reload)
            {
                scale = 1.05f;
            }

            //The player's current frame is drawn with its health bar
            frameAnimation.Draw(sb, bounds, Position, rotationAngle, rotationOrigin, scale);
            DrawHealthBar(sb, blankTexture);
        }


        //Pre: None
        //Post: The wall displacement is returned
        //Desc: A method that returns the wall displacement
        public Vector2 GetWallDisplacement()
        {
            //Returns the wall displacement
            return wallDisplacement;
        }


        //Pre: The weapon to change too
        //Post: The weapon is changed
        //Desc: A method for changing the current weapon
        public void ChangeWeapon(WeaponTypes weaponChange)
        {
            //If the current weapon is not the same as the weapon needed to be changed to
            if (currentWeapon.WeaponType != weaponChange)
            {
                //Sets the needing to shoot and reload to false
                NeedReload = false;
                NeedShooting = false;

                //Sets the frame animation for the shooting and reloading to not done
                frameAnimation.ShootingFrameDone = false;
                frameAnimation.ReloadFrameDone = false;

                //Resets the shooting delay count
                shootDelayCount = 0;

                //Switch statement for the wepaon to change to
                switch (weaponChange)
                {
                    //If the wepaon is a rifle
                    case WeaponTypes.Rifle:
                        //Sets the frame animation to not being a knife animation and set the current weapont to a rifle
                        frameAnimation.KnifeAnimation = false;
                        currentWeapon = rifle;
                        break;

                    //If the weapon is a handgun
                    case WeaponTypes.Handgun:
                        //Sets the frame animation to not being a knife animation and set the current weapon to a handgun
                        frameAnimation.KnifeAnimation = false;
                        currentWeapon = handgun;
                        break;

                    //If the weapon is a shotgun
                    case WeaponTypes.Shotgun:
                        //Sets the frame animation to not being a knife animation and set the current weapon to a shotgun
                        frameAnimation.KnifeAnimation = false;
                        currentWeapon = shotgun;
                        break;

                    //If the weapon is a throwing knife
                    case WeaponTypes.ThrowingKnife:
                        //Sets the frame animation to being a knife animation and sets the current weapon to a knife 
                        frameAnimation.KnifeAnimation = true;
                        currentWeapon = knife;
                        break;
                }

                //If the human is currently moving, it's frame animation is set to the moving frames
                if (HumanMoving)
                {
                    frameAnimation.AnimState = AnimationState.Move;
                    frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), MOVE_FRAME_DELAY);
                }
                //If the human is currently not moving, it's frame animation is set to the idle frames
                else
                {
                    frameAnimation.AnimState = AnimationState.Idle;
                    frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), IDLE_FRAME_DELAY);
                }
            }
        }


        //Pre: None
        //Post: The current shot delay is returned
        //Desc: A method for determining the shot delay for the current weapon
        private int CurrentShotDelay()
        {
            //If the current weapon is a handgun, the shot delay for the handgun is returned
            if (currentWeapon == handgun)
            {
                return Handgun.SHOT_DELAY;
            }
            //If the current weapon is a shotgun, the shot delay for a shotgun is returned
            else if (currentWeapon == shotgun)
            {
                return Shotgun.SHOT_DELAY;
            }
            //If the current weapon is a rifle, the shot delay for a rifle is returned
            else if (currentWeapon == rifle)
            {
                return PlayerRifle.SHOT_DELAY;
            }
            //If the current weapon is a knife, the shot delay for a knife is returned
            else if (currentWeapon == knife)
            {
                return ThrowingKnife.SHOT_DELAY;
            }

            //If the current weapon is somehow neither of those a shot delay of 0 is returned
            return 0;
        }


        //Pre: None
        //Post: The player is set to moving
        //Desc: An overriden method that sets the current player to moving
        public override void SetHumanMoving()
        {
            //Calls the base moving method
            base.SetHumanMoving();

            //Resets the frame animation of the player to the moving animation
            frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), MOVE_FRAME_DELAY);
        }


        //Pre: None
        //Post: The player is set to idle
        //Desc: An overriden method that sets the current to idle
        public override void SetHumanIdle()
        {
            //If the player is currently not shooting or reloading
            if (frameAnimation.AnimState != AnimationState.Shoot || frameAnimation.AnimState != AnimationState.Reload)
            {
                //Calls the base idle method
                base.SetHumanIdle();

                //Resets the frame animation of the player to the idle animation
                frameAnimation.ResetFrame(currentWeapon.GetTextures(frameAnimation.AnimState), IDLE_FRAME_DELAY);
            }
        }


        //Pre: The angle to move at and the array of game tiles
        //Post: The player is moved
        //Desc: A method for moving the player
        public void MovePlayer(float angle, Tile[,] gameTiles)
        {
            //The direction the player is moving in is calculated
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)(Math.Sin(angle)));
            direction.Normalize();

            //The 4 corners of the player are calculated
            Vector2 topLeft = Position + (direction * HUMAN_SPEED) + wallDisplacement;
            Vector2 bottomRight = new Vector2(topLeft.X + bounds.Width, topLeft.Y + bounds.Height) - (wallDisplacement * 2);
            Vector2 bottomLeft = new Vector2(topLeft.X, bottomRight.Y);
            Vector2 topRight = new Vector2(bottomRight.X, topLeft.Y);

            //If the player is between the bounds of the map
            if (topLeft.X >= wallDisplacement.X && bottomRight.X <= screenWidth - wallDisplacement.X
                && topLeft.Y >= wallDisplacement.Y && bottomRight.Y <= screenHeight - wallDisplacement.Y)
            {
                //The top left corner's row and column are calculated
                int topLeftRow = (int)(topLeft.X / Tile.TILE_X_SIZE);
                int topLeftColumn = (int)(topLeft.Y / Tile.TILE_Y_SIZE);

                //The bottom right corner's row and column are calculated
                int bottomRightRow = (int)(bottomRight.X / Tile.TILE_X_SIZE);
                int bottomRightColumn = (int)(bottomRight.Y / Tile.TILE_Y_SIZE);

                //The bottom left corner's row and column are calculated
                int bottomLeftRow = (int)(bottomLeft.X / Tile.TILE_X_SIZE);
                int bottomLeftColumn = (int)(bottomLeft.Y / Tile.TILE_Y_SIZE);

                //The top right corner's row and column are calculated
                int topRightRow = (int)(topRight.X / Tile.TILE_X_SIZE);
                int topRightColumn = (int)(topRight.Y / Tile.TILE_Y_SIZE);

                //If none of the corners are on a wall tile
                if (gameTiles[topLeftRow, topLeftColumn].CurrentTileType != TileType.Wall &&
                    gameTiles[bottomRightRow, bottomRightColumn].CurrentTileType != TileType.Wall &&
                    gameTiles[bottomLeftRow, bottomLeftColumn].CurrentTileType != TileType.Wall &&
                    gameTiles[topRightRow, topRightColumn].CurrentTileType != TileType.Wall)
                {
                    //If the player is currently not moving, shooting, or reloading
                    if (HumanMoving == false && frameAnimation.AnimState != AnimationState.Reload && frameAnimation.AnimState != AnimationState.Shoot)
                    {
                        //The player is set to moving
                        SetHumanMoving();
                    }

                    //The position and bounds of the player are updated
                    Position += direction * HUMAN_SPEED;
                    SetBounds();
                }
            }
        }


        //Pre: None
        //Post: The wepaon type of the current weapon is returned
        //Desc: A method for returning the method type of the current weapon
        public WeaponTypes GetCurrentWeaponType()
        {
            //If the current weapon is a knife, a knife weapon type is returned
            if (currentWeapon == knife)
            {
                return WeaponTypes.ThrowingKnife;
            }
            //If the current weapon is a rifle, a rifle weapon type is returned
            else if (currentWeapon == rifle)
            {
                return WeaponTypes.Rifle;
            }
            //If the current weapon is a shotgun, a shotgun weapon type is returned
            else if (currentWeapon == shotgun)
            {
                return WeaponTypes.Shotgun;
            }
            //If the current weapon is anything else (has to be a handgun cause only other choice), a handgun weapon type is returned
            else
            {
                return WeaponTypes.Handgun;
            }
        }


        //Pre: None 
        //Post: The ammo of the current gun is returned
        //Desc: A getter method for the ammo of the current gun
        public int CurrentGunAmmo()
        {
            //If the weapon is a kinfe
            if (currentWeapon == knife)
            {
                //Returns -1 cause a knife doesn't have ammo
                return -1;
            }
            //If the weapon is anything else
            else
            {
                //Returns the wepaons ammo
                return currentWeapon.currentAmmo;
            }
        }
    }
}
