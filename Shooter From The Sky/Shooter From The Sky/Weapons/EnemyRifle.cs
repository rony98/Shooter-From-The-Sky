/*
 * Author: Rony Verch
 * File Name: EnemyRifle.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 30, 2015
 * Modified Date: January 20, 2015
 * Description: The rifle that the enemies use. This rifle is the only weapon that enemies use and is much weaker then the player's rifle because there is only one player but 
 *              multiple enemies who are unlikely to miss because they aim directly at the player. The only way they miss is if the player dodges the enemies bullets.
 */

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class EnemyRifle : Weapon
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constants for the different sprite amount
        public const int IDLE_SPRITES_AMOUNT = 20;
        public const int MOVE_SPRITES_AMOUNT = 20;
        public const int RELOAD_SPRITES_AMOUNT = 20;
        public const int SHOOT_SPRITES_AMOUNT = 3;

        //Constants for the bullet speed, bullet damage, and ammo capacity
        public const float BULLET_SPEED = 7.9f;
        public const int BULLET_DAMAGE = 10;
        public const int AMMO_CAPACITY = 12;

        //Constant for the delay between shots
        public const int SHOT_DELAY = 50;

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
        //Post: The enemy rifle is created
        //Desc: A constructor for the enemy rifle
        public EnemyRifle()
        {
            //Sets the ammo
            currentAmmo = AMMO_CAPACITY;
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
                idleTextures[i] = cm.Load<Texture2D>("Enemy Sprites\\Idle\\enemy-idle_rifle_" + i);
            }

            //Loads the moving sprites
            for (int i = 0; i < MOVE_SPRITES_AMOUNT; i++)
            {
                moveTextures[i] = cm.Load<Texture2D>("Enemy Sprites\\Move\\enemy-move_rifle_" + i);
            }

            //Loads the reload sprites
            for (int i = 0; i < RELOAD_SPRITES_AMOUNT; i++)
            {
                reloadtextures[i] = cm.Load<Texture2D>("Enemy Sprites\\Reload\\enemy-reload_rifle_" + i);
            }

            //Loads the shooting sprites
            for (int i = 0; i < SHOOT_SPRITES_AMOUNT; i++)
            {
                shootTextures[i] = cm.Load<Texture2D>("Enemy Sprites\\Shoot\\enemy-shoot_rifle_" + i);
            }
        }


        //Pre: The current animation state
        //Post: The texture2d array is returned containing all the textures that need to be drawn at the time
        //Desc: A method which is overridden and returns the texture array that needs to be drawn
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
