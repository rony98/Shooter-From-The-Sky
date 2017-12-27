/*
 * Author: Rony Verch
 * File Name: Tile.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 20, 2015
 * Modified Date: January 20, 2015
 * Description: A class for the tiles that is used to draw the entire map and know where obstacles are for the player and enemies.
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
    class Tile
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Constants for the X and Y size of the tile
        public const int TILE_X_SIZE = 64;
        public const int TILE_Y_SIZE = 64;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Properties for the row and column of the tile
        public int Row { get; private set; }
        public int Column { get; private set; }

        //The position of the tile
        public Rectangle PositionRect { get; private set; }

        //Property for the tile type of this tile
        public TileType CurrentTileType { get; set; }

        //Static textures that the tiles could be represented by
        static private Texture2D wallTexture;
        static private Texture2D regTileTexture;
        static private Texture2D spawnTileTexture;
        static private Texture2D saveTileTexture;

        //Array for the lines that are needed for pathfinding
        public RaycastLine[] Lines { get; private set; }

        //Properties for whether there is an enemy on this tile and what the enemy is (used for creating maps)
        public bool EnemyExists { get; set; }
        public Enemy EnemyPlaced { get; set; }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: Row, Column, and the tile type for this tile
        //Post: The Tile is created using the row, column, and tile type
        //Desc: A constructor for the current tile 
        public Tile(int row, int column, TileType tileType)
        {
            //Sets the current row, column, and tile type
            Row = row;
            Column = column;
            CurrentTileType = tileType;

            //Calculates the X and Y position of the current tile
            CalcPostion();

            //If the current tile is a wall tile
            if (CurrentTileType == TileType.Wall)
            {
                //Sets four index's to the lines array representing four lines to the tile
                Lines = new RaycastLine[4];

                //Sets the lines for raycasting
                Lines[0] = new RaycastLine(true, false, new Vector2(PositionRect.X, PositionRect.Y),
                    new Vector2(PositionRect.X, PositionRect.Y + PositionRect.Height));
                Lines[1] = new RaycastLine(true, false, new Vector2(PositionRect.X + PositionRect.Width, PositionRect.Y),
                    new Vector2(PositionRect.X + PositionRect.Width, PositionRect.Y + PositionRect.Height));
                Lines[2] = new RaycastLine(false, true, new Vector2(PositionRect.X, PositionRect.Y),
                    new Vector2(PositionRect.X + PositionRect.Width, PositionRect.Y));
                Lines[3] = new RaycastLine(false, true, new Vector2(PositionRect.X, PositionRect.Y + PositionRect.Height),
                    new Vector2(PositionRect.X + PositionRect.Width, PositionRect.Y + PositionRect.Height));
            }

            //Sets the enemy to unexistant (for map creation)
            EnemyExists = false;
            EnemyPlaced = null;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: None
        //Post: The position of the current tile is calculated
        //Desc: A method which calculates the position of the current tile
        public void CalcPostion()
        {
            //Calculates and set the position
            PositionRect = new Rectangle(TILE_X_SIZE * Row, TILE_Y_SIZE * Column, TILE_X_SIZE, TILE_Y_SIZE);
        }


        //Pre: None
        //Post: The Tile is drawn using the correct texture
        //Desc: A method which draws the tile
        public void Draw(SpriteBatch sb)
        {
            //Switch statement for the current tile to draw
            switch (CurrentTileType)
            {
                //If the tile is a regular/blank tile, draw it
                case TileType.Blank:
                    sb.Draw(regTileTexture, PositionRect, Color.White);
                    break;
                //If the tile is a wall, draw it
                case TileType.Wall:
                    sb.Draw(wallTexture, PositionRect, Color.White);
                    break;
                //If the tile is the spawn tile, draw it
                case TileType.Spawn:
                    sb.Draw(spawnTileTexture, PositionRect, Color.White);
                    break;
                //If the tile is a tile from which the game can be saved, draw it
                case TileType.Save:
                    sb.Draw(saveTileTexture, PositionRect, Color.White);
                    break;
                default:
                    break;
            }
        }


        //Pre: The Content Manager used to load the textures
        //Post: The texture data is set
        //Desc: A static method which sets the texture data for the tile textures that will be used to draw all the tiles in the game
        public static void SetTextureData(ContentManager cm)
        {
            //Loads all the static textures
            wallTexture = cm.Load<Texture2D>("Tiles\\Wall Tile");
            regTileTexture = cm.Load<Texture2D>("Tiles\\Reg Tile");
            spawnTileTexture = cm.Load<Texture2D>("Tiles\\Spawn Tile");
            saveTileTexture = cm.Load<Texture2D>("Tiles\\Save Game Tile");
        }
    }
}
