/*
 * Author: Rony Verch
 * File Name: Label.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 23, 2015
 * Modified Date: January 20, 2015
 * Description: Label class that is used to draw text throughout the game
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class Label
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constants for the displacement between the sides of the screen
        public const int X_SIDE_DISPLACEMENT = 20;
        public const int Y_SIDE_DISPLACEMENT = 20;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Vector2 for the position of the label
        private Vector2 labelPos;

        //Integers for the width and height of the screen
        private int screenWidth;
        private int screenHeight;

        //The font of the label
        private SpriteFont labelFont;

        //The text of the label
        private string text;

        //The colour for the text
        private Color colour;


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The text, screen width, screen height, the position of the label, the font of the label, and the colour of the text
        //Post: The label is created
        //Desc: A constructor for the label
        public Label(string text, int screenWidth, int screenHeight, Vector2 labelPos, SpriteFont labelFont, Color colour)
        {
            //Sets all the values for the label
            this.labelPos = labelPos;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.labelFont = labelFont;
            this.text = text;
            this.colour = colour;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: The text's X is centered
        //Desc: A method for centering the text
        public void CenterTextX()
        {
            //Updates the labels position
            labelPos = new Vector2((screenWidth / 2) - (labelFont.MeasureString(text).X / 2), labelPos.Y);
        }


        //Pre: None
        //Post: The text's Y is centered
        //Desc: A method for centering the text
        public void CenterTextY()
        {
            //Updates the labels position
            labelPos = new Vector2(labelPos.X, (screenHeight / 2) - (labelFont.MeasureString(text).Y / 2));
        }


        //Pre: None
        //Post: The text's X is centered with a displacement
        //Desc: A method for centering the text with a displacement
        public void CenterTextXDisplacement(Vector2 displacement)
        {
            //Updates the labels position
            labelPos = new Vector2((screenWidth / 2) - (labelFont.MeasureString(text).X / 2), labelPos.Y);
            labelPos += displacement;
        }


        //Pre: None
        //Post: The text is aligned to the left of the screen
        //Desc: A method for aligning the text to the left of the screen
        public void AlignLeft()
        {
            //Updates the labels position
            labelPos = new Vector2(X_SIDE_DISPLACEMENT, labelPos.Y);
        }


        //Pre: None
        //Post: The text is aligned to the right of the screen
        //Desc: A method for aligning the text to the right of the screen
        public void AlignRight()
        {
            //Updates the labels position
            labelPos = new Vector2(screenWidth - labelFont.MeasureString(text).X - X_SIDE_DISPLACEMENT, labelPos.Y);
        }


        //Pre: None
        //Post: The text is aligned to the top of the screen
        //Desc: A method for aligning the text to the top of the screen
        public void AlignTop()
        {
            //Updates the labels position
            labelPos = new Vector2(labelPos.X, Y_SIDE_DISPLACEMENT);
        }


        //Pre: None
        //Post: The text is aligned to the bottom of the screen
        //Desc: A method for aligning the text to the bottom of the screen
        public void AlignBottom()
        {
            //Updates the labels position
            labelPos = new Vector2(labelPos.X, screenHeight - Y_SIDE_DISPLACEMENT - labelFont.MeasureString(text).Y);
        }


        //Pre: The spritebatch
        //Post: The label is drawn
        //Desc: A method for drawing the label
        public void Draw(SpriteBatch sb)
        {
            //Draws the label
            sb.DrawString(labelFont, text, labelPos, colour);
        }
    }
}
