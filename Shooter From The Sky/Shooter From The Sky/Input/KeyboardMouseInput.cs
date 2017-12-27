/*
 * Author: Rony Verch
 * File Name: KeyboardMouseInput.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 23, 2015
 * Modified Date: January 20, 2015
 * Description: Keyboard Mouse Input class for checking the input from the user, whether it is the user typing on their keyboard or clicking/moving their mouse.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camera2D_XNA4;

namespace Shooter_From_The_Sky
{
    class KeyboardMouseInput
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constant for the max size of a number
        public const int MAX_NUM_TEXT_SIZE = 3;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Keyboard variables
        private KeyboardState keyboardState;
        private Keys[] pressedKeys = new Keys[] { Keys.None };
        private Keys[] prevPressedKeys = new Keys[] { Keys.None };

        //Mouse Variables
        public MouseState mouse { get; private set; }
        private bool mouseButtonHeld = false;
        private bool shiftKeyHeld = false;
        private bool controlKeyHeld = false;

        //Variable for what the user is typing
        public string userText { get; private set; }

        //Variable for the max a user can write
        private int maxTextSize;

        //Property for the tile type and whether the map creator needs to change the one it's using
        public TileType CurrentTileType { get; set; }

        //Variable for the position the camera should look at for the map creator
        private Vector2 mapCreatorPos;

        //Property for whether the the current object being placed is an enemy
        public bool PlacingEnemy { get; set; }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: The KeyboardMouseInput object is created
        //Desc: A constructor for the KeyboardMouseInput object that does not take in any information
        public KeyboardMouseInput()
        {
            //Sets the max text size to a default of 10, creates the user text, and sets the current tile to blank
            maxTextSize = 10;
            userText = "";
            CurrentTileType = TileType.Blank;
        }


        //Pre: The max text size
        //Post: The KeyboardMouseInput object is created with a max text size that is given
        //Desc: A constructor for the KeyboardMouseInput object that takes in the max text size
        public KeyboardMouseInput(int maxTextSize)
        {
            //Sets the max text size to the size that was given, creates the user text, and sets the current tile to blank
            this.maxTextSize = maxTextSize;
            userText = "";
            CurrentTileType = TileType.Blank;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The list of keys that need to be checked
        //Post: An integer is returned for how much the buttons need to be scrolled
        //Desc:
        public int ScrollButtons(List<Keys> keysToCheck)
        {
            //Updates the current keyboard state
            keyboardState = Keyboard.GetState();

            //Updates the keys which are pressed
            pressedKeys = keyboardState.GetPressedKeys();

            //Creates a integer that is used to score whether the buttons need to scroll up or down
            int tempInt = 0;

            //Loop for every key that was pressed
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                //If the key is valid
                if (CheckOldKeys(i, keysToCheck))
                {
                    //If the key is an up key, the buttons are set to scroll up
                    if (pressedKeys[i] == Keys.W || pressedKeys[i] == Keys.Up)
                    {
                        tempInt = 1;
                    }
                    //If the key is a down key, the buttons are set to scroll down
                    else if (pressedKeys[i] == Keys.S || pressedKeys[i] == Keys.Down)
                    {
                        tempInt = -1;
                    }
                }
            }

            //Set the previously pressed keys to the currently pressed keys
            prevPressedKeys = pressedKeys;

            //Returns whether to scroll the buttons up, down, or stay the same
            return tempInt;
        }


        //Pre: The List of buttons, the gamestate, the camera object, the screen height and the camera displacement
        //Post: The scrolling buttons are updated with whether they were clicked or not and the new gamestate is returned
        //Desc: A method for updating the scrolling buttons and whether they were clicked or not
        public GameState UpdateScrollingButtons(List<Button> buttons, GameState gameState, Cam2D camera, int screenHeight, int cameraDisplacement)
        {
            //Sets the mouses' state
            mouse = Mouse.GetState();

            //If the mouse button is released
            if (mouse.LeftButton == ButtonState.Released)
            {
                //Set it to not being held anymore
                mouseButtonHeld = false;
            }

            //Loop for every button
            for (int i = 0; i < buttons.Count; i++)
            {
                //Creates a rectangle that is used to store the location of the current button
                Rectangle tempRect;

                //If the current button is not the first button, the rectangle for it is set
                if (i != 0)
                {
                    tempRect = new Rectangle(buttons[i].GetCurrentRectangle().X, (buttons[i].GetCurrentRectangle().Y - (int)((camera.GetPosition().Y + cameraDisplacement) - (int)(screenHeight / 2))),
                        buttons[i].GetCurrentRectangle().Width, buttons[i].GetCurrentRectangle().Height);
                }
                //If the current button is the first button, the rectangle for it is set
                else
                {
                    tempRect = buttons[i].GetCurrentRectangle();
                }

                //If the current button is visible on the screen
                if (camera.IntersectsScreen(buttons[i].GetCurrentRectangle()) || buttons[i].GetText() == "Go Back")
                {
                    //The current button is checked for hovering
                    if (CheckButtonHovering(tempRect))
                    {
                        //The current button is checked if it was pressed
                        if (CheckButtonPressed(tempRect))
                        {
                            //Changes the states depending on the state stored in the button class and sets the button to clicked
                            gameState = buttons[i].GetGameState();
                            buttons[i].isClicked = true;
                        }
                        //If the current button was not pressed but is hovering, the current button is set to hovering
                        else
                        {
                            buttons[i].SetHovering(true);
                        }
                    }
                    //if the current button is hovering, the button is set to hovering
                    else
                    {
                        buttons[i].SetHovering(false);
                    }
                }
            }

            //Returns the game state
            return gameState;
        }


        //Pre: The list of buttons, and the gamestate
        //Post: The new gamestate is returned
        //Desc: A method for updating whether the current buttons were clicked or are being hovered over
        public GameState UpdateButtons(List<Button> buttons, GameState gameState)
        {
            //Sets the mouses' state
            mouse = Mouse.GetState();

            //If the mouse button is released
            if (mouse.LeftButton == ButtonState.Released)
            {
                //Set it to not being held anymore
                mouseButtonHeld = false;
            }

            //Loop for every button
            for (int i = 0; i < buttons.Count; i++)
            {
                //If the current button is being hovered over
                if (CheckButtonHovering(buttons[i].GetCurrentRectangle()))
                {
                    //If the current button is being pressed
                    if (CheckButtonPressed(buttons[i].GetCurrentRectangle()))
                    {
                        //Changes the states depending on the state stored in the button class
                        gameState = buttons[i].GetGameState();
                    }
                    //If the current button is not being pressed but is being hovered over, the button is set to being hovered over
                    else
                    {
                        buttons[i].SetHovering(true);
                    }
                }
                //If the current button is being hovered over, the button is set to being hovered over
                else
                {
                    buttons[i].SetHovering(false);
                }
            }

            //Returns the new gamestate
            return gameState;
        }


        //Pre: The list of textbox's
        //Post: The textbox's are updated
        //Desc: A method for updating whether textbox's are pressed and the typing should go into them
        public void UpdateTextBoxes(List<TextBox> textBoxs)
        {
            //Sets the mouses' state
            mouse = Mouse.GetState();

            //If the mouse button is released
            if (mouse.LeftButton == ButtonState.Released)
            {
                //Set it to not being held anymore
                mouseButtonHeld = false;
            }

            //Loop for every textbox
            for (int i = 0; i < textBoxs.Count; i++)
            {
                //If the current textbox is pressed on
                if (CheckButtonPressed(textBoxs[i].GetCurrentRectangle()))
                {
                    //Sets the current textbox to the opposite state that it was on before, if it was pressed it is set to unpressed and vice versa
                    textBoxs[i].TextBoxClicked = !textBoxs[i].TextBoxClicked;

                    //Loop for every textbox
                    for (int j = 0; j < textBoxs.Count; j++)
                    {
                        //If the current textbox is not the same one as was checked before the current textbox is set to unpressed
                        if (j != i)
                        {
                            textBoxs[j].TextBoxClicked = false;
                        }
                    }
                }
            }
        }


        //Pre: The list of keys that need to be checked, the current text that is being added to, and whether numbers are being added
        //Post: The keyboard keys are checked and the string of text is updated
        //Desc: A method for checking and updating the keyboard keys
        public void UpdateKeyboardTyping(List<Keys> keysToCheck, string currentText, bool numsAdding)
        {
            //Updates the current keyboard state
            keyboardState = Keyboard.GetState();

            //Updates the keys which are pressed
            pressedKeys = keyboardState.GetPressedKeys();

            //Sets the shift key and control key to not held
            shiftKeyHeld = false;
            controlKeyHeld = false;

            //Resets the text currently written
            userText = "";

            //Loop for every key that was pressed
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                //If the current key is a shift key
                if (pressedKeys[i] == Keys.LeftShift || pressedKeys[i] == Keys.RightShift)
                {
                    //Set the shift key to being held and reset the current key
                    shiftKeyHeld = true;
                    pressedKeys[i] = Keys.None;
                }
                //If the current key is the control key
                else if (pressedKeys[i] == Keys.RightControl || pressedKeys[i] == Keys.LeftControl)
                {
                    //Sets the control key to being held and resets the current key
                    controlKeyHeld = true;
                    pressedKeys[i] = Keys.None;
                }
            }

            //Loop for every key that was pressed
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                //If the current key is the back key
                if (pressedKeys[i] == Keys.Back && CheckOldKeys(i, keysToCheck))
                {
                    //If the user is currently holding a control key
                    if (controlKeyHeld)
                    {
                        //Resets the text
                        userText = "";
                        currentText = "";
                    }
                    else
                    {
                        //If the user wrote any text
                        if (userText != "")
                        {
                            //Remove the last character of what they wrote
                            userText = userText.Remove(userText.Length - 1);
                        }
                        //If the user had any text written previously
                        else if (currentText != "")
                        {
                            //Removes the last character of what the user had written before
                            currentText = currentText.Remove(currentText.Length - 1);
                        }
                    }
                }
                //If the current key is any other key
                else
                {
                    //If the key is something that can be used 
                    if (CheckOldKeys(i, keysToCheck))
                    {
                        //If numbers are currently being added
                        if (numsAdding)
                        {
                            //If the amount of text the user has already written isn't too much
                            if (userText.Length + currentText.Length < MAX_NUM_TEXT_SIZE)
                            {
                                //Update the text the user has written
                                userText += pressedKeys[i].ToString().ToUpper()[1];
                            }
                        }
                        //If any other keys are currently being added
                        else
                        {
                            //If the shift key is being held
                            if (shiftKeyHeld)
                            {
                                //If the amount of text the user has already written isn't too much
                                if (userText.Length + currentText.Length < maxTextSize)
                                {
                                    //Update the text the user has written
                                    userText += pressedKeys[i].ToString().ToUpper();
                                }
                            }
                            //If the shift key is not being held
                            else
                            {
                                //If the amount of text the user has already written isn't too much
                                if (userText.Length + currentText.Length < maxTextSize)
                                {
                                    //Update the text the user has written
                                    userText += pressedKeys[i].ToString().ToLower();
                                }
                            }
                        }
                    }
                }
            }

            //Updates the user text with the current text
            userText = currentText + userText;

            //Set the previously pressed keys to the currently pressed keys
            prevPressedKeys = pressedKeys;
        }


        //Pre: None
        //Post: The map creator position is reset and whether an enemy is being placed is set to false
        //Desc: A method for resetting the position of the mapcreator and setting whether an enemy is being placed to false
        public void ResetPosition()
        {
            //The mapcreator position is reset and the enemy being placed is set to false
            mapCreatorPos = new Vector2(0, 0);
            PlacingEnemy = false;
        }


        //Pre: The key that needs to be checked
        //Post: A boolean for whether the key is valid is returned
        //Desc: A method for checking whether a specific key is valid
        public bool CheckKey(Keys key)
        {
            //Updates the current keyboard state
            keyboardState = Keyboard.GetState();

            //Updates the keys which are pressed
            pressedKeys = keyboardState.GetPressedKeys();

            //Creates a list of keys and adds the key that needs to be checked to it
            List<Keys> keyToCheck = new List<Keys>();
            keyToCheck.Add(key);

            //Loop for every key that was pressed
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                //If the current key is valid
                if (CheckOldKeys(i, keyToCheck))
                {
                    //Set the previously pressed keys to the currently pressed keys
                    prevPressedKeys = pressedKeys;

                    //Returns that the key is valid
                    return true;
                }
            }

            //Set the previously pressed keys to the currently pressed keys
            prevPressedKeys = pressedKeys;

            //Returns that the key is invalid
            return false;
        }


        //Pre: None
        //Post: A boolean for whether the left mouse button is clicked
        //Desc: A method for checking the mouse
        public bool CheckMouse()
        {
            //Updates the mouse state
            mouse = Mouse.GetState();

            //If the mouse button is released
            if (mouse.LeftButton == ButtonState.Released)
            {
                //Set it to not being held anymore
                mouseButtonHeld = false;
            }

            //If the left mouse button is pressed and wasn't before
            if (mouse.LeftButton == ButtonState.Pressed && !mouseButtonHeld)
            {
                //Sets the mouse button to held
                mouseButtonHeld = true;

                //Returns that the left button being pressed now is valid
                return true;
            }

            //Returns that the left button was pressed before and is invalid now
            return false;
        }


        //Pre:
        //Post:
        //Desc:
        public GameState UpdateCreatorHelp(GameState gameState)
        {
            //Updates the current keyboard state
            keyboardState = Keyboard.GetState();

            //Updates the keys which are pressed
            pressedKeys = keyboardState.GetPressedKeys();

            //Loop for every key that was pressed
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                //If the current key is the H key
                if (pressedKeys[i] == Keys.Escape)
                {
                    //Sets the new game state
                    gameState = GameState.MapCreator;
                }
            }

            //Set the previously pressed keys to the currently pressed keys
            prevPressedKeys = pressedKeys;

            //Returns the current game state
            return gameState;
        }


        //Pre: The list of keys that need to be checked, the array of gametiles, the current gamestate, and the camera
        //Post: The new gamestate is returned
        //Desc: A method for updating the creator controls
        public GameState UpdateCreatorControls(List<Keys> keysToCheck, Tile[,] gameTiles, GameState gameState, Cam2D camera)
        {
            //Updates the current keyboard state
            keyboardState = Keyboard.GetState();

            //Updates the mouse state
            mouse = Mouse.GetState();

            //If the mouse button is released
            if (mouse.LeftButton == ButtonState.Released)
            {
                //Set it to not being held anymore
                mouseButtonHeld = false;
            }

            //Updates the keys which are pressed
            pressedKeys = keyboardState.GetPressedKeys();

            //Loop for every key that was pressed
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                //If the current key is the up key
                if (pressedKeys[i] == Keys.Up || pressedKeys[i] == Keys.W)
                {
                    //Move the camera up
                    mapCreatorPos.Y -= (Human.HUMAN_SPEED * 5);
                }
                //If the current key is the down key
                else if (pressedKeys[i] == Keys.Down || pressedKeys[i] == Keys.S)
                {
                    //Move the camera down
                    mapCreatorPos.Y += (Human.HUMAN_SPEED * 5);
                }
                //If the current key is the left key
                else if (pressedKeys[i] == Keys.Left || pressedKeys[i] == Keys.A)
                {
                    //Move the camera to the left
                    mapCreatorPos.X -= (Human.HUMAN_SPEED * 5);
                }
                //If the current key is the right key
                else if (pressedKeys[i] == Keys.Right || pressedKeys[i] == Keys.D)
                {
                    //Move the camera to the right
                    mapCreatorPos.X += (Human.HUMAN_SPEED * 5);
                }

                //If the X position is less then 0
                if (mapCreatorPos.X < 0)
                {
                    //Set the position equal to 0
                    mapCreatorPos.X = 0;
                }

                //If the X position is greater then the screen size
                if (mapCreatorPos.X > ((Rectangle)camera.GetLimits()).Width)
                {
                    //Sets the position to the screen size
                    mapCreatorPos.X = ((Rectangle)camera.GetLimits()).Width;
                }

                //If the Y position is less then 0
                if (mapCreatorPos.Y < 0)
                {
                    //Set the position equal to 0
                    mapCreatorPos.Y = 0;
                }

                //If the Y position is greater then the screen size
                if (mapCreatorPos.Y > ((Rectangle)camera.GetLimits()).Height)
                {
                    //Sets the position to the screen size
                    mapCreatorPos.Y = ((Rectangle)camera.GetLimits()).Height;
                }

                //Looks at the position
                camera.LookAt(mapCreatorPos);

                //If the key is something that can be used 
                if (CheckOldKeys(i, keysToCheck))
                {
                    //If the current key is the one key
                    if (pressedKeys[i] == Keys.NumPad1 || pressedKeys[i] == Keys.D1)
                    {
                        //Sets the tile that is currently used
                        CurrentTileType = TileType.Blank;
                        PlacingEnemy = false;
                    }
                    //If the current key is the two key
                    else if (pressedKeys[i] == Keys.NumPad2 || pressedKeys[i] == Keys.D2)
                    {
                        //Sets the tile that is currently used
                        CurrentTileType = TileType.Save;
                        PlacingEnemy = false;
                    }
                    //If the current key is the three key
                    else if (pressedKeys[i] == Keys.NumPad3 || pressedKeys[i] == Keys.D3)
                    {
                        //Sets the tile that is currently used
                        CurrentTileType = TileType.Spawn;
                        PlacingEnemy = false;
                    }
                    //If the current key is the four key
                    else if (pressedKeys[i] == Keys.NumPad4 || pressedKeys[i] == Keys.D4)
                    {
                        //Sets the tile that is currently used
                        CurrentTileType = TileType.Wall;
                        PlacingEnemy = false;
                    }
                    //If the current key is the 5 key
                    else if (pressedKeys[i] == Keys.NumPad5 || pressedKeys[i] == Keys.D5)
                    {
                        //Sets the enemy to be placing
                        PlacingEnemy = true;
                    }
                    //If the current key is the escape key
                    else if (pressedKeys[i] == Keys.Escape)
                    {
                        //Sets the game state
                        gameState = GameState.SaveCreatorMap;
                    }
                    //If the current key is the H key
                    else if (pressedKeys[i] == Keys.H)
                    {
                        //Sets the game state
                        gameState = GameState.MapCreatorHelp;
                    }
                }
            }

            //Set the previously pressed keys to the currently pressed keys
            prevPressedKeys = pressedKeys;

            //Returns the current game state
            return gameState;
        }


        //Pre: The list of keys to check, the player object, the current gamestate, and the array of gametiles
        //Post: The current gamestate is returned
        //Desc: A method for updating the game controls
        public GameState UpdateGameControls(List<Keys> keysToCheck, Player player, GameState gameState, Tile[,] gameTiles)
        {
            //Updates the current keyboard state
            keyboardState = Keyboard.GetState();

            //Updates the mouse state
            mouse = Mouse.GetState();

            //If the mouse button is released
            if (mouse.LeftButton == ButtonState.Released)
            {
                //Set it to not being held anymore
                mouseButtonHeld = false;
            }

            //Updates the keys which are pressed
            pressedKeys = keyboardState.GetPressedKeys();

            //If no keys were pressed
            if (pressedKeys.Length == 0)
            {
                //If the player is currently moving
                if (player.HumanMoving)
                {
                    //Sets the human/player to idle, meaning it's not moving
                    player.SetHumanIdle();
                }
            }

            //If the left mouse button is pressed
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                //Sets the player to need shooting
                player.NeedShooting = true;
            }

            //If the right mouse button is pressed and the player isn't currently reloading
            if (mouse.RightButton == ButtonState.Pressed && player.GetCurrentAnimState() != AnimationState.Reload)
            {
                //Sets the player to need reloading
                player.NeedReload = true;
            }

            //Loop for every key that was pressed
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                //If the current key is the up key
                if (pressedKeys[i] == Keys.Up || pressedKeys[i] == Keys.W)
                {
                    //Move the player up
                    player.MovePlayer(MathHelper.ToRadians(270), gameTiles);
                }
                //If the current key is the down key
                else if (pressedKeys[i] == Keys.Down || pressedKeys[i] == Keys.S)
                {
                    //Move the player down
                    player.MovePlayer(MathHelper.ToRadians(90), gameTiles);
                }
                //If the current key is the left key
                else if (pressedKeys[i] == Keys.Left || pressedKeys[i] == Keys.A)
                {
                    //Move the player to the left
                    player.MovePlayer(MathHelper.ToRadians(180), gameTiles);
                }
                //If the current key is the right key
                else if (pressedKeys[i] == Keys.Right || pressedKeys[i] == Keys.D)
                {
                    //Move the player to the right
                    player.MovePlayer(MathHelper.ToRadians(0), gameTiles);
                }
                else
                {
                    //If the player is currently moving
                    if (player.HumanMoving)
                    {
                        //Sets the human/player to idle, meaning it's not moving
                        player.SetHumanIdle();
                    }
                }

                //If the key is something that can be used 
                if (CheckOldKeys(i, keysToCheck))
                {
                    //If the current key is the one key
                    if (pressedKeys[i] == Keys.NumPad1 || pressedKeys[i] == Keys.D1)
                    {
                        //Sets the player's weapon if it's not the one currently equipped
                        player.ChangeWeapon(WeaponTypes.Handgun);
                    }
                    //If the current key is the two key
                    else if (pressedKeys[i] == Keys.NumPad2 || pressedKeys[i] == Keys.D2)
                    {
                        //Sets the player's weapon if it's not the one currently equipped
                        player.ChangeWeapon(WeaponTypes.Rifle);
                    }
                    //If the current key is the three key
                    else if (pressedKeys[i] == Keys.NumPad3 || pressedKeys[i] == Keys.D3)
                    {
                        //Sets the player's weapon if it's not the one currently equipped
                        player.ChangeWeapon(WeaponTypes.Shotgun);
                    }
                    //If the current key is the four key
                    else if (pressedKeys[i] == Keys.NumPad4 || pressedKeys[i] == Keys.D4)
                    {
                        //Sets the player's weapon if it's not the one currently equipped
                        player.ChangeWeapon(WeaponTypes.ThrowingKnife);
                    }
                    //If the current key is the escape key
                    else if (pressedKeys[i] == Keys.Escape)
                    {
                        gameState = GameState.Pause;
                    }
                }
            }

            //Set the previously pressed keys to the currently pressed keys
            prevPressedKeys = pressedKeys;

            //Returns the current game state
            return gameState;
        }


        //Pre: The rectangle of the current button
        //Post: A boolean for whether the current button was pressed
        //Desc: A method for checking if a button was pressed
        private bool CheckButtonPressed(Rectangle currentButton)
        {
            //Boolean for whether the button was pressed
            bool buttonPressed = false;

            //Checks if the mouse is over the current button and the left mouse button is not held
            if (currentButton.Y <= mouse.Y && currentButton.Y + currentButton.Height >= mouse.Y && currentButton.X <= mouse.X && currentButton.X + currentButton.Width >= mouse.X && !mouseButtonHeld)
            {
                //If the left mouse button is pressed, the left mouse button is set to held and the button is set to pressed
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    mouseButtonHeld = true;
                    buttonPressed = true;
                }
            }

            //Returns whether teh button is pressed or not
            return buttonPressed;
        }


        //Pre: The rectangle for the current button
        //Post: A boolean for whether the button is being hovered over
        //Desc: A method for checking whether a button is being hovered over
        private bool CheckButtonHovering(Rectangle currentButton)
        {
            //Boolean for whether the button is being hovered over
            bool buttonHovering = false;

            //If the the mouse is over the current button, the button is set to be hoevring over
            if (currentButton.Y <= mouse.Y && currentButton.Y + currentButton.Height >= mouse.Y && currentButton.X <= mouse.X && currentButton.X + currentButton.Width >= mouse.X)
            {
                buttonHovering = true;
            }

            //Returns whether the button is being hovered over
            return buttonHovering;
        }


        //Pre: The index of the current key to check and the list of keys that need to be checked
        //Post: A boolean for whether the current key is valid
        //Desc: A method for checking whether a key is valid
        private bool CheckOldKeys(int keyIndex, List<Keys> keysToCheck)
        {
            //Boolean for whether the current key is a new key which is a valid key and an integer for the count
            bool newKey = true;
            int count = 0;

            //Foreach key in the keys to check list
            foreach (Keys key in keysToCheck)
            {
                //If the pressed key at the current key index is not the same as the key from the foreach loop, the count is added to
                if (pressedKeys[keyIndex] != key)
                {
                    count++;
                }
            }

            //If the count is eqaul to the amount of keys to check, the new key is set to false
            if (count == keysToCheck.Count)
            {
                newKey = false;
            }

            //Loop for every previously pressed key
            for (int i = 0; i < prevPressedKeys.Length; i++)
            {
                //If the current previously pressed key is the same key as the current pressed keys at the key index
                if (prevPressedKeys[i] == pressedKeys[keyIndex])
                {
                    //Sets the new key to false and stops checking the previously pressed keys
                    newKey = false;
                    i = prevPressedKeys.Length;
                }
            }

            //Returns the new key
            return newKey;
        }
    }
}
