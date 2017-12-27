/*
 * Author: Rony Verch
 * File Name: HighscoresHelper.cs
 * Project Name: Shooter From The Sky
 * Creation Date: January 6, 2015
 * Modified Date: January 20, 2015
 * Description: A helper class to save high scores data onto an online server.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class HighscoresHelper
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //The suffix's and file type for the highscores files
        public const string FILE_NAME_SUFFIX = "_Highscores.txt";
        public const string FILE_PATH_SUFFIX = "_Highscores.txt";
        public const string FILE_TYPE = "TXT";
        public const string FILE_DESCRIPTION = "Map Highscores";

        //The max amount of scores that are saved for one map
        public const int MAX_SCORES = 10;


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Variable for the online helper 
        private OnlineStorageHelper onlineHelper;

        //Variables for the stream reader and writer
        private StreamReader reader;
        private StreamWriter writer;

        //Lists for the user names and the times for each user
        public List<string> UserNames { get; set; }
        public List<float> UserTimes { get; set; }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The online helper object
        //Post: None
        //Desc: A constructor for the highscores helper
        public HighscoresHelper(OnlineStorageHelper onlineHelper)
        {
            //Sets the online helper
            this.onlineHelper = onlineHelper;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The map name
        //Post: The map is downloaded
        //Desc: A method for downloading the map
        public void DownloadHighscoresMap(string map)
        {
            //Resets the user names and user times lists
            UserNames = new List<string>();
            UserTimes = new List<float>();

            //If the file was able to be downloaded
            if (onlineHelper.DownloadFile(map + FILE_PATH_SUFFIX, map + FILE_NAME_SUFFIX))
            {
                //Creates a strean reader with the map path
                reader = new StreamReader(map + FILE_PATH_SUFFIX);

                //Gets the score count from the file
                int scoreCount = Convert.ToInt32(reader.ReadLine());

                //For each score that is saved
                for (int i = 0; i < scoreCount; i++)
                {
                    //Reads the user name and the time for the current user
                    UserNames.Add(reader.ReadLine());
                    UserTimes.Add((float)Convert.ToDouble(reader.ReadLine()));
                }

                //Closes the stream reader
                reader.Close();
            }
        }


        //Pre: The name of the new map
        //Post: The highscores file for the new map is added
        //Desc: A method for adding a new map
        public void AddHighscoresMap(string newMap)
        {
            //Creates a new stream writer with the path of the new map
            writer = new StreamWriter(newMap + FILE_PATH_SUFFIX);

            //Writes a 0 in the file for no scores
            writer.WriteLine(0);

            //Closes the stream writer
            writer.Close();

            //Uploads the new file to the online server
            onlineHelper.UpdateFile(newMap + FILE_NAME_SUFFIX, newMap + FILE_PATH_SUFFIX, FILE_TYPE, FILE_DESCRIPTION);
        }


        //Pre: The map name, the user name, and the elapsed time of the user
        //Post: A boolean for whether the highscore was updated
        //Desc: A method for updating the highscores of a map
        public bool UpdateHighscoresMap(string mapName, string user, float elapsedTime)
        {
            //Temporarily used lists for the names and times
            List<string> tempNames = new List<string>();
            List<float> tempTimes = new List<float>();

            //If the list of usernames is not null
            if (UserNames != null)
            {
                //Clears the usernames and times for each user
                UserNames.Clear();
                UserTimes.Clear();
            }

            //Checks if the file exists
            if (onlineHelper.CheckFileExists(mapName + FILE_NAME_SUFFIX))
            {
                //Downloads the file
                onlineHelper.DownloadFile(mapName + FILE_PATH_SUFFIX, mapName + FILE_NAME_SUFFIX);

                //Creates a stream reader for the file
                reader = new StreamReader(mapName + FILE_PATH_SUFFIX);

                //Gets the score count using the reader from the file
                int scoreCount = Convert.ToInt32(reader.ReadLine());

                //Loop for every score saved in the file
                for (int i = 0; i < scoreCount; i++)
                {
                    //Adds the neames and times to the lists from the file
                    tempNames.Add(reader.ReadLine());
                    tempTimes.Add((float)Convert.ToDouble(reader.ReadLine()));
                }

                //Closes the stream reader
                reader.Close();

                //Adds the new user and their time
                tempNames.Add(user);
                tempTimes.Add(elapsedTime);

                //Sorts the new times
                SortTimes(tempNames, tempTimes);

                //If the amount of scores saved is greater then the max amount of scores
                if (UserNames.Count > MAX_SCORES)
                {
                    //Removes the last score
                    UserNames.RemoveAt(UserNames.Count - 1);
                    UserTimes.RemoveAt(UserTimes.Count - 1);
                }

                //Creates the stream writer with the file path
                writer = new StreamWriter(mapName + FILE_PATH_SUFFIX);

                //Writes the amount of scores to the file
                writer.WriteLine(UserNames.Count);

                //Loop for every score
                for (int i = 0; i < UserNames.Count; i++)
                {
                    //Writers the user name and it's time to the file
                    writer.WriteLine(UserNames[i]);
                    writer.WriteLine(UserTimes[i]);
                }

                //Closes the file
                writer.Close();

                //If the file was updates
                if (onlineHelper.UpdateFile(mapName + FILE_NAME_SUFFIX, mapName + FILE_PATH_SUFFIX, FILE_TYPE, FILE_DESCRIPTION))
                {
                    //Returns that the file was updateed
                    return true;
                }
            }
            //If the file does not exist
            else
            {
                //Resets the names and the times for each name
                UserNames = new List<string>();
                UserTimes = new List<float>();

                //Adds the new user and their time
                UserNames.Add(user);
                UserTimes.Add(elapsedTime);

                //A stream writer is created with the path of the current map
                writer = new StreamWriter(mapName + FILE_PATH_SUFFIX);

                //Writes the amount of scores in the file
                writer.WriteLine(UserNames.Count);

                //Writes the new user name and time to the file
                writer.WriteLine(UserNames[0]);
                writer.WriteLine(UserTimes[0]);

                //Closes the stream writer
                writer.Close();

                //If the file was uploaded
                if (onlineHelper.UpdateFile(mapName + FILE_NAME_SUFFIX, mapName + FILE_PATH_SUFFIX, FILE_TYPE, FILE_DESCRIPTION))
                {
                    //Returns that the file was uploaded
                    return true;
                }
            }

            //Returns that the file was not uploaded
            return false;
        }


        //Pre: The list of names and their times
        //Post: None
        //Desc: A method for sorting the scores for all the users
        private void SortTimes(List<string> names, List<float> times)
        {
            //Temp float and string variables for sorting the scores
            float tempFloat;
            string tempString;

            //Loop for every time
            for (int i = 0; i < times.Count; i++)
            {
                //Loop for every time that wasn't sorted yet
                for (int j = 0; j < times.Count - i - 1; j++)
                {
                    //If the current time is greater then the next
                    if (times[j] > times[j + 1])
                    {
                        //Swaps the location of the current time and the next
                        tempFloat = times[j + 1];
                        tempString = names[j + 1];

                        times[j + 1] = times[j];
                        names[j + 1] = names[j];

                        times[j] = tempFloat;
                        names[j] = tempString;
                    }
                }
            }

            //Sets the names and times
            UserNames = names;
            UserTimes = times;
        }
    }
}
