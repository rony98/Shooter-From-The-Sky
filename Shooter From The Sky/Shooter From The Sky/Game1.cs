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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
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

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Tile[,] gameTiles = new Tile[50, 50];
        Vector2 spawnLocation = new Vector2(64, 64);
        Rectangle fullScreenSize = new Rectangle(0, 0, 3200, 3200);

        List<Enemy> enemiesInView = new List<Enemy>();
        List<Tile> tilesInView = new List<Tile>();

        Player player;
        List<Enemy> enemies = new List<Enemy>();

        List<Projectile> projectiles = new List<Projectile>();

        Tile startTile;

        GameState gameState = GameState.GameLogin;

        List<Keys> gameKeyboardKeys = new List<Keys>();
        List<Keys> keyboardTypingKeys = new List<Keys>();
        List<Keys> keyboardNumKeys = new List<Keys>();

        KeyboardMouseInput keyboardMouseInput = new KeyboardMouseInput();

        QuadTree quadTree = new QuadTree(0, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT));
        List<Projectile> returnObjects = new List<Projectile>();

        Cam2D camera;

        OnlineStorageHelper onlineHelper = new OnlineStorageHelper();
        ProfileHelper profileHelper;

        string currentUser = "";
        Vector2 currentUserPos = new Vector2(20, 20);
        bool isAccountInvalid = false;
        bool userTaken = false;

        bool invalidMapSize = false;

        SpriteFont buttonFont;
        SpriteFont enlargedButtonFont;
        SpriteFont titleFont;

        Texture2D backgroundTexture;
        Rectangle backgroundRect = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

        List<Label> mapCreatorLabels = new List<Label>();
        List<Label> mapCreatorStartLabel = new List<Label>();
        List<Label> loginLabels = new List<Label>();

        List<TextBox> loginTextboxs = new List<TextBox>();
        List<TextBox> startMapCreatorTextboxs = new List<TextBox>();

        List<Button> loginButtons = new List<Button>();
        List<Button> mainMenuButtons = new List<Button>();
        List<Button> startMapCreatorButtons = new List<Button>();

        int newRowAmount = 0;
        int newColumnAmount = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;

            graphics.PreferMultiSampling = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            camera = new Cam2D(GraphicsDevice.Viewport, fullScreenSize, 1, 1, 0, new Rectangle(0, 0, 0, 0));

            profileHelper = new ProfileHelper(onlineHelper);

            buttonFont = this.Content.Load<SpriteFont>("Fonts\\ButtonFont");
            enlargedButtonFont = this.Content.Load<SpriteFont>("Fonts\\EnlargedButtonFont");
            titleFont = this.Content.Load<SpriteFont>("Fonts\\TitleFont");

            backgroundTexture = new Texture2D(GraphicsDevice, 1, 1);
            backgroundTexture.SetData(new Color[] { Color.White });

            CreateTextBoxs();
            CreateLabels();
            CreateButtons();

            SetKeys();

            // TODO: use this.Content to load your game content here
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

            switch (gameState)
            {
                case GameState.GameLogin:
                    keyboardMouseInput.UpdateTextBoxes(loginTextboxs);

                    for (int i = 0; i < loginTextboxs.Count; i++)
                    {
                        if (loginTextboxs[i].TextBoxClicked)
                        {
                            if (loginTextboxs[i].Text == "USER HERE" || loginTextboxs[i].Text == "PASS HERE")
                            {
                                loginTextboxs[i].Text = "";
                            }

                            keyboardMouseInput.UpdateKeyboardTyping(keyboardTypingKeys, loginTextboxs[i].Text, false);
                            loginTextboxs[i].Text = keyboardMouseInput.userText;
                        }
                    }

                    gameState = keyboardMouseInput.UpdateButtons(loginButtons, gameState);

                    if (gameState == GameState.MainMenu)
                    {
                        userTaken = false;

                        if (profileHelper.CheckForProfile(loginTextboxs[0].Text, loginTextboxs[1].Text))
                        {
                            currentUser = loginTextboxs[0].Text;
                        }
                        else
                        {
                            gameState = GameState.GameLogin;
                            isAccountInvalid = true;
                        }
                    }
                    else if (gameState == GameState.AccountCreator)
                    {
                        if (loginTextboxs[0].Text != "" && loginTextboxs[1].Text != "")
                        {
                            if (profileHelper.AddProfile(loginTextboxs[0].Text, loginTextboxs[1].Text))
                            {
                                currentUser = loginTextboxs[0].Text;
                                gameState = GameState.MainMenu;
                            }
                            else
                            {
                                userTaken = true;
                                gameState = GameState.GameLogin;
                            }
                        }
                        else
                        {
                            isAccountInvalid = true;
                            gameState = GameState.GameLogin;
                        }
                    }

                    break;
                case GameState.StartMapCreator:

                    keyboardMouseInput.UpdateTextBoxes(startMapCreatorTextboxs);

                    for (int i = 0; i < startMapCreatorTextboxs.Count; i++)
                    {
                        if (startMapCreatorTextboxs[i].TextBoxClicked)
                        {
                            if (startMapCreatorTextboxs[i].Text == "ROW NUM" || startMapCreatorTextboxs[i].Text == "COLUMN NUM")
                            {
                                startMapCreatorTextboxs[i].Text = "";
                            }

                            keyboardMouseInput.UpdateKeyboardTyping(keyboardNumKeys, startMapCreatorTextboxs[i].Text, true);
                            startMapCreatorTextboxs[i].Text = keyboardMouseInput.userText;
                        }
                    }

                    gameState = keyboardMouseInput.UpdateButtons(startMapCreatorButtons, gameState);

                    if (gameState == GameState.MapCreator)
                    {
                        if (Convert.ToInt32(startMapCreatorTextboxs[0].Text) <= 200 && Convert.ToInt32(startMapCreatorTextboxs[1].Text) <= 200
                            && Convert.ToInt32(startMapCreatorTextboxs[0].Text) >= 15 && Convert.ToInt32(startMapCreatorTextboxs[1].Text) >= 15)
                        {
                            newRowAmount = Convert.ToInt32(startMapCreatorTextboxs[0].Text);
                            newColumnAmount = Convert.ToInt32(startMapCreatorTextboxs[1].Text);

                            CreateMap();
                        }
                        else
                        {
                            startMapCreatorTextboxs[0].Text = "";
                            startMapCreatorTextboxs[1].Text = "";

                            gameState = GameState.StartMapCreator;

                            invalidMapSize = true;
                        }
                    }

                    break;
                case GameState.MainMenu:

                    gameState = keyboardMouseInput.UpdateButtons(mainMenuButtons, gameState);

                    switch (gameState)
                    {
                        case GameState.StartMapCreator:

                            startMapCreatorTextboxs[0].Text = "ROW NUM";
                            startMapCreatorTextboxs[1].Text = "COLUMN NUM";

                            break;
                        case GameState.ChooseMap:

                            //Download all the maps and display all the avialable maps to the user
                            //After they click a map, if a load of it is possible ask the user if they want to load the map

                            break;
                        case GameState.ExitGame:

                            this.Exit();

                            break;
                        case GameState.SignOut:

                            gameState = GameState.GameLogin;

                            loginTextboxs[0].Text = "USER HERE";
                            loginTextboxs[1].Text = "PASS HERE";

                            for (int i = 0; i < loginTextboxs.Count; i++)
                            {
                                loginTextboxs[i].TextBoxClicked = false;
                            }

                            currentUser = "";

                            break;
                    }

                    break;
                case GameState.Game:

                    if (!IsActive)
                    {
                        gameState = GameState.Pause;
                    }

                    gameState = keyboardMouseInput.UpdateGameControls(gameKeyboardKeys, player, gameState, gameTiles);

                    player.Update();

                    camera.LookAt(player.GetBounds());

                    player.CalcRotation(keyboardMouseInput.mouse.X + (camera.GetPosition().X - (SCREEN_WIDTH / 2)),
                        keyboardMouseInput.mouse.Y + (camera.GetPosition().Y - (SCREEN_HEIGHT / 2)));

                    projectiles.AddRange(player.Projectiles);
                    player.Projectiles.Clear();

                    enemiesInView.Clear();
                    tilesInView.Clear();

                    foreach (Tile tile in gameTiles)
                    {
                        if (camera.IntersectsScreen(tile.PositionRect))
                        {
                            tilesInView.Add(tile);
                        }
                    }

                    foreach (Enemy enemy in enemies)
                    {
                        if (camera.IntersectsScreen(enemy.GetBounds()))
                        {
                            enemiesInView.Add(enemy);
                        }
                    }

                    for (int i = 0; i < enemiesInView.Count; i++)
                    {
                        enemiesInView[i].Update(player, gameTiles);

                        projectiles.AddRange(enemiesInView[i].Projectiles);
                        enemiesInView[i].Projectiles.Clear();

                        if (enemiesInView[i].Health <= 0)
                        {
                            enemies.Remove(enemiesInView[i]);
                            enemiesInView[i] = null;
                            enemiesInView.RemoveAt(i);
                        }
                    }

                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        projectiles[i].Update(gameTiles, new Vector2(camera.GetPosition().X + (SCREEN_WIDTH / 2), camera.GetPosition().Y + (SCREEN_HEIGHT / 2)));

                        if (projectiles[i].NeedDestroy)
                        {
                            projectiles[i] = null;
                            projectiles.RemoveAt(i);
                            i--;
                        }
                    }

                    quadTree.Clear();

                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        quadTree.Insert(projectiles[i]);
                    }

                    CheckBulletCollision();

                    break;
                case GameState.Pause:
                    break;
                case GameState.MapCreatorHelp:

                    if (IsActive)
                    {
                        //Sets the game state using the returned value form the key checking class
                        gameState = keyboardMouseInput.UpdateCreatorHelp(gameState);
                    }

                    break;
                case GameState.MapCreator:

                    if (IsActive)
                    {
                        gameState = keyboardMouseInput.UpdateCreatorControls(gameKeyboardKeys, gameTiles, gameState, camera);
                    }

                    enemiesInView.Clear();
                    tilesInView.Clear();

                    foreach (Tile tile in gameTiles)
                    {
                        if (camera.IntersectsScreen(tile.PositionRect))
                        {
                            tilesInView.Add(tile);
                        }
                    }

                    foreach (Enemy enemy in enemies)
                    {
                        if (camera.IntersectsScreen(enemy.GetBounds()))
                        {
                            enemiesInView.Add(enemy);
                        }
                    }


                    if (IsActive)
                    {
                        if (keyboardMouseInput.mouse.LeftButton == ButtonState.Pressed)
                        {
                            int row = (int)((keyboardMouseInput.mouse.X + (camera.GetPosition().X - (SCREEN_WIDTH / 2))) / Tile.TILE_X_SIZE);
                            int column = (int)((keyboardMouseInput.mouse.Y + (camera.GetPosition().Y - (SCREEN_HEIGHT / 2))) / Tile.TILE_Y_SIZE);

                            if (row >= 0 && column >= 0 && row <= gameTiles.GetLength(0) && column <= gameTiles.GetLength(1))
                            {
                                if (!keyboardMouseInput.PlacingEnemy)
                                {
                                    if (keyboardMouseInput.CurrentTileType == TileType.Spawn)
                                    {
                                        if (keyboardMouseInput.CheckMouse())
                                        {
                                            if (startTile != null)
                                            {
                                                gameTiles[startTile.Row, startTile.Column].CurrentTileType = TileType.Blank;
                                            }

                                            startTile = gameTiles[row, column];
                                            gameTiles[row, column].CurrentTileType = TileType.Spawn;
                                        }
                                    }
                                    else
                                    {
                                        if (gameTiles[row, column].CurrentTileType == TileType.Spawn)
                                        {
                                            startTile = null;
                                            gameTiles[row, column].CurrentTileType = keyboardMouseInput.CurrentTileType;
                                        }
                                        else
                                        {
                                            gameTiles[row, column].CurrentTileType = keyboardMouseInput.CurrentTileType;
                                        }
                                    }

                                    if (gameTiles[row, column].EnemyExists)
                                    {
                                        gameTiles[row, column].EnemyExists = false;
                                        enemies.Remove(gameTiles[row, column].EnemyPlaced);
                                        gameTiles[row, column].EnemyPlaced = null;
                                    }
                                }
                                else
                                {
                                    if (!gameTiles[row, column].EnemyExists)
                                    {
                                        if (gameTiles[row, column].CurrentTileType == TileType.Spawn)
                                        {
                                            startTile = null;
                                        }

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

                    break;
                default:
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
                case GameState.GameLogin:

                    spriteBatch.Begin();

                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    for (int i = 0; i < loginTextboxs.Count; i++)
                    {
                        loginTextboxs[i].Draw(spriteBatch, backgroundTexture);
                    }

                    for (int i = 0; i < loginLabels.Count; i++)
                    {
                        loginLabels[i].Draw(spriteBatch);
                    }

                    for (int i = 0; i < loginButtons.Count; i++)
                    {
                        loginButtons[i].DrawButton(spriteBatch);
                    }

                    if (userTaken)
                    {
                        spriteBatch.DrawString(buttonFont, "The username enterned for the new account is invalid, please try again.", new Vector2(60, 670), Color.White);
                    }
                    else if (isAccountInvalid)
                    {
                        spriteBatch.DrawString(buttonFont, "The current information is invalid, please try again.", new Vector2(170, 670), Color.White);
                    }

                    spriteBatch.End();

                    break;
                case GameState.MainMenu:

                    spriteBatch.Begin();

                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    for (int i = 0; i < mainMenuButtons.Count; i++)
                    {
                        mainMenuButtons[i].DrawButton(spriteBatch);
                    }

                    spriteBatch.DrawString(buttonFont, "Current User: " + currentUser, currentUserPos, Color.White);

                    spriteBatch.End();

                    break;
                case GameState.Game:
                case GameState.MapCreator:

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());

                    //Draws all the tiles
                    foreach (Tile tile in tilesInView)
                    {
                        tile.Draw(spriteBatch);
                    }

                    if (gameState == GameState.Game)
                    {
                        player.Draw(spriteBatch, backgroundTexture);
                    }

                    foreach (Enemy enemy in enemiesInView)
                    {
                        enemy.Draw(spriteBatch, backgroundTexture);
                    }

                    foreach (Projectile projectile in projectiles)
                    {
                        projectile.Draw(spriteBatch);
                    }

                    spriteBatch.End();

                    if (gameState == GameState.MapCreator)
                    {
                        spriteBatch.Begin();

                        mapCreatorLabels[0].Draw(spriteBatch);

                        spriteBatch.End();
                    }

                    break;
                case GameState.Pause:
                    break;
                case GameState.MapCreatorHelp:

                    spriteBatch.Begin();

                    DrawCreatorHelp();

                    spriteBatch.End();

                    break;
                case GameState.StartMapCreator:

                    spriteBatch.Begin();

                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

                    for (int i = 0; i < mapCreatorStartLabel.Count - 1; i++)
                    {
                        mapCreatorStartLabel[i].Draw(spriteBatch);
                    }

                    for (int i = 0; i < startMapCreatorTextboxs.Count; i++)
                    {
                        startMapCreatorTextboxs[i].Draw(spriteBatch, backgroundTexture);
                    }

                    for (int i = 0; i < startMapCreatorButtons.Count; i++)
                    {
                        startMapCreatorButtons[i].DrawButton(spriteBatch);
                    }

                    if (invalidMapSize)
                    {
                        mapCreatorStartLabel[2].Draw(spriteBatch);
                    }

                    spriteBatch.End();

                    break;
            }

            base.Draw(gameTime);
        }


        //Pre:
        //Post:
        //Desc:
        private void NewGame()
        {
            for (int i = 0; i < gameTiles.GetLength(0); i++)
            {
                for (int j = 0; j < gameTiles.GetLength(1); j++)
                {
                    gameTiles[i, j] = new Tile(i, j, TileType.Blank);
                }
            }

            startTile = gameTiles[5, 4];

            gameTiles[5, 4].CurrentTileType = TileType.Spawn;
            gameTiles[5, 5] = new Tile(5, 5, TileType.Wall);
            gameTiles[5, 6] = new Tile(5, 6, TileType.Wall);
            gameTiles[5, 7] = new Tile(5, 7, TileType.Wall);
            gameTiles[6, 7] = new Tile(6, 7, TileType.Wall);
            gameTiles[7, 7] = new Tile(7, 7, TileType.Wall);
            gameTiles[6, 5] = new Tile(6, 5, TileType.Wall);
            gameTiles[7, 5] = new Tile(7, 5, TileType.Wall);

            player = new Player(startTile.PositionRect.X, startTile.PositionRect.Y, HUMAN_WIDTH, HUMAN_HEIGHT, fullScreenSize.Width, fullScreenSize.Height);
            enemies.Add(new Enemy(gameTiles, 6 * Tile.TILE_X_SIZE, 6 * Tile.TILE_Y_SIZE, HUMAN_WIDTH, HUMAN_HEIGHT, fullScreenSize.Width, fullScreenSize.Height));

            gameState = GameState.Game;
        }


        //Pre:
        //Post:
        //Desc:
        private void CreateMap()
        {
            gameTiles = new Tile[newRowAmount, newColumnAmount];

            for (int i = 0; i < gameTiles.GetLength(0); i++)
            {
                for (int j = 0; j < gameTiles.GetLength(1); j++)
                {
                    gameTiles[i, j] = new Tile(i, j, TileType.Blank);
                }
            }

            gameState = GameState.MapCreator;
            keyboardMouseInput.ResetPosition();
        }

        //Pre:
        //Post:
        //Desc:
        private void SetKeys()
        {
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
        }


        //Pre:
        //Post:
        //Desc:
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
            mapCreatorStartLabel.Add(new Label("Row Amount:", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(startMapCreatorTextboxs[0].GetCurrentRectangle().X -
                titleFont.MeasureString("Row Amount:").X - LABEL_TEXT_BOX_X, startMapCreatorTextboxs[0].GetCurrentRectangle().Y - (titleFont.MeasureString("Row Amount:").Y / 2)
                + (int)(startMapCreatorTextboxs[0].GetCurrentRectangle().Height / 2)), titleFont, Color.White));
            mapCreatorStartLabel.Add(new Label("Col. Amount:", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(startMapCreatorTextboxs[1].GetCurrentRectangle().X -
                titleFont.MeasureString("Col. Amount:").X - LABEL_TEXT_BOX_X, startMapCreatorTextboxs[1].GetCurrentRectangle().Y - (titleFont.MeasureString("Col. Amount:").Y / 2)
                + (int)(startMapCreatorTextboxs[1].GetCurrentRectangle().Height / 2)), titleFont, Color.White));
            mapCreatorStartLabel.Add(new Label("The Row and Column must be between 15 and 200 tiles.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(20, 20), buttonFont, Color.White));
            mapCreatorStartLabel.Add(new Label("The current map size is invalid, please try again.", SCREEN_WIDTH, SCREEN_HEIGHT, new Vector2(170, 670), buttonFont, Color.White));
        }


        //Pre:
        //Post:
        //Desc:
        private void CreateTextBoxs()
        {
            //Creates the textboxs for the login screen with positions that have it looks nice in the game
            loginTextboxs.Add(new TextBox(new Rectangle(450, 150, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT), SCREEN_WIDTH, SCREEN_HEIGHT, buttonFont, "USER HERE", Color.Red, Color.Yellow, Color.Black));
            loginTextboxs.Add(new TextBox(new Rectangle(450, 350, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT), SCREEN_WIDTH, SCREEN_HEIGHT, buttonFont, "PASS HERE", Color.Red, Color.Yellow, Color.Black));

            //Creates the textboxs for the map creator screen size choosing menu
            startMapCreatorTextboxs.Add(new TextBox(new Rectangle(450, 150, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT), SCREEN_WIDTH, SCREEN_HEIGHT, buttonFont, "ROW NUM", Color.Red, Color.Yellow, Color.Black));
            startMapCreatorTextboxs.Add(new TextBox(new Rectangle(450, 350, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT), SCREEN_WIDTH, SCREEN_HEIGHT, buttonFont, "COLUMN NUM", Color.Red, Color.Yellow, Color.Black));
        }


        //Pre:
        //Post:
        //Desc:
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
            mainMenuButtons.Add(new Button(new Rectangle(370, 110, 284, 70), new Rectangle(362, 100, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Play Game", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.ChooseMap));
            mainMenuButtons.Add(new Button(new Rectangle(370, 230, 284, 70), new Rectangle(362, 220, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Create Map", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.StartMapCreator));
            mainMenuButtons.Add(new Button(new Rectangle(370, 350, 284, 70), new Rectangle(362, 340, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Instructions", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.Instructions));
            mainMenuButtons.Add(new Button(new Rectangle(370, 470, 284, 70), new Rectangle(362, 460, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
                "Sign Out", buttonFont, enlargedButtonFont, Color.White, Color.Yellow, GameState.SignOut));
            mainMenuButtons.Add(new Button(new Rectangle(370, 590, 284, 70), new Rectangle(362, 580, 300, 90), new Vector2(20, 20), Color.Black, Color.Red,
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

            for (int i = 0; i < startMapCreatorButtons.Count; i++)
            {
                startMapCreatorButtons[i].SetTextureData(graphics);
            }
        }


        //Pre:
        //Post:
        //Desc:
        private void CheckBulletCollision()
        {
            returnObjects.Clear();
            quadTree.Retrieve(returnObjects, player);

            returnObjects = returnObjects.Distinct().ToList();

            if (returnObjects.Count > 0)
            {
                for (int i = 0; i < returnObjects.Count; i++)
                {
                    if (!returnObjects[i].NeedDestroy && !returnObjects[i].PlayerBullet && CollisionDetection.Collision(player.GetBounds(),
                        returnObjects[i].GetBounds(), player.GetCurrentTexture(), returnObjects[i].GetTexture()))
                    {
                        player.Health -= projectiles[i].Damage;
                        returnObjects[i].NeedDestroy = true;
                    }
                }
            }

            foreach (Enemy enemy in enemies)
            {
                returnObjects.Clear();
                quadTree.Retrieve(returnObjects, enemy);

                returnObjects = returnObjects.Distinct().ToList();

                if (returnObjects.Count > 0)
                {
                    for (int i = 0; i < returnObjects.Count; i++)
                    {
                        if (!returnObjects[i].NeedDestroy && CollisionDetection.Collision(enemy.GetBounds(), returnObjects[i].GetBounds(), enemy.GetCurrentTexture(),
                            returnObjects[i].GetTexture()))
                        {
                            if (returnObjects[i].PlayerBullet)
                            {
                                enemy.Health -= projectiles[i].Damage;
                                returnObjects[i].NeedDestroy = true;
                            }
                            else
                            {
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

            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].NeedDestroy)
                {
                    projectiles[i] = null;
                    projectiles.RemoveAt(i);
                    i--;
                }
            }
        }


        //Pre:
        //Post:
        //Desc:
        private void DrawCreatorHelp()
        {
            spriteBatch.Draw(backgroundTexture, backgroundRect, Color.Black);

            for (int i = 1; i < mapCreatorLabels.Count; i++)
            {
                mapCreatorLabels[i].Draw(spriteBatch);
            }
        }
    }
}
