/*
 * Author: Rony Verch
 * File Name: Button.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 25, 2015
 * Modified Date: January 20, 2015
 * Description: Button class for drawing the buttons that are used throughout the game
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
    class Button
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Rectangles of the button border and outline
        private Rectangle buttonBorderRect;
        private Rectangle buttonOutlineRect;

        //Rectangles of the enlarged button border and outline
        private Rectangle buttonBorderEnlargedRect;
        private Rectangle buttonOutlineEnlargedRect;

        //The amount the button gets enlarged by
        private Vector2 buttonEnlargeAmount;

        //Colours of the button border and outline
        private Color buttonBorderColor;
        private Color buttonOutlineColor;

        //Variables for the text of the button
        private string buttonText;
        private SpriteFont buttonFont;
        private SpriteFont buttonEnlargeFont;
        private Vector2 buttonTextLocation;
        private Vector2 textEnlargeLocation;
        private Color buttonTextColor;
        private Color buttonTextHoverColor;

        //State button changes to
        private GameState buttonStateChange;

        //Texture for drawing the button
        private Texture2D buttonBorderTexture;
        private Texture2D buttonOutlineTexture;

        //Boolean for whether the mouse is hovering over the button and whether the current button was clicked
        private bool isHovering = false;
        public bool isClicked { get; set; }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The button attributes
        //Post: A button is created
        //Desc: A constructor for a button to be made
        public Button(Rectangle buttonBorderRect, Rectangle buttonOutlineRect, Vector2 buttonEnlargeAmount, Color buttonBorderColor, Color buttonOutlineColor,
            string buttonText, SpriteFont buttonFont, SpriteFont buttonEnlargeFont, Color buttonTextColor, Color buttonTextHoverColor, GameState buttonStateChange)
        {
            //Apply all the constructors data to the attributes of the class
            this.buttonBorderRect = buttonBorderRect;
            this.buttonOutlineRect = buttonOutlineRect;
            this.buttonEnlargeAmount = buttonEnlargeAmount;
            this.buttonBorderColor = buttonBorderColor;
            this.buttonOutlineColor = buttonOutlineColor;
            this.buttonText = buttonText;
            this.buttonFont = buttonFont;
            this.buttonEnlargeFont = buttonEnlargeFont;
            this.buttonTextColor = buttonTextColor;
            this.buttonTextHoverColor = buttonTextHoverColor;
            this.buttonStateChange = buttonStateChange;

            //Sets the button to not being clicked yet
            isClicked = false;

            //Calculates button and text locations 
            CalculateEnlargedButton();
            CalculateTextLocation();
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The rectangle for the button if it's hovring and if it's not
        //Post: The rectangle is returned
        //Desc: A subprogram that returns the rectangle of the button if it's hovering and if it's not
        public Rectangle GetCurrentRectangle()
        {
            //If the button is being hovered over
            if (isHovering)
            {
                //Return the hovering rectangle
                return buttonOutlineEnlargedRect;
            }
            //If the button is not being hoevered over
            else
            {
                //Return the regular rectangle
                return buttonOutlineRect;
            }
        }


        //Pre: The current game state
        //Post: The game state is returned
        //Desc: A subprogram that returns the current game state
        public GameState GetGameState()
        {
            //Returns the game state
            return buttonStateChange;
        }


        //Pre: If the button is changing to be hovered or not
        //Post: Whether the button is hovering is set
        //Desc: A subprogram that sets whether the current button is being hovered over or not
        public void SetHovering(bool isHovering)
        {
            //Set whether it's hovering
            this.isHovering = isHovering;
        }


        //Pre: The graphics device manager
        //Post: The texture data is set for the button
        //Desc: A subprogram that sets the texture data for the button
        public void SetTextureData(GraphicsDeviceManager graphics)
        {
            //Gives a value to the border and outline of the buttons texture
            buttonBorderTexture = new Texture2D(graphics.GraphicsDevice, 80, 30);
            buttonOutlineTexture = new Texture2D(graphics.GraphicsDevice, 80, 30);

            //Sets the color data for the outline and border
            Color[] dataBorder = new Color[80 * 30];
            Color[] dataOutline = new Color[80 * 30];

            //For loop that sets the color data for the outline and border
            for (int i = 0; i < dataBorder.Length; i++)
            {
                //Sets the colour data for the outline and border
                dataBorder[i] = Color.White;
                dataOutline[i] = Color.Black;
            }

            //Sets the data to the outline and the border
            buttonBorderTexture.SetData(dataOutline);
            buttonOutlineTexture.SetData(dataBorder);
        }


        //Pre: None
        //Post: The enlarged button rectangle is calculated
        //Desc: A subprogram that calculates the enlarged button rectangle
        private void CalculateEnlargedButton()
        {
            //Enalrged button rectangle is calculated
            buttonBorderEnlargedRect = new Rectangle(buttonBorderRect.X - Convert.ToInt32(buttonEnlargeAmount.X / 2), buttonBorderRect.Y - Convert.ToInt32(buttonEnlargeAmount.Y / 2),
                buttonBorderRect.Width + Convert.ToInt32(buttonEnlargeAmount.X), buttonBorderRect.Height + Convert.ToInt32(buttonEnlargeAmount.Y));
            buttonOutlineEnlargedRect = new Rectangle(buttonOutlineRect.X - Convert.ToInt32(buttonEnlargeAmount.X / 2), buttonOutlineRect.Y - Convert.ToInt32(buttonEnlargeAmount.Y / 2),
                buttonOutlineRect.Width + Convert.ToInt32(buttonEnlargeAmount.X), buttonOutlineRect.Height + Convert.ToInt32(buttonEnlargeAmount.Y));
        }


        //Pre: None
        //Post: The text location is calculated
        //Desc: A subprogram that calculates the text location
        private void CalculateTextLocation()
        {
            //Center of a button is calculated
            Vector2 centerRegButton = new Vector2(buttonBorderRect.X + Convert.ToInt32(buttonBorderRect.Width / 2),
                buttonBorderRect.Y + Convert.ToInt32(buttonBorderRect.Height / 2));
            Vector2 centerEnlargeButton = new Vector2(buttonBorderEnlargedRect.X + Convert.ToInt32(buttonBorderEnlargedRect.Width / 2),
                buttonBorderEnlargedRect.Y + Convert.ToInt32(buttonBorderEnlargedRect.Height / 2));

            //The location of the text is calculated
            buttonTextLocation = new Vector2(centerRegButton.X - Convert.ToInt32(buttonFont.MeasureString(buttonText).X / 2), centerRegButton.Y -
                Convert.ToInt32(buttonFont.MeasureString(buttonText).Y / 2));
            textEnlargeLocation = new Vector2(centerEnlargeButton.X - Convert.ToInt32(buttonEnlargeFont.MeasureString(buttonText).X / 2), centerEnlargeButton.Y -
                Convert.ToInt32(buttonEnlargeFont.MeasureString(buttonText).Y / 2));
        }

        //Pre: The spritebatch
        //Post: The button is drawn
        //Desc: A subprogram that draws the button
        public void DrawButton(SpriteBatch spriteBatch)
        {
            //If the button is being hoevered over
            if (isHovering)
            {
                //Draw the button which is hovered over
                spriteBatch.Draw(buttonOutlineTexture, buttonOutlineEnlargedRect, buttonOutlineColor);
                spriteBatch.Draw(buttonBorderTexture, buttonBorderEnlargedRect, buttonBorderColor);

                spriteBatch.DrawString(buttonEnlargeFont, buttonText, textEnlargeLocation, buttonTextHoverColor);
            }
            //If the button is not being hovered over
            else
            {
                //Draw the button that is not being hoevered over
                spriteBatch.Draw(buttonOutlineTexture, buttonOutlineRect, buttonOutlineColor);
                spriteBatch.Draw(buttonBorderTexture, buttonBorderRect, buttonBorderColor);

                spriteBatch.DrawString(buttonFont, buttonText, buttonTextLocation, buttonTextColor);
            }
        }


        //Pre: None
        //Post: The text for the current button is returned
        //Desc: A getter method for the text of the button
        public string GetText()
        {
            //Returns the button's text
            return buttonText;
        }
    }
}
