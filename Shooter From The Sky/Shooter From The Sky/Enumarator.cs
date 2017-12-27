/*
 * Author: Rony Verch
 * File Name: Enumarator.cs
 * Project Name: Shooter From The Sky
 * Creation Date: December 20, 2015
 * Modified Date: January 20, 2015
 * Description: A class that stores enumarator's which can be used through the entire game
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    /////////////////////////////////////// Enumarator \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    //Enumarator for the tile type
    public enum TileType
    {
        Blank,
        Wall,
        Spawn,
        Save,
    };

    //Enumarator for the animation state for players/enemies for all weapons except knife
    public enum AnimationState
    {
        Idle,
        Move,
        Reload,
        Shoot,
    };

    //Enumarator for the different game states the came could be in
    public enum GameState
    {
        GameLogin,
        AccountCreator,
        MainMenu,
        Game,
        ChooseMap,
        LoadSaveChoice,
        Pause,
        StartMapCreator,
        MapCreatorHelp,
        MapCreator, 
        SaveCreatorMap,
        ExitGame,
        Instructions,
        SignOut,
        Highscores,
        ProfileStats,
        SaveGame,
        NewGame,
        LoadGame,
        GameOver,
    };

    //Enumarator for the different weapon types
    public enum WeaponTypes
    {
        Rifle,
        Handgun,
        Shotgun,
        ThrowingKnife,
    };

    //Enumarator for the different quadrants that one object can be relative to the other
    public enum Quadrant
    {
        NorthEast,
        SouthEast,
        SouthWest,
        NorthWest,
        None,
    }

    //Enumarator for the different types of projectile
    public enum ProjectileType
    {
        Knife,
        Bullet,
    }
}
