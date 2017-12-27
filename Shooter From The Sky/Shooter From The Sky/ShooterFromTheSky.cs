/*
 * Author: Rony Verch
 * File Name: ShooterFromTheSky.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 20, 2015
 * Modified Date: January 20, 2015
 * Description: The driver class for the entire game. The game is a top-down shooter that has profiles, highscores, maps, and stats all saved online to be able to be accessed from
 *              anywhere using this client. This driver class runs the entire game and makes all the calls that need to be made.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using Camera2D_XNA4;

namespace Shooter_From_The_Sky
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ShooterFromTheSky : Microsoft.Xna.Framework.Game
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constants for the screen width and height
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 768;

        //Constants for the textbox size
        const int TEXT_BOX_WIDTH = 350;
        const int TEXT_BOX_HEIGHT = 60;

        //Constants for the displacement between a label and a textbox
        const int LABEL_TEXT_BOX_X = 20;

        //Constants for the width and height of the player/enemies
        const int HUMAN_WIDTH = 64;
        const int HUMAN_HEIGHT = 64;

        //Constants for the min and max tile size of a map
        const int MIN_MAP_SIZE = 20;
        const int MAX_MAP_SIZE = 200;

        //Constants for the outline of the button sizes
        const int BUTTON_OUTLINE_WIDTH = 4;
        const int BUTTON_OUTLINE_HEIGHT = 5;

        //Constants for the displacement and location between the map choosing buttons
        const int CHOOSING_BUTTON_Y_DISPLACEMENT = 20;
        const int CHOOSING_BUTTON_X = 337;

        //Constant for the camera displacement for the map scrolling buttons
        const int CAMERA_SCROLLING_DISPLACEMENT = 100;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Graphics device manager and sprite batch
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Gametiles array and the full size of the screen (not only what's visible)
        Tile[,] gameTiles;
        Rectangle fullScreenSize;

        //Lists for the enemies and tiles that are currently in view
        List<Enemy> enemiesInView = new List<Enemy>();
        List<Tile> tilesInView = new List<Tile>();

        //The player, the enemies, and the projectiles
        Player player;
        List<Enemy> enemies = new List<Enemy>();
        List<Projectile> projectiles = new List<Projectile>();

        //The start tile for the player
        Tile startTile;

        //The current game state
        GameState gameState = GameState.GameLogin;

        //The keys that are used in game
        List<Keys> gameKeyboardKeys = new List<Keys>();
        List<Keys> keyboardTypingKeys = new List<Keys>();
        List<Keys> keyboardNumKeys = new List<Keys>();
        List<Keys> scrollButtonKeys = new List<Keys>();

        //The keyboard mouse input 
        KeyboardMouseInput keyboardMouseInput = new KeyboardMouseInput();

        //The quadtree for the collisions and the objects it returns
        QuadTree quadTree = new QuadTree(0, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT));
        List<Projectile> returnObjects = new List<Projectile>();

        //The camera for scrolling around the map
        Cam2D camera;

        //The different helpers for saving information onto the server
        OnlineStorageHelper onlineHelper = new OnlineStorageHelper();
        ProfileHelper profileHelper;
        MapSaveLoadHelper mapSaveLoadHelper;
        ProfileStatsHelper profileStatsHelper;
        HighscoresHelper highScoresHelper;

        //The current user and it position, and the current map
        string currentUser = "";
        string currentMap = "";
        Vector2 currentUserPos = new Vector2(20, 20);

        //Booleans for whether the account is valid and if a user name is taken
        bool isAccountInvalid = false;
        bool userTaken = false;

        //Booleans for whether a map size is invalid or a map name is invalid
        bool invalidMapSize = false;
        bool mapNameInvalid = false;

        //Boolean for whether choosing a map in unavailable
        bool unableChooseMap = false;

        //Boolean for whether a save for the map is available and whether saving the map didn't work
        bool saveMapAvailable = false;
        bool unableSaveMap = false;

        //Boolean for whether the user is choosing high scores
        bool choosingHighscores = false;

        //Fonts for the game
        SpriteFont buttonFont;
        SpriteFont enlargedButtonFont;
        SpriteFont titleFont;

        //The texture and rectangle for the backround
        Texture2D backgroundTexture;
        Rectangle backgroundRect = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

        //Labels for the differnet menus in the game
        List<Label> mapCreatorLabels = new List<Label>();
        List<Label> mapCreatorStartLabels = new List<Label>();
        List<Label> loginLabels = new List<Label>();
        List<Label> saveMapCreatorLabels = new List<Label>();
        List<Label> mainMenuLabels = new List<Label>();
        List<Label> loadSaveLabels = new List<Label>();
        List<Label> pauseMenuLabels = new List<Label>();
        List<Label> highscoreLabels = new List<Label>();
        List<Label> gameOverLabels = new List<Label>();
        List<Label> profileStatsLabels = new List<Label>();
        List<Label> instructionsLabels = new List<Label>();
        List<Label> chooseMapLabels = new List<Label>();

        //Textbox's for the different menus in the game
        List<TextBox> loginTextboxs = new List<TextBox>();
        List<TextBox> startMapCreatorTextboxs = new List<TextBox>();
        List<TextBox> saveMapCreatorTextboxs = new List<TextBox>();

        //Buttons for the different menus in the game
        List<Button> loginButtons = new List<Button>();
        List<Button> mainMenuButtons = new List<Button>();
        List<Button> startMapCreatorButtons = new List<Button>();
        List<Button> saveMapCreatorButtons = new List<Button>();
        List<Button> chooseMapButtons = new List<Button>();
        List<Button> pauseMenuButtons = new List<Button>();
        List<Button> pauseMenuSaveButton = new List<Button>();
        List<Button> loadSaveButtons = new List<Button>();
        List<Button> highScoreButtons = new List<Button>();
        List<Button> gameOverButtons = new List<Button>();
        List<Button> profileStatsButtons = new List<Button>();
        List<Button> instructionButtons = new List<Button>();

        //The new row and column amount for the game
        int newRowAmount = 0;
        int newColumnAmount = 0;

        //The amount the buttons are currently scrolling
        int currentButtonMovement = 0;

        //The amount of time the game has already been going through and the stop watch to track the rest of the time
        float previouslyElapsedTime = 0f;
        Stopwatch elapsedTime = new Stopwatch();

        //The amount of enemies killed and bullets shot
        int enemiesKilled = 0;
        int bulletsShot = 0;

        //Vector2 for the highscore displacement between the two columns
        Vector2 highscoreDisplacement = new Vector2(512, 0);

        //Integer for the current enemy needed to be checked
        int currentEnemyCheck = 0;

        //Variables for the sound in the game
        Song backgroundMusic;


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: The ShooterFromTheSky driver class is created
        //Desc: A constructor for the class
        public ShooterFromTheSky()
        {
            //Sets the graphics device manager and the root directory for the content
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Sets the mouse visible and the width and height of the screen
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;

            //Sets the multiple sampling to be preferred
            graphics.PreferMultiSampling = true;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Initializes the online helper so access to the server is possible
            onlineHelper.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Loads the music and plays it with repeat on
            backgroundMusic = this.Content.Load<Song>("Sounds\\Background Music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;

            //Sets the texture data for all the tiles (static method)
            Tile.SetTextureData(Content);

            //Sets the texture data for all the weapons
            EnemyRifle.SetTextureData(Content);
            Handgun.SetTextureData(Content);
            PlayerRifle.SetTextureData(Content);
            Shotgun.SetTextureData(Content);
            ThrowingKnife.SetTextureData(Content);

            //Sets the texture data for the projectiles
            Projectile.SetTextureData(Content);

            //Creates all the helpers for the server saving
            profileStatsHelper = new ProfileStatsHelper(onlineHelper);
            profileHelper = new ProfileHelper(onlineHelper, profileStatsHelper);
            highScoresHelper = new HighscoresHelper(onlineHelper);
            mapSaveLoadHelper = new MapSaveLoadHelper(onlineHelper, highScoresHelper, HUMAN_WIDTH, HUMAN_HEIGHT);

            //Sets the fonts for the game
            buttonFont = this.Content.Load<SpriteFont>("Fonts\\ButtonFont");
            enlargedButtonFont = this.Content.Load<SpriteFont>("Fonts\\EnlargedButtonFont");
            titleFont = this.Content.Load<SpriteFont>("Fonts\\TitleFont");

            //Sets the background texture data
            backgroundTexture = new Texture2D(GraphicsDevice, 1, 1);
            backgroundTexture.SetData(new Color[] { Color.White });

            //Creates the textbox's, labels, and buttons
            CreateTextBoxs();
            CreateLabels();
            CreateButtons();

            //Sets the keys
            SetKeys();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Switch statement for the current game state
            switch (gameState)
            {

                /////////////////////////////////////////////////// GAME LOGIN \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.GameLogin:

                    //Updadtes the text box's
                    keyboardMouseInput.UpdateTextBoxes(loginTextboxs);

                    //Loop for every login text box
                    for (int i = 0; i < loginTextboxs.Count; i++)
                    {
                        //If the text box is clicked
                        if (loginTextboxs[i].TextBoxClicked)
                        {
                            //If the text in the text box match's
                            if (loginTextboxs[i].Text == "USER HERE" || loginTextboxs[i].Text == "PASS HERE")
                            {
                                //The textbox is reset
                                loginTextboxs[i].Text = "";
                            }

                            //The text being typed is checked and is added to the current text box
                            keyboardMouseInput.UpdateKeyboardTyping(keyboardTypingKeys, loginTextboxs[i].Text, false);
                            loginTextboxs[i].Text = keyboardMouseInput.userText;
                        }
                    }

                    //Updates the buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateButtons(loginButtons, gameState);

                    //If the game state is the main menu
                    if (gameState == GameState.MainMenu)
                    {
                        //Set the user taken to false
                        userTaken = false;

                        //Check for the current profile and if it exists set the current user
                        if (profileHelper.CheckForProfile(loginTextboxs[0].Text, loginTextboxs[1].Text))
                        {
                            currentUser = loginTextboxs[0].Text;
                        }
                        //If the profile is invalid tell the user to try again
                        else
                        {
                            gameState = GameState.GameLogin;
                            isAccountInvalid = true;
                        }
                    }
                    //If the game state is account creator
                    else if (gameState == GameState.AccountCreator)
                    {
                        //If the text in the textbox's is valid
                        if (loginTextboxs[0].Text != "" && loginTextboxs[1].Text != "" && loginTextboxs[0].Text != "USER HERE" && loginTextboxs[1].Text != "PASS HERE")
                        {
                            //If the profile is added, the current user is to to the added profile's user name
                            if (profileHelper.AddProfile(loginTextboxs[0].Text, loginTextboxs[1].Text))
                            {
                                currentUser = loginTextboxs[0].Text;
                                gameState = GameState.MainMenu;
                            }
                            //If the profile is not added, the user is told to try again
                            else
                            {
                                userTaken = true;
                                gameState = GameState.GameLogin;
                            }
                        }
                        //If the text in teh text box is not valid the user is told to try again
                        else
                        {
                            isAccountInvalid = true;
                            gameState = GameState.GameLogin;
                        }
                    }

                    break;

                /////////////////////////////////////////////////// START MAP CREATOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.StartMapCreator:

                    //Updates the text box's
                    keyboardMouseInput.UpdateTextBoxes(startMapCreatorTextboxs);

                    //Loop for each text box
                    for (int i = 0; i < startMapCreatorTextboxs.Count; i++)
                    {
                        //If the current text box is clicked
                        if (startMapCreatorTextboxs[i].TextBoxClicked)
                        {
                            //If the text in the text box is the default text, the text is set to blank
                            if (startMapCreatorTextboxs[i].Text == "ROW NUM" || startMapCreatorTextboxs[i].Text == "COLUMN NUM")
                            {
                                startMapCreatorTextboxs[i].Text = "";
                            }

                            //What the user is typing is checked and added to the current text box
                            keyboardMouseInput.UpdateKeyboardTyping(keyboardNumKeys, startMapCreatorTextboxs[i].Text, true);
                            startMapCreatorTextboxs[i].Text = keyboardMouseInput.userText;
                        }
                    }

                    //Updates the buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateButtons(startMapCreatorButtons, gameState);

                    //If the game state is the map creator
                    if (gameState == GameState.MapCreator)
                    {
                        //If the text is valid the map is created
                        if (startMapCreatorTextboxs[0].Text != "ROW NUM" && startMapCreatorTextboxs[1].Text != "COLUMN NUM")
                        {
                            if (Convert.ToInt32(startMapCreatorTextboxs[0].Text) <= MAX_MAP_SIZE && Convert.ToInt32(startMapCreatorTextboxs[1].Text) <= MAX_MAP_SIZE
                                && Convert.ToInt32(startMapCreatorTextboxs[0].Text) >= MIN_MAP_SIZE && Convert.ToInt32(startMapCreatorTextboxs[1].Text) >= MIN_MAP_SIZE)
                            {
                                newRowAmount = Convert.ToInt32(startMapCreatorTextboxs[0].Text);
                                newColumnAmount = Convert.ToInt32(startMapCreatorTextboxs[1].Text);

                                CreateMap();
                            }
                            //If the text is not valid the user is told to try again
                            else
                            {
                                startMapCreatorTextboxs[0].Text = "";
                                startMapCreatorTextboxs[1].Text = "";

                                gameState = GameState.StartMapCreator;

                                invalidMapSize = true;
                            }
                        }
                    }

                    break;

                /////////////////////////////////////////////////// MAIN MENU \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.MainMenu:

                    //Updates the buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateButtons(mainMenuButtons, gameState);

                    //If the game state is on choosing the map, choosing high scores is set to false
                    if (gameState == GameState.ChooseMap)
                    {
                        choosingHighscores = false;
                    }

                    //If choosing high scores is true, the game state is set to choosing high scores
                    if (choosingHighscores)
                    {
                        gameState = GameState.ChooseMap;
                    }

                    //Switch statement for the game state
                    switch (gameState)
                    {
                        //If the game state is instructions
                        case GameState.Instructions:

                            //Set the instructions go back button to go to the main menu
                            instructionButtons[0] = new Button(new Rectangle(370, 670, 284, 60), new Rectangle(362, 660, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                                "Go Back", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MainMenu);
                            instructionButtons[0].SetTextureData(graphics);

                            break;

                        //If the game state is the start map creator
                        case GameState.StartMapCreator:

                            //The start map creator textbox's are reset
                            startMapCreatorTextboxs[0].Text = "ROW NUM";
                            startMapCreatorTextboxs[1].Text = "COLUMN NUM";

                            startMapCreatorTextboxs[0].TextBoxClicked = false;
                            startMapCreatorTextboxs[1].TextBoxClicked = false;

                            break;

                        //If the game state is on choosing a map
                        case GameState.ChooseMap:

                            //The file with all the maps is downloaded
                            if (mapSaveLoadHelper.ReadAllMaps())
                            {
                                //List of all the maps
                                List<string> allMaps = mapSaveLoadHelper.GetAllMaps();

                                //The buttons are cleared
                                chooseMapButtons.Clear();

                                //The border width and height is calculated
                                int borderWidth = TEXT_BOX_WIDTH - (BUTTON_OUTLINE_WIDTH * 2);
                                int borderHeight = TEXT_BOX_HEIGHT - (BUTTON_OUTLINE_HEIGHT * 2);

                                //The go back button is added
                                chooseMapButtons.Add(new Button(new Rectangle(CHOOSING_BUTTON_X + BUTTON_OUTLINE_WIDTH, SCREEN_HEIGHT - CHOOSING_BUTTON_Y_DISPLACEMENT - borderHeight, borderWidth,
                                    borderHeight), new Rectangle(CHOOSING_BUTTON_X, SCREEN_HEIGHT - CHOOSING_BUTTON_Y_DISPLACEMENT - borderHeight - BUTTON_OUTLINE_HEIGHT, TEXT_BOX_WIDTH,
                                    TEXT_BOX_HEIGHT), new Vector2(0, 0), Color.Black, Color.Red, "Go Back", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MainMenu));
                                chooseMapButtons[0].SetTextureData(graphics);

                                //An integer for the current button Y
                                int buttonY = 49;

                                //Loop for all the maps
                                for (int i = 0; i < allMaps.Count; i++)
                                {
                                    //The button is added for the current map
                                    chooseMapButtons.Add(new Button(new Rectangle(CHOOSING_BUTTON_X + BUTTON_OUTLINE_WIDTH, buttonY + BUTTON_OUTLINE_HEIGHT, borderWidth, borderHeight), new Rectangle(CHOOSING_BUTTON_X,
                                        buttonY, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT), new Vector2(0, 0), Color.Black, Color.Red, allMaps[i], buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.Game));
                                    chooseMapButtons[i + 1].SetTextureData(graphics);

                                    //The button Y is added to
                                    buttonY += TEXT_BOX_HEIGHT + CHOOSING_BUTTON_Y_DISPLACEMENT;
                                }

                                //The viewport for the camera scrolling
                                Viewport viewport = new Viewport(new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height - CAMERA_SCROLLING_DISPLACEMENT));

                                //The camera instance is created
                                camera = new Cam2D(viewport, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, chooseMapButtons[chooseMapButtons.Count - 1].GetCurrentRectangle().Y
                                    + TEXT_BOX_HEIGHT + CHOOSING_BUTTON_Y_DISPLACEMENT), 1, 1, 0, new Rectangle(0, 0, 0, 0));
                            }
                            //If the file is not downloaded, the user is told choosing map was unable to be loaded
                            else
                            {
                                gameState = GameState.MainMenu;
                                unableChooseMap = true;
                            }

                            break;

                        //If the game state is on exit game
                        case GameState.ExitGame:

                            //Exits the game
                            this.Exit();

                            break;

                        //If the game state is on signing out
                        case GameState.SignOut:

                            //Sets the game state to game login and resets all the login information
                            gameState = GameState.GameLogin;
                            loginTextboxs[0].Text = "USER HERE";
                            loginTextboxs[1].Text = "PASS HERE";
                            currentUser = "";

                            //Resets all the textbox's being clicked
                            for (int i = 0; i < loginTextboxs.Count; i++)
                            {
                                loginTextboxs[i].TextBoxClicked = false;
                            }

                            break;

                        //If the game state is on high scores
                        case GameState.Highscores:

                            //Choosing high scores is set to true and the game state is set to main menu
                            choosingHighscores = true;
                            gameState = GameState.MainMenu;

                            break;

                        //If the game state is on profile stats
                        case GameState.ProfileStats:

                            //If the profile stats for the current profile were read
                            if (profileStatsHelper.ReadProfileStats(currentUser))
                            {
                                //The labels for the profile stats are cleared
                                profileStatsLabels.Clear();

                                //The time that is displayed is calculated
                                float totalMilliseconds = (float)profileStatsHelper.TotalGameTime;
                                float seconds = (float)Math.Floor(totalMilliseconds / 1000);
                                float minutes = (float)Math.Floor(seconds / 60);
                                seconds = (float)Math.Floor(seconds % 60);
                                float milliseconds = (float)Math.Round(totalMilliseconds % 1000, 1);

                                //All the labels are created for the profile stats
                                profileStatsLabels.Add(new Label(currentUser + "'s Profile Stats", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), enlargedButtonFont, Color.White));
                                profileStatsLabels.Add(new Label("Enemies Killed: " + profileStatsHelper.EnemiesKilled, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 150), buttonFont, Color.White));
                                profileStatsLabels.Add(new Label("Health Lost: " + profileStatsHelper.HealthLost, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 200), buttonFont, Color.White));
                                profileStatsLabels.Add(new Label("Death Amount: " + profileStatsHelper.DeathAmount, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 250), buttonFont, Color.White));
                                profileStatsLabels.Add(new Label("Total Game Time: " + minutes + " minutes, " + seconds + " seconds, " + milliseconds + " milliseconds", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 300), buttonFont, Color.White));
                                profileStatsLabels.Add(new Label("Total Projectiles Shot: " + profileStatsHelper.BulletsShot, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 350), buttonFont, Color.White));

                                //The first label is aligned
                                profileStatsLabels[0].AlignTop();
                                profileStatsLabels[0].CenterTextX();

                                //Loop for every other label that aligns the labels to the left
                                for (int i = 1; i < profileStatsLabels.Count; i++)
                                {
                                    profileStatsLabels[i].AlignLeft();
                                }
                            }
                            //If the profile stats were not read, the game state is set to main menu because displaying the stats was unavilable
                            else
                            {
                                gameState = GameState.MainMenu;
                            }

                            break;
                    }

                    break;

                /////////////////////////////////////////////////// GAME \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.Game:

                    //if the program is currently unactive, the game is paused
                    if (!IsActive)
                    {
                        gameState = GameState.Pause;
                    }

                    //Updates the game controls
                    gameState = keyboardMouseInput.UpdateGameControls(gameKeyboardKeys, player, gameState, gameTiles);

                    //Updates the player
                    player.Update();

                    //Has the camera look at the player
                    camera.LookAt(player.GetBounds());

                    //The displacement for the player
                    float displacementX = 32;
                    float displacementY = 32;

                    //Calculates the rotation of the player
                    player.CalcRotation(keyboardMouseInput.mouse.X + (camera.GetPosition().X - (SCREEN_WIDTH / 2)), keyboardMouseInput.mouse.Y +
                        (camera.GetPosition().Y - (SCREEN_HEIGHT / 2)), displacementX, displacementY);

                    //Adds to the amount of bullets shot
                    bulletsShot += player.Projectiles.Count;

                    //Adds to all the projectiles and clears the projectiles from the players
                    projectiles.AddRange(player.Projectiles);
                    player.Projectiles.Clear();

                    //The enemies and tiles in view is cleared
                    enemiesInView.Clear();
                    tilesInView.Clear();

                    //For each tile in the game tiles, if the tile is between the bounds of the camera it is added to tiles in view list
                    foreach (Tile tile in gameTiles)
                    {
                        if (camera.IntersectsScreen(tile.PositionRect))
                        {
                            tilesInView.Add(tile);
                        }
                    }

                    //For each enemy in the enemies list, if the enemy is between the bounds of the camera it is added to enemies in view list
                    foreach (Enemy enemy in enemies)
                    {
                        if (camera.IntersectsScreen(enemy.GetBounds()))
                        {
                            enemiesInView.Add(enemy);
                        }
                    }

                    //If the current enemy checked is greater then the amount of enemies, the current enemy to check is set to 0
                    if (currentEnemyCheck >= enemiesInView.Count)
                    {
                        currentEnemyCheck = 0;
                    }

                    //Loop for every enemy in view
                    for (int i = 0; i < enemiesInView.Count; i++)
                    {
                        //If the current enemy needs to be checked for pathfinding, it is updated with checking for pathfinding, otherwise it is updated without checkign pathfinding
                        if (currentEnemyCheck == i || currentEnemyCheck + 1 == i || currentEnemyCheck + 2 == i)
                        {
                            enemiesInView[i].Update(player, gameTiles, true);
                        }
                        else
                        {
                            enemiesInView[i].Update(player, gameTiles, false);
                        }

                        //Adds the projectiles of the current enemy to the global projectiles and clears the projectiles from the enemy
                        projectiles.AddRange(enemiesInView[i].Projectiles);
                        enemiesInView[i].Projectiles.Clear();

                        //If the health of the enemy is less then or equal to 0, the enemy is removed
                        if (enemiesInView[i].Health <= 0)
                        {
                            enemies.Remove(enemiesInView[i]);
                            enemiesInView[i] = null;
                            enemiesInView.RemoveAt(i);
                        }
                    }

                    //The current enemy to check pathfinding for is added to
                    currentEnemyCheck += 3;

                    //Loop for every projectile
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        //Updates the projectile
                        projectiles[i].Update(gameTiles, new Vector2(camera.GetPosition().X + (SCREEN_WIDTH / 2), camera.GetPosition().Y + (SCREEN_HEIGHT / 2)));

                        //If the projectile needs to be destroyed, it is destroyed
                        if (projectiles[i].NeedDestroy)
                        {
                            projectiles[i] = null;
                            projectiles.RemoveAt(i);
                            i--;
                        }
                    }

                    //Clears the quad tree
                    quadTree.Clear();

                    //Inserts every projectile into the quadtree through the loop
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        quadTree.Insert(projectiles[i]);
                    }

                    //Checks the bullet collision
                    CheckBulletCollision();

                    //If the game is paused, the stopwatch is set to stopped
                    if (gameState == GameState.Pause)
                    {
                        elapsedTime.Stop();
                    }

                    //If the enemy dies
                    if (player.Health <= 0)
                    {
                        //The stop watch is stopped and the gamestate is updated
                        elapsedTime.Stop();
                        gameState = GameState.GameOver;

                        //Clears the game over labels
                        gameOverLabels.Clear();

                        //Adds a label 
                        gameOverLabels.Add(new Label("You have died! Better luck next time.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), enlargedButtonFont, Color.White));

                        //Calculates the time that will be displayed
                        float totalMilliseconds = (float)elapsedTime.Elapsed.TotalMilliseconds;
                        float seconds = (float)Math.Floor(totalMilliseconds / 1000);
                        float minutes = (float)Math.Floor(seconds / 60);
                        seconds = (float)Math.Floor(seconds % 60);
                        float milliseconds = (float)Math.Round(totalMilliseconds % 1000, 1);

                        //Adds labels to be displayed
                        gameOverLabels.Add(new Label("Your time was: " + minutes + " minutes, " + seconds + " seconds, and " + milliseconds + " milliseconds", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 100), buttonFont, Color.White));
                        gameOverLabels.Add(new Label("The amount of enemies killed is " + enemiesKilled, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 150), buttonFont, Color.White));
                        gameOverLabels.Add(new Label("The amount of bullets you shot was " + bulletsShot, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 200), buttonFont, Color.White));

                        //Aligns the first label to the top
                        gameOverLabels[0].AlignTop();

                        //Aligns all the labels to the left
                        for (int i = 0; i < gameOverLabels.Count; i++)
                        {
                            gameOverLabels[i].AlignLeft();
                        }
                    }
                    //If the user won the map
                    else if (enemies.Count == 0)
                    {
                        //The stop watch is stopped and the gamestate is updated
                        elapsedTime.Stop();
                        gameState = GameState.GameOver;

                        //Clears the game over labels
                        gameOverLabels.Clear();

                        //Adds a label 
                        gameOverLabels.Add(new Label("You have finished the map, congratulations!", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), enlargedButtonFont, Color.White));

                        //Calculates the time that will be displayed
                        float totalMilliseconds = (float)elapsedTime.Elapsed.TotalMilliseconds;
                        float seconds = (float)Math.Floor(totalMilliseconds / 1000);
                        float minutes = (float)Math.Floor(seconds / 60);
                        seconds = (float)Math.Floor(seconds % 60);
                        float milliseconds = (float)Math.Round(totalMilliseconds % 1000, 1);

                        //Adds labels to be displayed
                        gameOverLabels.Add(new Label("Your time was: " + minutes + " minutes, " + seconds + " seconds, and " + milliseconds + " milliseconds", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 100), buttonFont, Color.White));
                        gameOverLabels.Add(new Label("The amount of enemies killed is " + enemiesKilled, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 150), buttonFont, Color.White));
                        gameOverLabels.Add(new Label("The amount of bullets you shot was " + bulletsShot, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 200), buttonFont, Color.White));
                        gameOverLabels.Add(new Label("The amount of health you have left is " + player.Health, SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 250), buttonFont, Color.White));

                        //Aligns the first label to the top
                        gameOverLabels[0].AlignTop();

                        //Aligns all the labels to the left
                        for (int i = 0; i < gameOverLabels.Count; i++)
                        {
                            gameOverLabels[i].AlignLeft();
                        }
                    }

                    break;

                /////////////////////////////////////////////////// PAUSE GAME \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.Pause:

                    //If the escape key is currently being pressed, unpause the game
                    if (keyboardMouseInput.CheckKey(Keys.Escape))
                    {
                        gameState = GameState.Game;
                    }

                    //If the player is currently on a save tile, the save map is set to being available, otherwise it is set ot be unavailable 
                    if (gameTiles[(int)Math.Round(player.Position.X / Tile.TILE_X_SIZE, 0), (int)Math.Round(player.Position.Y / Tile.TILE_Y_SIZE, 0)].CurrentTileType == TileType.Save)
                    {
                        saveMapAvailable = true;
                    }
                    else
                    {
                        saveMapAvailable = false;
                    }

                    //Updates the buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateButtons(pauseMenuButtons, gameState);

                    //If the save map is available
                    if (saveMapAvailable)
                    {
                        //Updates the buttons and sets the game state
                        gameState = keyboardMouseInput.UpdateButtons(pauseMenuSaveButton, gameState);
                    }

                    //If the gamestate is the game, the elapsed time stop watch is started
                    if (gameState == GameState.Game)
                    {
                        elapsedTime.Start();
                    }
                    //If the game state is instructions
                    else if (gameState == GameState.Instructions)
                    {
                        //Sets the instructions go back button to go to the pause menu
                        instructionButtons[0] = new Button(new Rectangle(370, 670, 284, 60), new Rectangle(362, 660, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                            "Go Back", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.Pause);
                        instructionButtons[0].SetTextureData(graphics);
                    }
                    //If the game state is saving the game
                    else if (gameState == GameState.SaveGame)
                    {
                        //If the map is saved, the game state is set to the main menu, otherwise the user is told the map was unable to be saved
                        if (mapSaveLoadHelper.SaveCurrentMap(currentMap, currentUser, gameTiles, enemies, player, projectiles, (float)previouslyElapsedTime + (float)elapsedTime.Elapsed.TotalMilliseconds, enemiesKilled))
                        {
                            gameState = GameState.MainMenu;
                        }
                        else
                        {
                            gameState = GameState.Pause;
                            unableSaveMap = true;
                        }
                    }

                    break;

                /////////////////////////////////////////////////// MAP CREATOR HELP \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.MapCreatorHelp:

                    //if the program is currently active
                    if (IsActive)
                    {
                        //Sets the game state using the returned value form the key checking class
                        gameState = keyboardMouseInput.UpdateCreatorHelp(gameState);
                    }

                    break;

                /////////////////////////////////////////////////// MAP CREATOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.MapCreator:

                    //if the program is currently active
                    if (IsActive)
                    {
                        //The controls for the creator are updated
                        gameState = keyboardMouseInput.UpdateCreatorControls(gameKeyboardKeys, gameTiles, gameState, camera);
                    }

                    //The enemies and tiles in view are cleared
                    enemiesInView.Clear();
                    tilesInView.Clear();

                    //For each tile in the game tiles, if the tile is between the bounds of the camera it is added to tiles in view list
                    foreach (Tile tile in gameTiles)
                    {
                        if (camera.IntersectsScreen(tile.PositionRect))
                        {
                            tilesInView.Add(tile);
                        }
                    }

                    //For each enemy in the enemies list, if the enemy is between the bounds of the camera it is added to enemies in view list
                    foreach (Enemy enemy in enemies)
                    {
                        if (camera.IntersectsScreen(enemy.GetBounds()))
                        {
                            enemiesInView.Add(enemy);
                        }
                    }

                    //if the program is currently active
                    if (IsActive)
                    {
                        //If the left mouse button is pressed
                        if (keyboardMouseInput.mouse.LeftButton == ButtonState.Pressed)
                        {
                            //The row and column of the mouse are calculated
                            int row = (int)((keyboardMouseInput.mouse.X + (camera.GetPosition().X - (SCREEN_WIDTH / 2))) / Tile.TILE_X_SIZE);
                            int column = (int)((keyboardMouseInput.mouse.Y + (camera.GetPosition().Y - (SCREEN_HEIGHT / 2))) / Tile.TILE_Y_SIZE);

                            //If the row and column are valid
                            if (row >= 0 && column >= 0 && row <= gameTiles.GetLength(0) && column <= gameTiles.GetLength(1))
                            {
                                //If an enemy is not being placed
                                if (!keyboardMouseInput.PlacingEnemy)
                                {
                                    //If the spawn tile is currently being pressd
                                    if (keyboardMouseInput.CurrentTileType == TileType.Spawn)
                                    {
                                        //The mouse is checked
                                        if (keyboardMouseInput.CheckMouse())
                                        {
                                            //If the current start tile is not null, the tile is set to blank instead of being a start tile
                                            if (startTile != null)
                                            {
                                                gameTiles[startTile.Row, startTile.Column].CurrentTileType = TileType.Blank;
                                            }

                                            //The start tile is changed
                                            startTile = gameTiles[row, column];
                                            gameTiles[row, column].CurrentTileType = TileType.Spawn;
                                        }
                                    }
                                    //If any other tile is currently being placed
                                    else
                                    {
                                        //If the current tile is a spawn tile
                                        if (gameTiles[row, column].CurrentTileType == TileType.Spawn)
                                        {
                                            //The start tile is set to null and the current tile is changed
                                            startTile = null;
                                            gameTiles[row, column].CurrentTileType = keyboardMouseInput.CurrentTileType;
                                        }
                                        //If the current tile is not a spawn tile
                                        else
                                        {
                                            //The current tile type is changed
                                            gameTiles[row, column].CurrentTileType = keyboardMouseInput.CurrentTileType;
                                        }
                                    }

                                    //If the enemy exists on a current tile
                                    if (gameTiles[row, column].EnemyExists)
                                    {
                                        //Sets the enemy to not existing or being placed on the tile
                                        gameTiles[row, column].EnemyExists = false;
                                        enemies.Remove(gameTiles[row, column].EnemyPlaced);
                                        gameTiles[row, column].EnemyPlaced = null;
                                    }
                                }
                                //If the enemy is being placed
                                else
                                {
                                    //If an enemy does not exist on the current tile
                                    if (!gameTiles[row, column].EnemyExists)
                                    {
                                        //If the current tile type is a spawn tile, the start tile is set to null
                                        if (gameTiles[row, column].CurrentTileType == TileType.Spawn)
                                        {
                                            startTile = null;
                                        }

                                        //The current tile is reset to have an enemy on it and an enemy is added
                                        gameTiles[row, column].CurrentTileType = TileType.Blank;
                                        enemies.Add(new Enemy(gameTiles, (row * Tile.TILE_X_SIZE), (column * Tile.TILE_Y_SIZE), HUMAN_WIDTH, HUMAN_HEIGHT,
                                            fullScreenSize.Width, fullScreenSize.Height));
                                        gameTiles[row, column].EnemyExists = true;
                                        gameTiles[row, column].EnemyPlaced = enemies[enemies.Count - 1];
                                    }
                                }
                            }
                        }
                    }

                    //If the game state is on save creator map, the textbox is reset for it
                    if (gameState == GameState.SaveCreatorMap)
                    {
                        saveMapCreatorTextboxs[0].Text = "NAME HERE";
                        saveMapCreatorTextboxs[0].TextBoxClicked = false;
                    }

                    break;

                /////////////////////////////////////////////////// SAVE CREATOR MAP \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.SaveCreatorMap:

                    //Updates the text box's
                    keyboardMouseInput.UpdateTextBoxes(saveMapCreatorTextboxs);

                    //If the current text box is clicked
                    if (saveMapCreatorTextboxs[0].TextBoxClicked)
                    {
                        //If the text is set to the original text, set the text to blank
                        if (saveMapCreatorTextboxs[0].Text == "NAME HERE")
                        {
                            saveMapCreatorTextboxs[0].Text = "";
                        }

                        //Updates what the user is typing to the current text box
                        keyboardMouseInput.UpdateKeyboardTyping(keyboardTypingKeys, saveMapCreatorTextboxs[0].Text, false);
                        saveMapCreatorTextboxs[0].Text = keyboardMouseInput.userText;
                    }

                    //Updates the buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateButtons(saveMapCreatorButtons, gameState);

                    //If the game state is the main menu
                    if (gameState == GameState.MainMenu)
                    {
                        //Check if the map does not exist
                        if (!mapSaveLoadHelper.CheckMapExists(saveMapCreatorTextboxs[0].Text))
                        {
                            //If the start tile is null
                            if (startTile == null)
                            {
                                //Set the start tile to the top left corner
                                gameTiles[0, 0].CurrentTileType = TileType.Spawn;
                                startTile = gameTiles[0, 0];
                            }

                            //Saves the new map
                            mapSaveLoadHelper.SaveNewMap(saveMapCreatorTextboxs[0].Text, gameTiles, enemies, startTile);
                        }
                        //If the map does exist, tell the user their map name is invalid
                        else
                        {
                            mapNameInvalid = true;
                            gameState = GameState.SaveCreatorMap;
                            saveMapCreatorTextboxs[0].Text = "";
                        }
                    }

                    break;

                /////////////////////////////////////////////////// CHOOSE MAP \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.ChooseMap:

                    //Checks the scrolling buttons and updates the movement
                    currentButtonMovement = keyboardMouseInput.ScrollButtons(scrollButtonKeys);

                    //Moves the buttons depending on the movement that the user said to move in
                    if (currentButtonMovement == 1)
                    {
                        camera.Move(new Vector2(0, -(TEXT_BOX_HEIGHT + CHOOSING_BUTTON_Y_DISPLACEMENT)));
                    }
                    else if (currentButtonMovement == -1)
                    {
                        camera.Move(new Vector2(0, TEXT_BOX_HEIGHT + CHOOSING_BUTTON_Y_DISPLACEMENT));
                    }

                    //Checks the scrolling buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateScrollingButtons(chooseMapButtons, gameState, camera, SCREEN_HEIGHT, (CAMERA_SCROLLING_DISPLACEMENT / 2));

                    //If the game state is main menu then choosing high scores is set to false
                    if (gameState == GameState.MainMenu)
                    {
                        choosingHighscores = false;
                    }

                    //If the game state is the game
                    if (gameState == GameState.Game)
                    {
                        //Loop for every choose map button
                        for (int i = 0; i < chooseMapButtons.Count; i++)
                        {
                            //If the current button was clicked
                            if (chooseMapButtons[i].isClicked == true)
                            {
                                //If the current button choosing is not the high scores
                                if (!choosingHighscores)
                                {
                                    //If a save of this map exists, let the user choose between either saving or loading. Otherwise, start a new game for the user
                                    if (mapSaveLoadHelper.CheckSaveExists(chooseMapButtons[i].GetText(), currentUser))
                                    {
                                        gameState = GameState.LoadSaveChoice;
                                        currentMap = chooseMapButtons[i].GetText();
                                        chooseMapButtons[i].isClicked = false;
                                    }
                                    else
                                    {
                                        chooseMapButtons[i].isClicked = false;

                                        NewGame(chooseMapButtons[i].GetText());
                                        currentMap = chooseMapButtons[i].GetText();

                                        i = chooseMapButtons.Count;
                                    }
                                }
                                //If the current button choosing is for the high scores
                                else
                                {
                                    //Sets the current map, the button clicked, downloads the current map, and sets the game state to high scores
                                    currentMap = chooseMapButtons[i].GetText();
                                    chooseMapButtons[i].isClicked = false;
                                    highScoresHelper.DownloadHighscoresMap(currentMap);
                                    gameState = GameState.Highscores;

                                    //Clears the list of labels for the high scores
                                    highscoreLabels.Clear();

                                    //Create all the highscore labels
                                    highscoreLabels.Add(new Label("The format for the highscores is Minutes:Seconds:Milliseconds", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), buttonFont, Color.White));
                                    highscoreLabels.Add(new Label("Name:", (int)(SCREEN_WIDTH / 2), SCREEN_HEIGHT, new Vector2(0, 80), enlargedButtonFont, Color.White));
                                    highscoreLabels.Add(new Label("Time:", (int)(SCREEN_WIDTH / 2), SCREEN_HEIGHT, new Vector2(0, 80), enlargedButtonFont, Color.White));

                                    //Integer for the current Y
                                    int currentY = 140;

                                    //Floats for the time that is displaced
                                    float totalMilliseconds;
                                    float minutes;
                                    float seconds;
                                    float milliseconds;

                                    //Loop for every user
                                    for (int j = 0; j < highScoresHelper.UserNames.Count; j++)
                                    {
                                        //Adds the label with the user name
                                        highscoreLabels.Add(new Label(highScoresHelper.UserNames[j], (int)(SCREEN_WIDTH / 2), SCREEN_HEIGHT, new Vector2(0, currentY), buttonFont, Color.White));

                                        //Calculates the time that needs to be displaued
                                        totalMilliseconds = (float)Convert.ToDouble(highScoresHelper.UserTimes[j]);
                                        seconds = (float)Math.Floor(totalMilliseconds / 1000);
                                        minutes = (float)Math.Floor(seconds / 60);
                                        seconds = (float)Math.Floor(seconds % 60);
                                        milliseconds = (float)Math.Round(totalMilliseconds % 1000, 1);

                                        //Adds the label with the current time for the user
                                        highscoreLabels.Add(new Label(minutes + ":" + seconds + ":" + milliseconds, (int)(SCREEN_WIDTH / 2), SCREEN_HEIGHT, new Vector2(0, currentY), buttonFont, Color.White));

                                        //Updates the current Y
                                        currentY += 50;
                                    }

                                    //Aligns all the labels
                                    highscoreLabels[0].CenterTextX();
                                    highscoreLabels[0].AlignTop();
                                    highscoreLabels[1].CenterTextX();
                                    highscoreLabels[2].CenterTextXDisplacement(highscoreDisplacement);

                                    //Loop for every time that is displayed
                                    for (int j = 3; j < highScoresHelper.UserNames.Count + highScoresHelper.UserTimes.Count + 3; j += 2)
                                    {
                                        //Centers the current label for both the names and the times
                                        highscoreLabels[j].CenterTextX();
                                        highscoreLabels[j + 1].CenterTextXDisplacement(highscoreDisplacement);
                                    }
                                }
                            }
                        }
                    }

                    break;

                /////////////////////////////////////////////////// LOAD SAVE CHOICE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.LoadSaveChoice:

                    //Updates the buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateButtons(loadSaveButtons, gameState);

                    //Depending on the current game state, a game is either created new or loaded
                    if (gameState == GameState.NewGame)
                    {
                        NewGame(currentMap);
                    }
                    else if (gameState == GameState.LoadGame)
                    {
                        LoadGame(currentMap, currentUser);
                    }

                    break;

                /////////////////////////////////////////////////// HIGHSCORES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.Highscores:

                    //Updates the buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateButtons(highScoreButtons, gameState);

                    break;

                /////////////////////////////////////////////////// GAME OVER \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.GameOver:

                    //Updates the buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateButtons(gameOverButtons, gameState);

                    //If the game state is the main menu
                    if (gameState == GameState.MainMenu)
                    {
                        //The profile stats are updated depending on the way the game ended
                        if (player.Health <= 0)
                        {
                            profileStatsHelper.UpdateProfileStats(currentUser, enemiesKilled, 100, 1, (float)elapsedTime.Elapsed.TotalMilliseconds, bulletsShot);
                        }
                        else if (enemies.Count == 0)
                        {
                            profileStatsHelper.UpdateProfileStats(currentUser, enemiesKilled, 100 - player.Health, 0, (float)elapsedTime.Elapsed.TotalMilliseconds, bulletsShot);
                            highScoresHelper.UpdateHighscoresMap(currentMap, currentUser, (float)previouslyElapsedTime + (float)elapsedTime.Elapsed.TotalMilliseconds);
                        }
                    }

                    break;

                /////////////////////////////////////////////////// PROFILE STATS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.ProfileStats:

                    //Updates the buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateButtons(profileStatsButtons, gameState);

                    break;

                /////////////////////////////////////////////////// INSTRUCTIONS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.Instructions:

                    //Updates the buttons and sets the game state
                    gameState = keyboardMouseInput.UpdateButtons(instructionButtons, gameState);

                    break;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            switch (gameState)
            {
                /////////////////////////////////////////////////// GAME LOGIN \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.GameLogin:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws all the text boxs
                    for (int i = 0; i < loginTextboxs.Count; i++)
                    {
                        loginTextboxs[i].Draw(spriteBatch, backgroundTexture);
                    }

                    //Draws all the labels
                    for (int i = 0; i < loginLabels.Count; i++)
                    {
                        loginLabels[i].Draw(spriteBatch);
                    }

                    //Draws all the buttons
                    for (int i = 0; i < loginButtons.Count; i++)
                    {
                        loginButtons[i].DrawButton(spriteBatch);
                    }

                    //If the user is taken, the user is informed of that
                    if (userTaken)
                    {
                        spriteBatch.DrawString(buttonFont, "The username enterned for the new account is invalid, please try again.", new Vector2(60, 670), Color.White);
                    }
                    //If the account is invalid, the user is informed of that
                    else if (isAccountInvalid)
                    {
                        spriteBatch.DrawString(buttonFont, "The current information is invalid, please try again.", new Vector2(170, 670), Color.White);
                    }

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// MAIN MENU \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.MainMenu:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws all the buttons
                    for (int i = 0; i < mainMenuButtons.Count; i++)
                    {
                        mainMenuButtons[i].DrawButton(spriteBatch);
                    }

                    //Draws the current user text
                    spriteBatch.DrawString(buttonFont, "Current User: " + currentUser, currentUserPos, Color.White);

                    //If unable to choose map
                    if (unableChooseMap)
                    {
                        //Draws the text informing the user that they are unable to choose a map
                        for (int i = 0; i < mainMenuLabels.Count; i++)
                        {
                            mainMenuLabels[i].Draw(spriteBatch);
                        }
                    }

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// GAME/MAP CREATOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.Game:
                case GameState.MapCreator:

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());

                    //Draws all the tiles
                    foreach (Tile tile in tilesInView)
                    {
                        tile.Draw(spriteBatch);
                    }

                    //Draws the player if the game state is the game
                    if (gameState == GameState.Game)
                    {
                        player.Draw(spriteBatch, backgroundTexture);
                    }

                    //If the game state is the game
                    if (gameState == GameState.Game)
                    {
                        //Draws every enemy with a health bar
                        foreach (Enemy enemy in enemiesInView)
                        {
                            enemy.Draw(spriteBatch, backgroundTexture, true);
                        }
                    }
                    //If the game state is on map creator
                    else
                    {
                        //Draws every enemy without a health bar
                        foreach (Enemy enemy in enemiesInView)
                        {
                            enemy.Draw(spriteBatch, backgroundTexture, false);
                        }
                    }

                    //Draws every projectile
                    foreach (Projectile projectile in projectiles)
                    {
                        projectile.Draw(spriteBatch);
                    }

                    spriteBatch.End();

                    //If the game state is on map creator
                    if (gameState == GameState.MapCreator)
                    {
                        spriteBatch.Begin();

                        //Draws the label
                        mapCreatorLabels[0].Draw(spriteBatch);

                        spriteBatch.End();
                    }
                    //If the game state is on the game
                    else if (gameState == GameState.Game)
                    {
                        spriteBatch.Begin();

                        //Draws the stats for the player
                        spriteBatch.DrawString(buttonFont, "Weapon: " + player.GetCurrentWeaponType().ToString(), new Vector2(10, 10), Color.White);

                        //If the current weapon is not a knife, the ammo for the weapon is shown
                        if (player.CurrentGunAmmo() != -1)
                        {
                            spriteBatch.DrawString(buttonFont, "Current Ammo: " + player.CurrentGunAmmo(), new Vector2(10, 50), Color.White);
                        }
                        //If the current weapon is a knife, infinity for the ammo is shown
                        else
                        {
                            spriteBatch.DrawString(buttonFont, "Current Ammo: Infinity", new Vector2(10, 50), Color.White);
                        }

                        spriteBatch.End();
                    }

                    break;

                /////////////////////////////////////////////////// CHOOSE MAP \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.ChooseMap:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws the button
                    chooseMapButtons[0].DrawButton(spriteBatch);

                    spriteBatch.End();

                    //Begins a sprite batch for the camera class used to scroll
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());

                    //Draws the first label
                    chooseMapLabels[0].Draw(spriteBatch);

                    //For every button
                    for (int i = 1; i < chooseMapButtons.Count; i++)
                    {
                        //If the button intersectcs with the camera's bounds, the button is drawn
                        if (camera.IntersectsScreen(chooseMapButtons[i].GetCurrentRectangle()))
                        {
                            chooseMapButtons[i].DrawButton(spriteBatch);
                        }
                    }

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// PAUSE GAME \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.Pause:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws all the buttons
                    for (int i = 0; i < pauseMenuButtons.Count; i++)
                    {
                        pauseMenuButtons[i].DrawButton(spriteBatch);
                    }

                    //If the map save is available, draws the button for map saving
                    if (saveMapAvailable)
                    {
                        pauseMenuSaveButton[0].DrawButton(spriteBatch);
                    }

                    //If unable to save map, draws the label telling the user that
                    if (unableSaveMap)
                    {
                        pauseMenuLabels[0].Draw(spriteBatch);
                    }

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// MAP CREATOR HELP \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.MapCreatorHelp:

                    spriteBatch.Begin();

                    //Draws the map creator help
                    DrawCreatorHelp();

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// START MAP CREATOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.StartMapCreator:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws all the labels
                    for (int i = 0; i < mapCreatorStartLabels.Count - 1; i++)
                    {
                        mapCreatorStartLabels[i].Draw(spriteBatch);
                    }

                    //Draws all the textboxs
                    for (int i = 0; i < startMapCreatorTextboxs.Count; i++)
                    {
                        startMapCreatorTextboxs[i].Draw(spriteBatch, backgroundTexture);
                    }

                    //Draws all the buttons
                    for (int i = 0; i < startMapCreatorButtons.Count; i++)
                    {
                        startMapCreatorButtons[i].DrawButton(spriteBatch);
                    }

                    //If the map size is invalid, draws the label telling the user that
                    if (invalidMapSize)
                    {
                        mapCreatorStartLabels[mapCreatorStartLabels.Count - 1].Draw(spriteBatch);
                    }

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// SAVE MAP CREATOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.SaveCreatorMap:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws all the labels
                    for (int i = 0; i < saveMapCreatorLabels.Count - 1; i++)
                    {
                        saveMapCreatorLabels[i].Draw(spriteBatch);
                    }

                    //Draws all the buttons
                    for (int i = 0; i < saveMapCreatorButtons.Count; i++)
                    {
                        saveMapCreatorButtons[i].DrawButton(spriteBatch);
                    }

                    //Draws the text box
                    saveMapCreatorTextboxs[0].Draw(spriteBatch, backgroundTexture);

                    //If the map name is invalid, draws the label telling the user to try again
                    if (mapNameInvalid)
                    {
                        saveMapCreatorLabels[2].Draw(spriteBatch);
                    }

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// LOAD/SAVE CHOICE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.LoadSaveChoice:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws all the labels
                    for (int i = 0; i < loadSaveLabels.Count; i++)
                    {
                        loadSaveLabels[i].Draw(spriteBatch);
                    }

                    //Draws every button for the load and save menu
                    for (int i = 0; i < loadSaveButtons.Count; i++)
                    {
                        loadSaveButtons[i].DrawButton(spriteBatch);
                    }

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// HIGHSCORES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.Highscores:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws all the labels
                    for (int i = 0; i < highscoreLabels.Count; i++)
                    {
                        highscoreLabels[i].Draw(spriteBatch);
                    }

                    //Draws the button
                    highScoreButtons[0].DrawButton(spriteBatch);

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// GAME OVER \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.GameOver:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws all the labels
                    for (int i = 0; i < gameOverLabels.Count; i++)
                    {
                        gameOverLabels[i].Draw(spriteBatch);
                    }

                    //Draws the button
                    gameOverButtons[0].DrawButton(spriteBatch);

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// PROFILE STATS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.ProfileStats:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws all the labels
                    for (int i = 0; i < profileStatsLabels.Count; i++)
                    {
                        profileStatsLabels[i].Draw(spriteBatch);
                    }

                    //Draws the button
                    profileStatsButtons[0].DrawButton(spriteBatch);

                    spriteBatch.End();

                    break;

                /////////////////////////////////////////////////// INSTRUCTIONS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                case GameState.Instructions:

                    spriteBatch.Begin();

                    //Draws the background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    //Draws all the labels
                    for (int i = 0; i < instructionsLabels.Count; i++)
                    {
                        instructionsLabels[i].Draw(spriteBatch);
                    }

                    //Draws the button
                    instructionButtons[0].DrawButton(spriteBatch);

                    spriteBatch.End();

                    break;
            }

            base.Draw(gameTime);
        }


        //Pre: The map name
        //Post: None
        //Desc: A method for creating a new game
        private void NewGame(string mapName)
        {
            //The current map is loaded
            mapSaveLoadHelper.LoadNewMap(mapName);

            //The gametiles, start tile, and enemies are set
            gameTiles = mapSaveLoadHelper.GameTiles;
            startTile = mapSaveLoadHelper.StartTile;
            enemies = mapSaveLoadHelper.Enemies;

            //Sets the full screen size, creates a new player instance, and creates a new camera instance
            fullScreenSize = new Rectangle(0, 0, Tile.TILE_X_SIZE * gameTiles.GetLength(0), Tile.TILE_Y_SIZE * gameTiles.GetLength(1));
            player = new Player(startTile.PositionRect.X, startTile.PositionRect.Y, HUMAN_WIDTH, HUMAN_HEIGHT, fullScreenSize.Width, fullScreenSize.Height);
            camera = new Cam2D(GraphicsDevice.Viewport, fullScreenSize, 1, 1, 0, player.GetBounds());
            elapsedTime.Reset();
            elapsedTime.Start();

            //The gamestate is set to game
            gameState = GameState.Game;
        }


        //Pre: The map name and the current user
        //Post: None
        //Desc: A method for loading the game
        private void LoadGame(string mapName, string currentUser)
        {
            //The current map is loaded
            mapSaveLoadHelper.LoadCurrentMap(mapName, currentUser);

            //The gametiles, enemies, projectiles, player, the previously elapsed time, and the enemies killed are set
            gameTiles = mapSaveLoadHelper.GameTiles;
            enemies = mapSaveLoadHelper.Enemies;
            projectiles = mapSaveLoadHelper.Projectiles;
            player = mapSaveLoadHelper.CurrentPlayer;
            previouslyElapsedTime = mapSaveLoadHelper.PreviouslyElapsedTime;
            enemiesKilled = mapSaveLoadHelper.EnemiesPreviouslyKilled;

            //Sets the full screen size, creates a new player instance, and creates a new camera instance
            fullScreenSize = new Rectangle(0, 0, Tile.TILE_X_SIZE * gameTiles.GetLength(0), Tile.TILE_Y_SIZE * gameTiles.GetLength(1));
            camera = new Cam2D(GraphicsDevice.Viewport, fullScreenSize, 1, 1, 0, player.GetBounds());
            elapsedTime.Reset();
            elapsedTime.Start();

            //The gamestate is set to the game
            gameState = GameState.Game;
        }


        //Pre: None
        //Post: None
        //Desc: A method for creating a map so it can changed for playing later
        private void CreateMap()
        {
            //The game tiles, full scree size, the camera, and the player are set
            gameTiles = new Tile[newRowAmount, newColumnAmount];
            fullScreenSize = new Rectangle(0, 0, newRowAmount * Tile.TILE_X_SIZE, newColumnAmount * Tile.TILE_Y_SIZE);
            camera = new Cam2D(GraphicsDevice.Viewport, fullScreenSize, 1, 1, 0, new Rectangle(0, 0, 0, 0));
            player = null;

            //The enemies are cleared
            enemies.Clear();

            //Loop for every value in the first dimension
            for (int i = 0; i < gameTiles.GetLength(0); i++)
            {
                //Loop for every value in the second dimension
                for (int j = 0; j < gameTiles.GetLength(1); j++)
                {
                    //Sets the new tile to blank
                    gameTiles[i, j] = new Tile(i, j, TileType.Blank);
                }
            }

            //Sets the game state to map creator and resets the position in the keyboard mouse input class
            gameState = GameState.MapCreator;
            keyboardMouseInput.ResetPosition();
        }


        //Pre: None
        //Post: None
        //Desc: The keys are set to be checked later
        private void SetKeys()
        {
            //Game keyboard keys are set
            gameKeyboardKeys.Add(Keys.NumPad1);
            gameKeyboardKeys.Add(Keys.NumPad2);
            gameKeyboardKeys.Add(Keys.NumPad3);
            gameKeyboardKeys.Add(Keys.NumPad4);
            gameKeyboardKeys.Add(Keys.NumPad5);
            gameKeyboardKeys.Add(Keys.D1);
            gameKeyboardKeys.Add(Keys.D2);
            gameKeyboardKeys.Add(Keys.D3);
            gameKeyboardKeys.Add(Keys.D4);
            gameKeyboardKeys.Add(Keys.D5);
            gameKeyboardKeys.Add(Keys.W);
            gameKeyboardKeys.Add(Keys.S);
            gameKeyboardKeys.Add(Keys.A);
            gameKeyboardKeys.Add(Keys.D);
            gameKeyboardKeys.Add(Keys.Down);
            gameKeyboardKeys.Add(Keys.Up);
            gameKeyboardKeys.Add(Keys.Left);
            gameKeyboardKeys.Add(Keys.Right);
            gameKeyboardKeys.Add(Keys.Escape);
            gameKeyboardKeys.Add(Keys.H);

            //Keyboard typing keys are set
            keyboardTypingKeys.Add(Keys.Q);
            keyboardTypingKeys.Add(Keys.W);
            keyboardTypingKeys.Add(Keys.E);
            keyboardTypingKeys.Add(Keys.R);
            keyboardTypingKeys.Add(Keys.T);
            keyboardTypingKeys.Add(Keys.Y);
            keyboardTypingKeys.Add(Keys.U);
            keyboardTypingKeys.Add(Keys.I);
            keyboardTypingKeys.Add(Keys.O);
            keyboardTypingKeys.Add(Keys.P);
            keyboardTypingKeys.Add(Keys.A);
            keyboardTypingKeys.Add(Keys.S);
            keyboardTypingKeys.Add(Keys.D);
            keyboardTypingKeys.Add(Keys.F);
            keyboardTypingKeys.Add(Keys.G);
            keyboardTypingKeys.Add(Keys.H);
            keyboardTypingKeys.Add(Keys.J);
            keyboardTypingKeys.Add(Keys.K);
            keyboardTypingKeys.Add(Keys.L);
            keyboardTypingKeys.Add(Keys.Z);
            keyboardTypingKeys.Add(Keys.X);
            keyboardTypingKeys.Add(Keys.C);
            keyboardTypingKeys.Add(Keys.V);
            keyboardTypingKeys.Add(Keys.B);
            keyboardTypingKeys.Add(Keys.N);
            keyboardTypingKeys.Add(Keys.M);
            keyboardTypingKeys.Add(Keys.LeftShift);
            keyboardTypingKeys.Add(Keys.RightShift);
            keyboardTypingKeys.Add(Keys.Back);
            keyboardTypingKeys.Add(Keys.LeftControl);
            keyboardTypingKeys.Add(Keys.RightControl);

            //Keyboard num keys are set
            keyboardNumKeys.Add(Keys.D0);
            keyboardNumKeys.Add(Keys.D1);
            keyboardNumKeys.Add(Keys.D2);
            keyboardNumKeys.Add(Keys.D3);
            keyboardNumKeys.Add(Keys.D4);
            keyboardNumKeys.Add(Keys.D5);
            keyboardNumKeys.Add(Keys.D6);
            keyboardNumKeys.Add(Keys.D7);
            keyboardNumKeys.Add(Keys.D8);
            keyboardNumKeys.Add(Keys.D9);
            keyboardNumKeys.Add(Keys.Back);

            //Scroll button keys are set
            scrollButtonKeys.Add(Keys.Up);
            scrollButtonKeys.Add(Keys.Down);
            scrollButtonKeys.Add(Keys.W);
            scrollButtonKeys.Add(Keys.S);
        }


        //Pre: None
        //Post: None
        //Desc: A method for creating all the labels
        private void CreateLabels()
        {
            //Creates all the map creator labels
            mapCreatorLabels.Add(new Label("Press H at any time for help", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), buttonFont, Color.Snow));
            mapCreatorLabels.Add(new Label("Map Creator Help", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), titleFont, Color.White));
            mapCreatorLabels.Add(new Label("Press the 1 key to be able to place a regular blank tile", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 140), buttonFont, Color.White));
            mapCreatorLabels.Add(new Label("Press the 2 key to be able to place a save tile", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 210), buttonFont, Color.White));
            mapCreatorLabels.Add(new Label("Press the 3 key to be able to place a spawn tile", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 280), buttonFont, Color.White));
            mapCreatorLabels.Add(new Label("Press the 4 key to be able to place a wall tile", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 350), buttonFont, Color.White));
            mapCreatorLabels.Add(new Label("Press the 5 key to be able to place an enemy on a tile", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 420), buttonFont, Color.White));
            mapCreatorLabels.Add(new Label("When finished creating the map, press the ESCAPE key in the map creator.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 490), buttonFont, Color.White));
            mapCreatorLabels.Add(new Label("This gives you the option to save your map.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 520), buttonFont, Color.White));
            mapCreatorLabels.Add(new Label("Enjoy creating your map!", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 590), buttonFont, Color.White));
            mapCreatorLabels.Add(new Label("Press the ESCAPE key to go back to creating a map", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), buttonFont, Color.White));

            //Aligns specific map creator labels to certain positions
            mapCreatorLabels[0].AlignBottom();
            mapCreatorLabels[0].AlignLeft();
            mapCreatorLabels[1].CenterTextX();
            mapCreatorLabels[1].AlignTop();

            //Aligns every label that hasn't been aligned yet to the left
            for (int i = 2; i < mapCreatorLabels.Count; i++)
            {
                mapCreatorLabels[i].AlignLeft();
            }

            //Aligns the last label in the map creator labels list to the bottom
            mapCreatorLabels[mapCreatorLabels.Count - 1].AlignBottom();

            //Creates all the login menu labels
            loginLabels.Add(new Label("Username:", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(loginTextboxs[0].GetCurrentRectangle().X -
                titleFont.MeasureString("Username:").X - LABEL_TEXT_BOX_X, loginTextboxs[0].GetCurrentRectangle().Y - (titleFont.MeasureString("Test").Y / 2)
                + (int)(loginTextboxs[0].GetCurrentRectangle().Height / 2)), titleFont, Color.White));
            loginLabels.Add(new Label("Password:", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(loginTextboxs[1].GetCurrentRectangle().X -
                titleFont.MeasureString("Password:").X - LABEL_TEXT_BOX_X, loginTextboxs[1].GetCurrentRectangle().Y - (titleFont.MeasureString("Test").Y / 2)
                + (int)(loginTextboxs[0].GetCurrentRectangle().Height / 2)), titleFont, Color.White));

            //Creates all the labels for the start map creator menu
            mapCreatorStartLabels.Add(new Label("Row Amount:", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(startMapCreatorTextboxs[0].GetCurrentRectangle().X -
                titleFont.MeasureString("Row Amount:").X - LABEL_TEXT_BOX_X, startMapCreatorTextboxs[0].GetCurrentRectangle().Y - (titleFont.MeasureString("Row Amount:").Y / 2)
                + (int)(startMapCreatorTextboxs[0].GetCurrentRectangle().Height / 2)), titleFont, Color.White));
            mapCreatorStartLabels.Add(new Label("Col. Amount:", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(startMapCreatorTextboxs[1].GetCurrentRectangle().X -
                titleFont.MeasureString("Col. Amount:").X - LABEL_TEXT_BOX_X, startMapCreatorTextboxs[1].GetCurrentRectangle().Y - (titleFont.MeasureString("Col. Amount:").Y / 2)
                + (int)(startMapCreatorTextboxs[1].GetCurrentRectangle().Height / 2)), titleFont, Color.White));
            mapCreatorStartLabels.Add(new Label("The Row and Column must be between " + MIN_MAP_SIZE + " and " + MAX_MAP_SIZE + " tiles.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), buttonFont, Color.White));
            mapCreatorStartLabels.Add(new Label("The current map size is invalid, please try again.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), buttonFont, Color.White));

            //Aligns the starting map creator labels
            mapCreatorStartLabels[2].AlignTop();
            mapCreatorStartLabels[2].AlignLeft();
            mapCreatorStartLabels[3].AlignBottom();
            mapCreatorStartLabels[3].CenterTextX();

            //Creates the labels for the saving the created map menu
            saveMapCreatorLabels.Add(new Label("Type the name of the map below to save your map.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 40), enlargedButtonFont, Color.White));
            saveMapCreatorLabels.Add(new Label("Name:", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(saveMapCreatorTextboxs[0].GetCurrentRectangle().X -
                titleFont.MeasureString("Name:").X - LABEL_TEXT_BOX_X, saveMapCreatorTextboxs[0].GetCurrentRectangle().Y - (titleFont.MeasureString("Name:").Y / 2)
                + (int)(saveMapCreatorTextboxs[0].GetCurrentRectangle().Height / 2)), titleFont, Color.White));
            saveMapCreatorLabels.Add(new Label("The current map name is invalid, please try again.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), buttonFont, Color.White));

            //Aligns the saving the map creator menu labels
            saveMapCreatorLabels[0].CenterTextX();
            saveMapCreatorLabels[2].AlignBottom();
            saveMapCreatorLabels[2].CenterTextX();

            //Creates the main menu labels
            mainMenuLabels.Add(new Label("Choosing Maps is", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(20, 150), buttonFont, Color.White));
            mainMenuLabels.Add(new Label("unavailable, please", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(20, 180), buttonFont, Color.White));
            mainMenuLabels.Add(new Label("try again.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(20, 210), buttonFont, Color.White));

            //Creates the labels for the choosing whether a loaded game or saved game is played
            loadSaveLabels.Add(new Label("Would you like to continue your saved map or restart?", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 100), buttonFont, Color.White));

            //Aligns the load save menu labels
            loadSaveLabels[0].CenterTextX();

            //Creates the pause menu labels
            pauseMenuLabels.Add(new Label("Unable to save map", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), buttonFont, Color.White));

            //Aligns the pause menu labels
            pauseMenuLabels[0].AlignBottom();
            pauseMenuLabels[0].CenterTextX();

            //Creates all the instruction labels
            instructionsLabels.Add(new Label("Game Instructions", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 0), titleFont, Color.White));
            instructionsLabels.Add(new Label("The point of the game is to kill all the enemies in a certain map  at the fastest", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 120), buttonFont, Color.White));
            instructionsLabels.Add(new Label("time possible. Once all the enemies are killed, the map will be complete and", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 155), buttonFont, Color.White));
            instructionsLabels.Add(new Label("your time will be saved in the highscores which can be viewed by anyone.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 190), buttonFont, Color.White));
            instructionsLabels.Add(new Label("To be able to kill the enemies, you are provided with certain weapons.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 245), buttonFont, Color.White));
            instructionsLabels.Add(new Label("To select each weapon in game, press:", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 280), buttonFont, Color.White));
            instructionsLabels.Add(new Label("The 1 key to use a handgun", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 315), buttonFont, Color.White));
            instructionsLabels.Add(new Label("The 2 key to use a rifle", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 350), buttonFont, Color.White));
            instructionsLabels.Add(new Label("The 3 key to use a shotgun", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 385), buttonFont, Color.White));
            instructionsLabels.Add(new Label("The 4 key to throw knives at enemies", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 420), buttonFont, Color.White));
            instructionsLabels.Add(new Label("During the game, to shoot your weapon press the left mouse button", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 475), buttonFont, Color.White));
            instructionsLabels.Add(new Label("During the game, to reload your weapon press the right mouse button", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 510), buttonFont, Color.White));
            instructionsLabels.Add(new Label("During the game, pressing escape when the player is on a save location,", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 545), buttonFont, Color.White));
            instructionsLabels.Add(new Label("will give you the ability to save and exit", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 580), buttonFont, Color.White));

            //Aligns specific instructions labels to certain positions
            instructionsLabels[0].CenterTextX();
            instructionsLabels[0].AlignTop();

            //Aligns every label that hasn't been aligned yet to the left
            for (int i = 1; i < instructionsLabels.Count; i++)
            {
                instructionsLabels[i].AlignLeft();
            }

            //Adds the choose map label
            chooseMapLabels.Add(new Label("Use the Up/Down arrow keys or W/S keys to scroll through map choices", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(0, 5), buttonFont, Color.White));

            //Aligns the choose map label
            chooseMapLabels[0].CenterTextX();
        }


        //Pre: None
        //Post: None
        //Desc: A method for creating teh text boxs
        private void CreateTextBoxs()
        {
            //Creates the textboxs for the login screen with positions that have it looks nice in the game
            loginTextboxs.Add(new TextBox(new Rectangle(450, 150, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT), SCREEN_WIDTH, SCREEN_HEIGHT, buttonFont, "USER HERE", Color.Red, Color.Yellow, Color.Black));
            loginTextboxs.Add(new TextBox(new Rectangle(450, 350, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT), SCREEN_WIDTH, SCREEN_HEIGHT, buttonFont, "PASS HERE", Color.Red, Color.Yellow, Color.Black));

            //Creates the textboxs for the map creator screen size choosing menu
            startMapCreatorTextboxs.Add(new TextBox(new Rectangle(450, 150, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT), SCREEN_WIDTH, SCREEN_HEIGHT, buttonFont, "ROW NUM", Color.Red, Color.Yellow, Color.Black));
            startMapCreatorTextboxs.Add(new TextBox(new Rectangle(450, 350, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT), SCREEN_WIDTH, SCREEN_HEIGHT, buttonFont, "COLUMN NUM", Color.Red, Color.Yellow, Color.Black));

            //Creates the textboxs for the saving the map of the map creator menu
            saveMapCreatorTextboxs.Add(new TextBox(new Rectangle(450, 250, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT), SCREEN_WIDTH, SCREEN_HEIGHT, buttonFont, "NAME HERE", Color.Red, Color.Yellow, Color.Black));
        }


        //Pre: None
        //Post: None
        //Desc: A method for creating the buttons
        private void CreateButtons()
        {
            //Creates the login buttons
            loginButtons.Add(new Button(new Rectangle(158, 550, 284, 70), new Rectangle(150, 540, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Login", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MainMenu));
            loginButtons.Add(new Button(new Rectangle(582, 550, 284, 70), new Rectangle(574, 540, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Create Account", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.AccountCreator));

            //Sets the texture data for the login buttons
            loginButtons[0].SetTextureData(graphics);
            loginButtons[1].SetTextureData(graphics);

            //Creates the main menu buttons
            mainMenuButtons.Add(new Button(new Rectangle(370, 59, 284, 60), new Rectangle(362, 49, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Play Game", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.ChooseMap));
            mainMenuButtons.Add(new Button(new Rectangle(370, 159, 284, 60), new Rectangle(362, 149, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Create Map", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.StartMapCreator));
            mainMenuButtons.Add(new Button(new Rectangle(370, 259, 284, 60), new Rectangle(362, 249, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Highscores", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.Highscores));
            mainMenuButtons.Add(new Button(new Rectangle(370, 359, 284, 60), new Rectangle(362, 349, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Profile Stats", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.ProfileStats));
            mainMenuButtons.Add(new Button(new Rectangle(370, 459, 284, 60), new Rectangle(362, 449, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Instructions", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.Instructions));
            mainMenuButtons.Add(new Button(new Rectangle(370, 559, 284, 60), new Rectangle(362, 549, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Sign Out", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.SignOut));
            mainMenuButtons.Add(new Button(new Rectangle(370, 659, 284, 60), new Rectangle(362, 649, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Exit Game", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.ExitGame));

            //Sets the texture data for all the main menu buttons
            for (int i = 0; i < mainMenuButtons.Count; i++)
            {
                mainMenuButtons[i].SetTextureData(graphics);
            }

            //Creates the login buttons
            startMapCreatorButtons.Add(new Button(new Rectangle(158, 550, 284, 70), new Rectangle(150, 540, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Go Back", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MainMenu));
            startMapCreatorButtons.Add(new Button(new Rectangle(582, 550, 284, 70), new Rectangle(574, 540, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Begin Creation", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MapCreator));

            //Sets the texture data for all the map creator start menu buttons
            for (int i = 0; i < startMapCreatorButtons.Count; i++)
            {
                startMapCreatorButtons[i].SetTextureData(graphics);
            }

            //Creates the buttons for the map creator saving menu
            saveMapCreatorButtons.Add(new Button(new Rectangle(158, 550, 284, 70), new Rectangle(150, 540, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Go Back", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MapCreator));
            saveMapCreatorButtons.Add(new Button(new Rectangle(582, 550, 284, 70), new Rectangle(574, 540, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Save Map", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MainMenu));

            //Sets the texture data for all the map creator saving menu buttons
            for (int i = 0; i < saveMapCreatorButtons.Count; i++)
            {
                saveMapCreatorButtons[i].SetTextureData(graphics);
            }

            //Creates all the pause menu buttons
            pauseMenuButtons.Add(new Button(new Rectangle(370, 159, 284, 60), new Rectangle(362, 149, 300, 80), new Vector2(20, 20), Color.Black, Color.Red, "Resume",
                buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.Game));
            pauseMenuButtons.Add(new Button(new Rectangle(370, 259, 284, 60), new Rectangle(362, 249, 300, 80), new Vector2(20, 20), Color.Black, Color.Red, "Instructions",
                buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.Instructions));
            pauseMenuButtons.Add(new Button(new Rectangle(370, 359, 284, 60), new Rectangle(362, 349, 300, 80), new Vector2(20, 20), Color.Black, Color.Red, "Main Menu",
                buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MainMenu));
            pauseMenuSaveButton.Add(new Button(new Rectangle(370, 459, 284, 60), new Rectangle(362, 449, 300, 80), new Vector2(20, 20), Color.Black, Color.Red, "Exit and Save",
                buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.SaveGame));

            //Sets the texture data for all the pause menu buttons
            pauseMenuSaveButton[0].SetTextureData(graphics);

            //Sets the texture data for all the pause menu buttons
            for (int i = 0; i < pauseMenuButtons.Count; i++)
            {
                pauseMenuButtons[i].SetTextureData(graphics);
            }

            //Creates the buttons for the choosing between loading and saving a game menu
            loadSaveButtons.Add(new Button(new Rectangle(158, 450, 284, 70), new Rectangle(150, 440, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Continue Game", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.LoadGame));
            loadSaveButtons.Add(new Button(new Rectangle(582, 450, 284, 70), new Rectangle(574, 440, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "New Game", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.NewGame));
            loadSaveButtons.Add(new Button(new Rectangle(370, 600, 284, 70), new Rectangle(362, 590, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Go Back", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.ChooseMap));


            //Sets the texture data for the choosing between loading and saving a game menu
            for (int i = 0; i < loadSaveButtons.Count; i++)
            {
                loadSaveButtons[i].SetTextureData(graphics);
            }

            //Creates the highscore button
            highScoreButtons.Add(new Button(new Rectangle(370, 670, 284, 60), new Rectangle(362, 660, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Go Back", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.ChooseMap));

            //Sets the texture data to the highscore button
            highScoreButtons[0].SetTextureData(graphics);

            //Creates the game over button
            gameOverButtons.Add(new Button(new Rectangle(370, 670, 284, 60), new Rectangle(362, 660, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Main Menu", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MainMenu));

            //Sets the texture data to the game over button
            gameOverButtons[0].SetTextureData(graphics);

            //Creates the profile stats button
            profileStatsButtons.Add(new Button(new Rectangle(370, 670, 284, 60), new Rectangle(362, 660, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Go Back", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MainMenu));

            //Sets the texture data to the profile stats button
            profileStatsButtons[0].SetTextureData(graphics);

            //Creates the instructions button
            instructionButtons.Add(new Button(new Rectangle(370, 670, 284, 60), new Rectangle(362, 660, 300, 80), new Vector2(20, 20), Color.Black, Color.Red,
                "Go Back", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.MainMenu));

            //Sets the texture data to the instructios button
            instructionButtons[0].SetTextureData(graphics);
        }


        //Pre: None
        //Post: None
        //Desc: A method for checking the collision between bullets and enemies/player
        private void CheckBulletCollision()
        {
            //The return objects are cleared and then the quad tree retrieves all the projectiles in the same quadrant as the player
            returnObjects.Clear();
            quadTree.Retrieve(returnObjects, player);

            //Any doubles in the list are removed
            returnObjects = returnObjects.Distinct().ToList();

            //If there is one or more projectiles
            if (returnObjects.Count > 0)
            {
                //Loop for every projectile
                for (int i = 0; i < returnObjects.Count; i++)
                {
                    //If the player collides with the projectile, the projectile is set to destroy and the player loses health
                    if (!returnObjects[i].NeedDestroy && !returnObjects[i].PlayerBullet && CollisionDetection.Collision(player.GetBounds(),
                        returnObjects[i].GetBounds(), player.GetCurrentTexture(), returnObjects[i].GetTexture()))
                    {
                        player.Health -= projectiles[i].Damage;
                        returnObjects[i].NeedDestroy = true;
                    }
                }
            }

            //Loop for every enemy
            foreach (Enemy enemy in enemies)
            {
                //The return objects are cleared and then the quad tree retrieves all the projectiles in the same quadrant as the current enemy
                returnObjects.Clear();
                quadTree.Retrieve(returnObjects, enemy);

                //Any doubles in the list are removed
                returnObjects = returnObjects.Distinct().ToList();

                //If there is one or more projectiles
                if (returnObjects.Count > 0)
                {
                    //Loop for every projectile
                    for (int i = 0; i < returnObjects.Count; i++)
                    {
                        //If the enemy collides with the projectile
                        if (!returnObjects[i].NeedDestroy && CollisionDetection.Collision(enemy.GetBounds(), returnObjects[i].GetBounds(), enemy.GetCurrentTexture(),
                            returnObjects[i].GetTexture()))
                        {
                            //If the projectile is the players, the enemy loses health, the projectile is set to be destroyed, and the enemies killed is incremented
                            if (returnObjects[i].PlayerBullet)
                            {
                                enemy.Health -= projectiles[i].Damage;
                                returnObjects[i].NeedDestroy = true;
                                enemiesKilled++;
                            }
                            //If the projectile is not the player's
                            else
                            {
                                //If the projectile owner is not the current enemy, the enemy loses health and the projectile is set to be destroyed
                                if (returnObjects[i].BulletOwner != enemy)
                                {
                                    enemy.Health -= projectiles[i].Damage;
                                    returnObjects[i].NeedDestroy = true;
                                }
                            }
                        }
                    }
                }
            }

            //Loop for every projectile
            for (int i = 0; i < projectiles.Count; i++)
            {
                //If the current projectile needs to be destroyed, it is destroyed
                if (projectiles[i].NeedDestroy)
                {
                    projectiles[i] = null;
                    projectiles.RemoveAt(i);
                    i--;
                }
            }
        }


        //Pre: None
        //Post: None
        //Desc: A method for drawing the creator help
        private void DrawCreatorHelp()
        {
            //The background is drawn
            spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

            //Every label is drawn
            for (int i = 1; i < mapCreatorLabels.Count; i++)
            {
                mapCreatorLabels[i].Draw(spriteBatch);
            }
        }
    }
}
