/*
 * Author: Rony Verch
 * File Name: Human.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 21, 2015
 * Modified Date: January 20, 2015
 * Description: Class for the human which is the parent of both the enemy and the player. The human is used for data and methos that both have enemy and player use.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class Human
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constants for the delay between frames for different player/enemy states
        protected const int IDLE_FRAME_DELAY = 4;
        protected const int MOVE_FRAME_DELAY = 2;
        protected const int RELOAD_FRAME_DELAY = 4;
        protected const int SHOOT_FRAME_DELAY = 10;
        protected const int KNIFE_THROW_FRAME_DELAY = 4;

        //Constants for different amount of frames for different texture sprites
        protected const int SHOOT_FRAME_AMOUNT = 2;
        protected const int KNIFE_THROW_FRAME_AMOUNT = 23;

        //Constant for the human speed
        public const int HUMAN_SPEED = 3;

        //Constants for the health bar size
        public const int HEALTH_BAR_X_SIZE = 80;
        public const int HEALTH_BAR_Y_SIZE = 15;

        //Constant for the displacement between the health bar and the human
        public const int HEALTH_BAR_DISPLACEMENT_Y = 20;
        public const int HEALTH_BAR_DISPLACEMENT_X = 32;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Position and bounds of the human
        public Vector2 Position { get; protected set; }
        protected Rectangle bounds;

        //Frame animation class used to help with doing all the frame animation
        protected FrameAnimation frameAnimation;

        //Boolean property for whether the human is moving currently
        public bool HumanMoving { get; protected set; }

        //Int variable for the delay between weapon shots
        protected int shootDelayCount = 0;

        //Property for the rotation angle of the human
        protected float rotationAngle = 0;

        //Vector2 for the rotation origin
        protected Vector2 rotationOrigin = new Vector2(0, 0);

        //Property for the health of the human and the rectangle to draw it
        public int Health { get; set; }
        private Rectangle healthBarRedRect = new Rectangle(0, 0, HEALTH_BAR_X_SIZE, HEALTH_BAR_Y_SIZE);
        private Rectangle healthBarGreenRect = new Rectangle(0, 0, HEALTH_BAR_X_SIZE, HEALTH_BAR_Y_SIZE);


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: A human is created
        //Desc: A constructor for the human
        public Human()
        {
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: The bounds are set for the human
        //Desc: A method that sets the bounds of the human for drawing
        protected void SetBounds()
        {
            //Updates the bounds of the human
            bounds.X = (int)Position.X;
            bounds.Y = (int)Position.Y;
        }


        //Pre: The bounds of the human
        //Post: The bounds are returned
        //Desc: A method which returns the bounds of the human
        public Rectangle GetBounds()
        {
            //Returns the bounds of the human
            return bounds;
        }


        //Pre: None
        //Post: The human is set to moving
        //Desc: A method for setting the human to moving
        public virtual void SetHumanMoving()
        {
            //Sets the human to moving and updates the frame animation state
            HumanMoving = true;
            frameAnimation.AnimState = AnimationState.Move;
        }


        //Pre: None
        //Post: The human is set to being idle
        //Desc: A method for settingt he human to idle
        public virtual void SetHumanIdle()
        {
            //Sets the human to not moving and updates the frame animation state
            HumanMoving = false;
            frameAnimation.AnimState = AnimationState.Idle;
        }


        //Pre: The x and y position the player is looking towards
        //Post: The rotation angle is calculated for the human
        //Desc: A method for calculating the rotation angle for the human
        public void CalcRotation(float xPos, float yPos, float displacementX, float displacementY)
        {
            //The center point for the human is calculated
            float imageCenterY = bounds.Height / 2;
            float imageCenterX = bounds.Width / 2;

            //The angle for the human is calculated and the rotation angle is set
            rotationAngle = (float)(Math.Atan2(yPos - Position.Y - displacementY, xPos - Position.X - displacementX));
            rotationOrigin = new Vector2(imageCenterX, imageCenterY);
        }


        //Pre: None
        //Post: The current texture that is being drawn for the human is returned
        //Desc: A method which returns the current texture of the human
        public Texture2D GetCurrentTexture()
        {
            //Returns the current texture
            return frameAnimation.GetTexture();
        }


        //Pre: None
        //Post: Returns the animation state
        //Desc: A method which returns the current animation state of the human
        public AnimationState GetCurrentAnimState()
        {
            //Returns the animation state
            return frameAnimation.AnimState;
        }

        //Pre: None
        //Post: The health bar location is set
        //Desc: A method for setting the health bar location
        private void SetHealthBarLocation()
        {
            //The red health bar bounds are calculated and set
            healthBarRedRect.X = (int)Position.X + HEALTH_BAR_DISPLACEMENT_X - (HEALTH_BAR_X_SIZE / 2);
            healthBarRedRect.Y = (int)Position.Y - HEALTH_BAR_DISPLACEMENT_Y;

            //The green health bar bounds are calculated and set
            healthBarGreenRect.X = healthBarRedRect.X;
            healthBarGreenRect.Y = healthBarRedRect.Y;
            healthBarGreenRect.Width = (int)((decimal)Health / (decimal)100 * (decimal)HEALTH_BAR_X_SIZE);
        }


        //Pre: The spritebatch and a blank texture
        //Post: The health bar is drawn
        //Desc: A method for drawing the health bar of the human
        protected void DrawHealthBar(SpriteBatch sb, Texture2D blankTexture)
        {
            //The health bar location is set
            SetHealthBarLocation();

            //The health bar is drawn 
            sb.Draw(blankTexture, healthBarRedRect, Color.Red);
            sb.Draw(blankTexture, healthBarGreenRect, Color.Green);
        }
    }
}
