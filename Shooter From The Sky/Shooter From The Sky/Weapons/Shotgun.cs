/*
 * Author: Rony Verch
 * File Name: Shotgun.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 30, 2015
 * Modified Date: January 20, 2015
 * Description: The shotgun is one of the strongest weapons in the game. It shoots 3 bullets at once which begin to spread as they move away from the player.
 */

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class Shotgun : Weapon
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constants for the different sprite amount
        const int IDLE_SPRITES_AMOUNT = 20;
        const int MOVE_SPRITES_AMOUNT = 20;
        const int RELOAD_SPRITES_AMOUNT = 20;
        const int SHOOT_SPRITES_AMOUNT = 3;

        //Constants for the bullet speed, bullet damage, and ammo capacity
        public const float BULLET_SPEED = 8.6f;
        public const int BULLET_DAMAGE = 60;
        public const int AMMO_CAPACITY = 10;

        //Constant for the delay between shots
        public const int SHOT_DELAY = 80;

        //Constants for the displacement from the start of the image to where the bullet gets shot
        public const int BULLET_DISPLACEMENT_X = 63;
        public const int BULLET_DISPLACEMENT_Y = 48;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Static arrays with textures used for drawing the player with the handgun
        static private Texture2D[] idleTextures = new Texture2D[IDLE_SPRITES_AMOUNT];
        static private Texture2D[] moveTextures = new Texture2D[MOVE_SPRITES_AMOUNT];
        static private Texture2D[] reloadtextures = new Texture2D[RELOAD_SPRITES_AMOUNT];
        static private Texture2D[] shootTextures = new Texture2D[SHOOT_SPRITES_AMOUNT];


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: The shotgun is created
        //Desc: A constructor for the shotgun
        public Shotgun()
        {
            //Sets the ammo and the weapon type
            currentAmmo = AMMO_CAPACITY;
            WeaponType = WeaponTypes.Shotgun;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The content manager
        //Post: The texture data is loaded
        //Desc: A static method which loads all the texture data for the class
        public static void SetTextureData(ContentManager cm)
        {
            //Loads the Idle sprites
            for (int i = 0; i < IDLE_SPRITES_AMOUNT; i++)
            {
                idleTextures[i] = cm.Load<Texture2D>("Player Sprites\\Shotgun\\Idle\\survivor-idle_shotgun_" + i);
            }

            //Loads the moving sprites
            for (int i = 0; i < MOVE_SPRITES_AMOUNT; i++)
            {
                moveTextures[i] = cm.Load<Texture2D>("Player Sprites\\Shotgun\\Move\\survivor-move_shotgun_" + i);
            }

            //Loads the reload sprites
            for (int i = 0; i < RELOAD_SPRITES_AMOUNT; i++)
            {
                reloadtextures[i] = cm.Load<Texture2D>("Player Sprites\\Shotgun\\Reload\\survivor-reload_shotgun_" + i);
            }

            //Loads the shooting sprites
            for (int i = 0; i < SHOOT_SPRITES_AMOUNT; i++)
            {
                shootTextures[i] = cm.Load<Texture2D>("Player Sprites\\Shotgun\\Shoot\\survivor-shoot_shotgun_" + i);
            }
        }


        //Pre: The current animation state
        //Post: The texture2d array is returned containing all the textures that need to be drawn at the time
        //Desc: A method which returns the texture array that needs to be drawn
        public override Texture2D[] GetTextures(AnimationState animState)
        {
            //Switch statement for the current animation state that adds a different texture array depending on it's value
            switch (animState)
            {
                case AnimationState.Idle:
                    return idleTextures;

                case AnimationState.Move:
                    return moveTextures;

                case AnimationState.Reload:
                    return reloadtextures;

                case AnimationState.Shoot:
                    return shootTextures;

                default:
                    return null;
            }
        }
    }
}
