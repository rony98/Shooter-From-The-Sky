/*
 * Author: Rony Verch
 * File Name: FrameAnimation.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 23, 2015
 * Modified Date: January 20, 2015
 * Description: Frame Animation class for displaying the correct frame for each frame animation that is needed
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class FrameAnimation
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constant for the frame the weapons are thrown/shot at
        public const int KNIFE_THROW_FRAME = 8;
        public const int GUN_SHOOT_FRAME = 1;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Boolean for whether the current animation type is sprite sheet animation or frame by frame (pic by pic)
        private bool spriteAnimation = false;

        //Textures for the frames
        private Texture2D spriteSheet;
        private Texture2D[] framesTextures;

        //Source Rectangle for the sprite sheet and the displacement between each frame
        private Rectangle sourceRect;
        private Vector2 frameDisplacement;

        //The amount of frames that exist
        private int frameAmount;

        //The frame numbers where the y for the source rectangle changes (moving down). This represents the first frames to be on a new y value
        private int[] ySwitch;

        //The delay between frames, the current frame, and a counter for when the frames should switch
        public int FrameDelay { get; set; }
        public int CurrFrame { get; private set; }
        public int Count { get; private set; }

        //Booleans for the different frames that could be finished
        public bool ShootingFrameDone { get; set; }
        public bool ReloadFrameDone { get; set; }

        //Boolean for whether the current animation is for a knife
        public bool KnifeAnimation { get; set; }

        //Animation state enum
        public AnimationState AnimState { get; set; }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The sprite sheet texture, the source rectangle, frame amount, the frames 
        //     where the y for the source rectangle switches, and the frame delay
        //Post: The Frame Animation object is created
        //Desc: Constructor for sprite sheet frame animation
        public FrameAnimation(Texture2D spriteSheet, Rectangle sourceRect, int frameAmount, int[] ySwitch, int frameDelay)
        {
            //Sets the type of animation to sprite sheet animation
            spriteAnimation = true;

            //Sets all the values from the constructor to the global fields
            this.spriteSheet = spriteSheet;
            this.sourceRect = sourceRect;
            this.frameAmount = frameAmount;
            this.ySwitch = ySwitch;
            this.FrameDelay = frameDelay;

            //Sets the count
            Count = 0;

            //Sets the frame displacement for the source rectangle
            frameDisplacement = new Vector2(sourceRect.Width, sourceRect.Height);

            //Sets the frames that could be done to false
            ShootingFrameDone = false;
            ReloadFrameDone = false;

            //Sets the current animation to not being knife (starts with handgun)
            KnifeAnimation = false;
        }


        //Pre: The textures for every frame in an array and the delay between frames
        //Post: The Frame Animation object is created
        //Desc: Constructor for frame by frame (pic by pic) animation
        public FrameAnimation(Texture2D[] framesTextures, int frameDelay)
        {
            //Sets all the values from the constructor to the global fields
            this.framesTextures = framesTextures;
            this.FrameDelay = frameDelay;

            //Sets the animation state
            AnimState = AnimationState.Idle;

            //Sets the frames that could be done to false
            ShootingFrameDone = false;
            ReloadFrameDone = false;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Updates the current frame that is needed to be drawn
        public void Update()
        {
            //If the count is greater then or equal to the delay
            if (Count >= FrameDelay)
            {
                //Update the count and the current frame
                Count = 0;
                CurrFrame++;

                //If the current animation type is sprite sheet animation
                if (spriteAnimation)
                {
                    //If the current frame is less then the amount of frames
                    if (CurrFrame < frameAmount)
                    {
                        //Temp variable for whether the source rect was changed
                        bool isChanged = false;

                        //Loop for every frame where the y could switch
                        for (int i = 0; i < ySwitch.Length; i++)
                        {
                            //If the current frame is one where the y could switch
                            if (CurrFrame == ySwitch[i])
                            {
                                //Update the temp variable to represent that the source rect was changed and the source rect with new values
                                isChanged = true;
                                sourceRect.X = 0;
                                sourceRect.Y += (int)frameDisplacement.Y;

                                break;
                            }
                        }

                        //If the source rect wasn't changed
                        if (!isChanged)
                        {
                            //Update the source rect
                            sourceRect.X += (int)frameDisplacement.X;
                        }
                    }
                    else
                    {
                        //Updates the current frame and source rect
                        CurrFrame = 0;
                        sourceRect.X = 0;
                        sourceRect.Y = 0;
                    }
                }
                else
                {
                    //If the current frame is greater then or equal to the amount of frames there are
                    if (CurrFrame >= framesTextures.Length)
                    {
                        //Set the current frame to 0 to reset the cycle
                        CurrFrame = 0;

                        //If the current animation state is either reload or shoot, set the frames done accordingly
                        if (AnimState == AnimationState.Reload && !KnifeAnimation)
                        {
                            ReloadFrameDone = true;
                        }
                    }
                }
            }

            //
            if (AnimState == AnimationState.Shoot)
            {
                //If the current animation is for the knife
                if (KnifeAnimation)
                {
                    //If the current frame is the one the knife gets thrown at
                    if (CurrFrame == KNIFE_THROW_FRAME && Count == 0)
                    {
                        ShootingFrameDone = true;
                    }
                }
                //If the current animation is for any other weapon
                else
                {
                    //If the current frame is the one the gun gets shots at
                    if (CurrFrame == GUN_SHOOT_FRAME && Count == 0)
                    {
                        ShootingFrameDone = true;
                    }
                }
            }

            //Increment the count
            Count++;
        }


        //Draws the current frame
        public void Draw(SpriteBatch sb, Rectangle bounds)
        {
            //Draws the current frame using either a source rect or a index of a texture array at the location given
            if (spriteAnimation)
            {
                sb.Draw(spriteSheet, bounds, sourceRect, Color.White);
            }
            else
            {
                sb.Draw(framesTextures[CurrFrame], bounds, Color.White);
            }
        }


        //Draws the current frame with a rotation 
        public void Draw(SpriteBatch sb, Rectangle bounds, Vector2 position, float rotationAngle, Vector2 rotationOrigin, float scale)
        {
            //Draws the current frame using either a source rect or a index of a texture array at the location given
            if (spriteAnimation)
            {
                sb.Draw(spriteSheet, bounds, sourceRect, Color.White, rotationAngle, rotationOrigin, SpriteEffects.None, 0);
            }
            else
            {
                sb.Draw(framesTextures[CurrFrame], position + rotationOrigin, null, Color.White, rotationAngle, rotationOrigin, scale, SpriteEffects.None, 1);
            }
        }


        //Pre: The new textures and the new frame delay
        //Post: The frames that are being drawn are reset and new frames are drawn
        //Desc: A method which resets the frames that are being drawn and sets new frames to be drawn
        public void ResetFrame(Texture2D[] newTextures, int newFrameDelay)
        {
            //Sets the new textures and frame delay
            framesTextures = newTextures;
            FrameDelay = newFrameDelay;

            //Resets the current frame and count
            Count = 0;
            CurrFrame = 0;
        }


        //Pre: None
        //Post: The texture that is currently being used is returned
        //Desc: A method that returns the texture that is currently being used
        public Texture2D GetTexture()
        {
            //Returns the current texturek
            return framesTextures[CurrFrame];
        }
    }
}
