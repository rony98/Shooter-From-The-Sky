/*
 * Author: Rony Verch
 * File Name: TextBox.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 24, 2015
 * Modified Date: January 20, 2015
 * Description: Textbox class for drawing and using a texbot in the game. The textbox allows the user to write stuff in it that is need for the game to function
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class TextBox
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


        //Rectangle for the position of the text box
        private Rectangle textBoxRect;

        //Vector2 for the position of the text in the text box
        private Vector2 textPos = new Vector2(0, 0);

        //Integers for the width and height of the screen
        private int screenWidth;
        private int screenHeight;

        //The font of the text box
        private SpriteFont textBoxFont;

        //Property for the text of the text box
        public string Text { get; set; }

        //Texbox surrounding colour, text colour, and clicked colour
        private Color backgroundColor;
        private Color clickedColor;
        private Color textColor;

        //Boolean property for if the current textbox is the one that's clicked on
        public bool TextBoxClicked { get; set; }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The rectangle of the textbox, the width/height of the screen, the text font, the text, and the three colours needed for the textbox
        //Post: The textbox is created
        //Desc: A constructor for the textbox
        public TextBox(Rectangle textBoxRect, int screenWidth, int screenHeight, SpriteFont textBoxFont, string text,
            Color backgroundColor, Color clickedColor, Color textColor)
        {
            //The textbox is not set to clicked
            TextBoxClicked = false;

            //All the textbox data is set
            this.textBoxRect = textBoxRect;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.textBoxFont = textBoxFont;
            this.Text = text;
            this.backgroundColor = backgroundColor;
            this.clickedColor = clickedColor;
            this.textColor = textColor;

            //The textbox text is aligned to the location of the textbox
            AlignText();
        }

        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: The text is aligned to the left (x) and center (y) of the text box
        //Desc: A method for aligning the text of the text box
        public void AlignText()
        {
            //Updates the text position
            textPos = new Vector2(textBoxRect.X + X_SIDE_DISPLACEMENT, textBoxRect.Y + (int)(textBoxRect.Height / 2) - (int)(textBoxFont.MeasureString(Text).Y / 2));
        }

        //Pre: None
        //Post: The text box's X is centered
        //Desc: A method for centering the text box X
        public void CenterTextBoxX()
        {
            //Updates the text box position
            textBoxRect = new Rectangle((int)(screenWidth / 2) - (int)(textBoxRect.Width / 2), textBoxRect.Y, textBoxRect.Width, textBoxRect.Height);

            //Aligns the text to the new position
            AlignText();
        }


        //Pre: None
        //Post: The text box's Y is centered
        //Desc: A method for centering the text box Y
        public void CenterTextBoxY()
        {
            //Updates the text box position
            textBoxRect = new Rectangle(textBoxRect.X, (int)(screenHeight / 2) - (int)(textBoxRect.Height / 2), textBoxRect.Width, textBoxRect.Height);

            //Aligns the text to the new position
            AlignText();
        }


        //Pre: None
        //Post: The textbox is aligned to the left of the screen
        //Desc: A method for aligning the textbox to the left of the screen
        public void AlignTextBoxLeft()
        {
            //Updates the text box position
            textBoxRect = new Rectangle(X_SIDE_DISPLACEMENT, textBoxRect.Y, textBoxRect.Width, textBoxRect.Height);

            //Aligns the text to the new position
            AlignText();
        }


        //Pre: None
        //Post: The textbox is aligned to the right of the screen
        //Desc: A method for aligning the textbox to the right of the screen
        public void AlignTextBoxRight()
        {
            //Updates the text box position
            textBoxRect = new Rectangle(screenWidth - textBoxRect.Width - X_SIDE_DISPLACEMENT, textBoxRect.Y, textBoxRect.Width, textBoxRect.Height);

            //Aligns the text to the new position
            AlignText();
        }


        //Pre: None
        //Post: The textbox is aligned to the top of the screen
        //Desc: A method for aligning the textbox to the top of the screen
        public void AlignTextBoxTop()
        {
            //Updates the text box position
            textBoxRect = new Rectangle(textBoxRect.X, Y_SIDE_DISPLACEMENT, textBoxRect.Width, textBoxRect.Height);

            //Aligns the text to the new position
            AlignText();
        }


        //Pre: None
        //Post: The textbox is aligned to the bottom of the screen
        //Desc: A method for aligning the textbox to the bottom of the screen
        public void AlignTextBoxBottom()
        {
            //Updates the text box position
            textBoxRect = new Rectangle(textBoxRect.X, screenHeight - Y_SIDE_DISPLACEMENT - textBoxRect.Height, textBoxRect.Width, textBoxRect.Height);

            //Aligns the text to the new position
            AlignText();
        }


        //Pre: None
        //Post: The rectangle for the text box is returned
        //Desc: A get method for the rectangle of the text box
        public Rectangle GetCurrentRectangle()
        {
            //Returns the rectangle
            return textBoxRect;
        }


        //Pre: The spritebatch and a blank texture
        //Post: The textbox is drawn
        //Desc: A method for drawing the textbox
        public void Draw(SpriteBatch sb, Texture2D blankTexture)
        {
            //If the text button is currently clicked, the background is drawn with one colours, otherwise it is drawn with another
            if (TextBoxClicked)
            {
                sb.Draw(blankTexture, textBoxRect, clickedColor);
            }
            else
            {
                sb.Draw(blankTexture, textBoxRect, backgroundColor);
            }

            //The text within the textbox is drawn
            sb.DrawString(textBoxFont, Text, textPos, textColor);
        }
    }
}
