/*
 * Author: Rony Verch
 * File Name: MapSaveLoadHelper.cs
 * Project Name: Shooter From The Sky
 * Creation Date: January 6, 2015
 * Modified Date: January 20, 2015
 * Description: A helper class to save maps onto an online server and load the maps from the server to be able to play the game.
 */

using com.shephertz.app42.paas.sdk.csharp.upload;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class MapSaveLoadHelper
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        //The suffix's for the new map files
        public const string NEW_MAP_SUFFIX = "_NewMap.txt";
        public const string NEW_MAP_PATH_SUFFIX = "_NewMap.txt";
        public const string NEW_MAP_FILE_DESCRIPTION = "New Map";

        //The suffix's for the saved map files
        public const string SAVE_MAP_SUFFIX = "_SavedMap.txt";
        public const string SAVE_MAP_PATH_SUFFIX = "_SavedMap.txt";
        public const string SAVE_MAP_FILE_DESCRIPTION = "Saved Map";

        //The suffix's for the all maps file
        public const string ALL_MAPS_FILE_NAME = "AllMaps.txt";
        public const string ALL_MAPS_FILE_PATH = "AllMaps.txt";
        public const string ALL_MAPS_FILE_DESCRIPTION = "A txt file with the names of every map that is available for the user";

        //The file type for all the files
        public const string FILE_TYPE = "TXT";


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Online helper and high score helper objects
        private OnlineStorageHelper onlineHelper;
        private HighscoresHelper highscoreHelper;

        //Stream read and writer
        private StreamReader reader;
        private StreamWriter writer;

        //Human width and height
        private int humanWidth;
        private int humanHeight;

        //Properties needed to store data that was written in a file so other classes can access loaded data
        public Tile[,] GameTiles { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public Tile StartTile { get; set; }
        public List<Projectile> Projectiles { get; set; }
        public Player CurrentPlayer { get; set; }
        public float PreviouslyElapsedTime { get; set; }
        public int EnemiesPreviouslyKilled { get; set; }

        //List for all the map names which can be displayed
        private List<string> allMapNames = new List<string>();


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The online helper, the highscore helper, the human width and height
        //Post: None
        //Desc: A constructor for the map save and load helper
        public MapSaveLoadHelper(OnlineStorageHelper onlineHelper, HighscoresHelper highscoreHelper, int humanWidth, int humanHeight)
        {
            //Sets all the data for the map save and load helper
            this.onlineHelper = onlineHelper;
            this.highscoreHelper = highscoreHelper;
            this.humanWidth = humanWidth;
            this.humanHeight = humanHeight;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The map name, the game tiles, the list of enemies, and the start tile
        //Post: A boolean for whether the new map was saved
        //Desc: A method for saving a new map
        public bool SaveNewMap(string mapName, Tile[,] gameTiles, List<Enemy> enemies, Tile startTile)
        {
            //Creates a stream writer with the path of the new map
            writer = new StreamWriter(mapName + NEW_MAP_PATH_SUFFIX);

            //Writes the lengths of each dimesnion of the gametiles array
            writer.WriteLine(gameTiles.GetLength(0));
            writer.WriteLine(gameTiles.GetLength(1));

            //Loop for the index's of the first dimension
            for (int i = 0; i < gameTiles.GetLength(0); i++)
            {
                //Loop for the index's of the second dimension
                for (int j = 0; j < gameTiles.GetLength(1); j++)
                {
                    //Writes the tile type for the current tile
                    writer.WriteLine(gameTiles[i, j].CurrentTileType);
                }
            }

            //Writes the amount of enemies
            writer.WriteLine(enemies.Count);

            //Loop for every enemy
            for (int i = 0; i < enemies.Count; i++)
            {
                //Writes the position of the current enemy
                writer.WriteLine(enemies[i].Position.X);
                writer.WriteLine(enemies[i].Position.Y);
            }

            //Writes the locaiton of the start tile
            writer.WriteLine(startTile.Row);
            writer.WriteLine(startTile.Column);

            //Closes the stream writer
            writer.Close();

            //If the all maps file exists and can be read
            if (ReadAllMaps())
            {
                //Adds a map to the list of maps
                allMapNames.Add(mapName);
            }
            //If the all maps file does not exist or can't be read
            else
            {
                //Clears what's currently in the list of maps and adds the new map
                allMapNames.Clear();
                allMapNames.Add(mapName);
            }

            //Writes the new set of maps to a file
            WriteAllMaps();

            //If the file with all the map names is uploaded onto the server
            if (onlineHelper.UpdateFile(ALL_MAPS_FILE_NAME, ALL_MAPS_FILE_PATH, FILE_TYPE, ALL_MAPS_FILE_DESCRIPTION))
            {
                //The new map that was created is uploaded onto the server and a highscores file for the map is created
                onlineHelper.UpdateFile(mapName + NEW_MAP_SUFFIX, mapName + NEW_MAP_PATH_SUFFIX, FILE_TYPE, NEW_MAP_FILE_DESCRIPTION);
                highscoreHelper.AddHighscoresMap(mapName);

                //True is returned which represents that the map was saved
                return true;
            }

            //False is returned which means that the map was not saved
            return false;
        }


        //Pre: The map name
        //Post: None
        //Desc: A new map is loaded to be played by the user
        public void LoadNewMap(string mapName)
        {
            //Creates temp data to store everything that is loaded from the server
            Tile[,] tempGameTiles;
            List<Enemy> tempEnemyList = new List<Enemy>();
            Tile tempStartTile;

            //Downloads the map that is wanted to be played from the server
            onlineHelper.DownloadFile(mapName + NEW_MAP_PATH_SUFFIX, mapName + NEW_MAP_SUFFIX);

            //A stream reader is created with the path of the new map
            reader = new StreamReader(mapName + NEW_MAP_PATH_SUFFIX);

            //Creates the 2D array of game tiles using the sizes from the file
            tempGameTiles = new Tile[Convert.ToInt32(reader.ReadLine()), Convert.ToInt32(reader.ReadLine())];

            //Loop for every value for the first dimension of the array
            for (int i = 0; i < tempGameTiles.GetLength(0); i++)
            {
                //Loop for every value of the seocnd dimension of the array
                for (int j = 0; j < tempGameTiles.GetLength(1); j++)
                {
                    //Sets the current tile using the values of the loop and the tile type that is read from the file
                    tempGameTiles[i, j] = new Tile(i, j, (TileType)Enum.Parse(typeof(TileType), reader.ReadLine()));
                }
            }

            //Gets the enemy count from the file
            int enemyCount = Convert.ToInt32(reader.ReadLine());

            //Calculates the screen width and height
            int screenWidth = Tile.TILE_X_SIZE * tempGameTiles.GetLength(0);
            int screenHeight = Tile.TILE_Y_SIZE * tempGameTiles.GetLength(1);

            //Loop for every enemy
            for (int i = 0; i < enemyCount; i++)
            {
                //Adds the current enemy using data that is read from the file
                tempEnemyList.Add(new Enemy(tempGameTiles, Convert.ToInt32(reader.ReadLine()), Convert.ToInt32(reader.ReadLine()), humanWidth, humanHeight, screenWidth, screenHeight));
            }

            //Reads the location of the start tile from the file and sets it
            tempStartTile = tempGameTiles[Convert.ToInt32(reader.ReadLine()), Convert.ToInt32(reader.ReadLine())];

            //Closes the stream reader
            reader.Close();

            //Sets the game tiles, enemies, and start tile to the properties which can be accessed by other classes which load the game
            GameTiles = tempGameTiles;
            Enemies = tempEnemyList;
            StartTile = tempStartTile;
        }


        //Pre: The map name and the current user
        //Post: A boolean for whether the map save exists
        //Desc: A method for checking whether a save of a map exists
        public bool CheckSaveExists(string mapName, string currentUser)
        {
            //If the current map exists
            if (onlineHelper.CheckFileExists(currentUser + "_" + mapName + SAVE_MAP_SUFFIX))
            {
                //Returns that the map exists
                return true;
            }

            //Returns that the map does not exist
            return false;
        }


        //Pre: The map name, the current user, the game tiles, the enemies, the player, all the projectiles, the amount of time that elapsed this far into the game, the amount of enemies killed
        //Post: A boolean for whether the map was saved
        //Desc: A method for saving the current map
        public bool SaveCurrentMap(string mapName, string currentUser, Tile[,] gameTiles, List<Enemy> enemies, Player player, List<Projectile> projectiles, float elapsedTime, int enemiesKilled)
        {
            //Creates the stream writer with the path of the current map save
            writer = new StreamWriter(currentUser + "_" + mapName + SAVE_MAP_PATH_SUFFIX);

            //Writes teh length of the game tiles array dimensions
            writer.WriteLine(gameTiles.GetLength(0));
            writer.WriteLine(gameTiles.GetLength(1));

            //Loop for every value of the first dimension of the gametiles 2D array
            for (int i = 0; i < gameTiles.GetLength(0); i++)
            {
                //Loop for every value of the second dimension of the gametiles 2D array
                for (int j = 0; j < gameTiles.GetLength(1); j++)
                {
                    //Writes the current tile type
                    writer.WriteLine(gameTiles[i, j].CurrentTileType);
                }
            }

            //Writes the amount of enemies to the file
            writer.WriteLine(enemies.Count);

            //Loop for every enemy
            for (int i = 0; i < enemies.Count; i++)
            {
                //The enemies position and health is written to the file
                writer.WriteLine(enemies[i].Position.X);
                writer.WriteLine(enemies[i].Position.Y);
                writer.WriteLine(enemies[i].Health);
            }

            //Writes the amount of projectiles
            writer.WriteLine(projectiles.Count);

            //Loop for every projectile
            for (int i = 0; i < projectiles.Count; i++)
            {
                //The projectiles values are written to the file
                writer.WriteLine(projectiles[i].GetBounds().X);
                writer.WriteLine(projectiles[i].GetBounds().Y);
                writer.WriteLine(projectiles[i].GetProjectileType());
                writer.WriteLine(projectiles[i].Damage);
                writer.WriteLine(projectiles[i].Speed);
                writer.WriteLine(projectiles[i].GetRotationAngle());
                writer.WriteLine(projectiles[i].NeedDestroy);
                writer.WriteLine(projectiles[i].PlayerBullet);

                //If the projectile is not a player's projectile
                if (!projectiles[i].PlayerBullet)
                {
                    //Loop for every enemy
                    for (int j = 0; j < enemies.Count; j++)
                    {
                        //If the bullet owner of the current projectile is the current enemy
                        if (projectiles[i].BulletOwner == enemies[j])
                        {
                            //The bullet owner's index is saved to the file and the for loop is stopped
                            writer.WriteLine(j);
                            j = enemies.Count;
                        }
                    }
                }
            }

            //Writes the player's stats to the file
            writer.WriteLine(player.Position.X);
            writer.WriteLine(player.Position.Y);
            writer.WriteLine(player.Health);

            //Writes the elapsed time of the game to the file and the amount of enemies killed
            writer.WriteLine(elapsedTime);
            writer.WriteLine(enemiesKilled);

            //Closes the stream writer
            writer.Close();

            //If the file was updated on the server
            if (onlineHelper.UpdateFile(currentUser + "_" + mapName + SAVE_MAP_SUFFIX, currentUser + "_" + mapName + SAVE_MAP_PATH_SUFFIX, FILE_TYPE, SAVE_MAP_FILE_DESCRIPTION))
            {
                //Returns that the map was saved
                return true;
            }

            //Returns that the map was not saved
            return false;
        }


        //Pre: The map name and the current user
        //Post: None
        //Desc: A method for loading the current map
        public void LoadCurrentMap(string mapName, string currentUser)
        {
            //Creates temp data to store everything that is loaded from the server
            Tile[,] tempGameTiles;
            List<Enemy> tempEnemyList = new List<Enemy>();
            Player tempPlayer;
            List<Projectile> tempProjectiles = new List<Projectile>();

            //Downloads the current map from the server to be loaded
            onlineHelper.DownloadFile(currentUser + "_ " + mapName + SAVE_MAP_PATH_SUFFIX, mapName + SAVE_MAP_SUFFIX);

            //Creates a new stream reader with the path of the map
            reader = new StreamReader(currentUser + "_" + mapName + SAVE_MAP_PATH_SUFFIX);

            //Creates the game tiles using the lengths that are saved in the file
            tempGameTiles = new Tile[Convert.ToInt32(reader.ReadLine()), Convert.ToInt32(reader.ReadLine())];

            //Loop for every value in the first dimension
            for (int i = 0; i < tempGameTiles.GetLength(0); i++)
            {
                //Loop for every value in the second dimension
                for (int j = 0; j < tempGameTiles.GetLength(1); j++)
                {
                    //Sets the current tile with the tile type from the file
                    tempGameTiles[i, j] = new Tile(i, j, (TileType)Enum.Parse(typeof(TileType), reader.ReadLine()));
                }
            }

            //Sets the enemy count
            int enemyCount = Convert.ToInt32(reader.ReadLine());

            //Sets the screen width and height
            int screenWidth = Tile.TILE_X_SIZE * tempGameTiles.GetLength(0);
            int screenHeight = Tile.TILE_Y_SIZE * tempGameTiles.GetLength(1);

            //Loop for every enemy
            for (int i = 0; i < enemyCount; i++)
            {
                //Adds the enemy using the data from the file and sets the enemies health
                wtempEnemyList.Add(new Enemy(tempGameTiles, Convert.ToInt32(reader.ReadLine()), Convert.ToInt32(reader.ReadLine()), humanWidth, humanHeight, screenWidth, screenHeight));
                tempEnemyList[i].Health = Convert.ToInt32(reader.ReadLine());
            }

            //Sets the projectile count
            int projectileCount = Convert.ToInt32(reader.ReadLine());

            //Loop for every projectile
            for (int i = 0; i < projectileCount; i++)
            {
                //Adds the projectile using values from the file that are read
                tempProjectiles.Add(new Projectile(new Vector2((float)Convert.ToDouble(reader.ReadLine()), (float)Convert.ToDouble(reader.ReadLine())),
                    (ProjectileType)Enum.Parse(typeof(ProjectileType), reader.ReadLine()), Convert.ToInt32(reader.ReadLine()), (float)Convert.ToDouble(reader.ReadLine()),
                    (float)Convert.ToDouble(reader.ReadLine()), screenWidth, screenHeight));

                //Sets properties of the projectile using values from the file that are read
                tempProjectiles[i].NeedDestroy = Convert.ToBoolean(reader.ReadLine());
                tempProjectiles[i].PlayerBullet = Convert.ToBoolean(reader.ReadLine());

                //If the projectile is not a player bullet
                if (!tempProjectiles[i].PlayerBullet)
                {
                    //Sets the bullet owner of the projectile using the index that is read from the file
                    tempProjectiles[i].BulletOwner = tempEnemyList[Convert.ToInt32(reader.ReadLine())];
                }
            }

            //Creates the player using data from the file and sets its health
            tempPlayer = new Player((int)Convert.ToDouble(reader.ReadLine()), (int)Convert.ToDouble(reader.ReadLine()), humanWidth, humanHeight, screenWidth, screenHeight);
            tempPlayer.Health = Convert.ToInt32(reader.ReadLine());

            //Stores the previously elapsed time and the amount of enemies previously killed
            PreviouslyElapsedTime = (float)Convert.ToDouble(reader.ReadLine());
            EnemiesPreviouslyKilled = Convert.ToInt32(reader.ReadLine());

            //Closes teh stream reader
            reader.Close();

            //Sets all the data from the file to the properties that can be accessed by other classes
            GameTiles = tempGameTiles;
            Enemies = tempEnemyList;
            CurrentPlayer = tempPlayer;
            Projectiles = tempProjectiles;
        }


        //Pre: The map name
        //Post: A boolean for whether the map exists
        //Desc: A method for checking whether a map exists
        public bool CheckMapExists(string mapName)
        {
            //If the file with all the map names is read
            if (ReadAllMaps())
            {
                //Loop through all the maps
                for (int i = 0; i < allMapNames.Count; i++)
                {
                    //If the map exists
                    if (allMapNames[i].ToLower() == mapName.ToLower())
                    {
                        //Returns that the map exists
                        return true;
                    }
                }
            }

            //Returns that the map doesn't exist
            return false;
        }


        //Pre: None
        //Post: Boolean for whether the file with all the maps was read
        //Desc: A method for reading the file with all the map names
        public bool ReadAllMaps()
        {
            //If the file with all the map names was downloaded
            if (onlineHelper.DownloadFile(ALL_MAPS_FILE_PATH, ALL_MAPS_FILE_NAME))
            {
                //A stream reader is created to read the file
                reader = new StreamReader(ALL_MAPS_FILE_PATH);

                //The old map names are cleared
                allMapNames.Clear();

                //While there is still something in the file to read
                while (!reader.EndOfStream)
                {
                    //The next line of the file is read and added to all the map names list
                    allMapNames.Add(reader.ReadLine());
                }

                //The stream reader is closed
                reader.Close();

                //True is returned representing that the maps were read
                return true;
            }

            //False is returned representing that the maps were not read
            return false;
        }


        //Pre: None
        //Post: The file with all the map names is written
        //Desc: A method for writing the file with all the map names
        public void WriteAllMaps()
        {
            //A stream writer is created for the file with all the map names
            writer = new StreamWriter(ALL_MAPS_FILE_PATH);

            //Loop for every map name
            for (int i = 0; i < allMapNames.Count; i++)
            {
                //The current map name is written to the file
                writer.WriteLine(allMapNames[i]);
            }

            //The stream writer for all the map names is closed
            writer.Close();
        }


        //Pre: None
        //Post: The list with all the map names is returned
        //Desc: A method for getting all the map names 
        public List<string> GetAllMaps()
        {

            //Returns the list with all the map names
            return allMapNames;
        }
    }
}
