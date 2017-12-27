/*
 * Author: Rony Verch
 * File Name: ThrowingKnife.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 30, 2015
 * Modified Date: January 20, 2015
 * Description: The throwing knife kills each enemy with only one knife but, its speed is relatively slow compared to other weapons.
 */

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class ThrowingKnife : Weapon
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constants for the different sprite amount
        const int IDLE_SPRITES_AMOUNT = 20;
        const int MOVE_SPRITES_AMOUNT = 20;
        const int THROW_SPRITES_AMOUNT = 24;

        //Constants for the knife speed and knife damage
        public const float KNIFE_SPEED = 6.6f;
        public const int KNIFE_DAMAGE = 100;

        //Constant for the delay between shots
        public const int SHOT_DELAY = 70;

        //Constants for the displacement from the start of the image to where the knife gets thrown
        public const int KNIFE_DISPLACEMENT_X = 56;
        public const int KNIFE_DISPLACEMENT_Y = 30;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Static arrays with textures used for drawing the player with the handgun
        static private Texture2D[] idleTextures = new Texture2D[IDLE_SPRITES_AMOUNT];
        static private Texture2D[] moveTextures = new Texture2D[MOVE_SPRITES_AMOUNT];
        static private Texture2D[] throwTextures = new Texture2D[THROW_SPRITES_AMOUNT];


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: The throwing knife is created
        //Desc: A constructor for the throwing knife
        public ThrowingKnife()
        {
            //The weapon type is set
            WeaponType = WeaponTypes.ThrowingKnife;
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
                idleTextures[i] = cm.Load<Texture2D>("Player Sprites\\Throwing Knife\\Idle\\survivor-idle_knife_" + i);
            }

            //Loads the moving sprites
            for (int i = 0; i < MOVE_SPRITES_AMOUNT; i++)
            {
                moveTextures[i] = cm.Load<Texture2D>("Player Sprites\\Throwing Knife\\Move\\survivor-move_knife_" + i);
            }

            //Loads the throwing of the knife sprites
            for (int i = 0; i < THROW_SPRITES_AMOUNT; i++)
            {
                throwTextures[i] = cm.Load<Texture2D>("Player Sprites\\Throwing Knife\\Throw\\survivor-throw_knife_" + i);
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

                case AnimationState.Shoot:
                    return throwTextures;

                default:
                    return null;
            }
        }
    }
}
