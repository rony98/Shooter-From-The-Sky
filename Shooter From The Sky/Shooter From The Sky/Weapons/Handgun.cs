/*
 * Author: Rony Verch
 * File Name: Handgun.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 30, 2015
 * Modified Date: January 20, 2015
 * Description: The handgun that the player uses. The handgun is a relatively weak weapon compared to the rifle and shotgun but it could still be used by the player.
 */

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class Handgun : Weapon
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constants for the different sprite amount
        public const int IDLE_SPRITES_AMOUNT = 20;
        public const int MOVE_SPRITES_AMOUNT = 20;
        public const int RELOAD_SPRITES_AMOUNT = 15;
        public const int SHOOT_SPRITES_AMOUNT = 3;

        //Constants for the bullet speed, bullet damage, and ammo capacity
        public const float BULLET_SPEED = 6.8f;
        public const int BULLET_DAMAGE = 60;
        public const int AMMO_CAPACITY = 20;

        //Constant for the delay between shots
        public const int SHOT_DELAY = 30;

        //Constants for the displacement from the start of the image to where the bullet gets shot
        public const int BULLET_DISPLACEMENT_X = 62;
        public const int BULLET_DISPLACEMENT_Y = 51;


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
        //Post: The handgun is created
        //Desc: A constructor for the hand gun
        public Handgun()
        {
            //Sets the ammo and the weapon type
            currentAmmo = AMMO_CAPACITY;
            WeaponType = WeaponTypes.Handgun;
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
                idleTextures[i] = cm.Load<Texture2D>("Player Sprites\\Handgun\\Idle\\survivor-idle_handgun_" + i);
            }

            //Loads the moving sprites
            for (int i = 0; i < MOVE_SPRITES_AMOUNT; i++)
            {
                moveTextures[i] = cm.Load<Texture2D>("Player Sprites\\Handgun\\Move\\survivor-move_handgun_" + i);
            }

            //Loads the reload sprites
            for (int i = 0; i < RELOAD_SPRITES_AMOUNT; i++)
            {
                reloadtextures[i] = cm.Load<Texture2D>("Player Sprites\\Handgun\\Reload\\survivor-reload_handgun_" + i);
            }

            //Loads the shooting sprites
            for (int i = 0; i < SHOOT_SPRITES_AMOUNT; i++)
            {
                shootTextures[i] = cm.Load<Texture2D>("Player Sprites\\Handgun\\Shoot\\survivor-shoot_handgun_" + i);
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
