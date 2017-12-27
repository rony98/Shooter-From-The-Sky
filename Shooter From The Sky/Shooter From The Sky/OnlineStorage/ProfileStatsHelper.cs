/*
 * Author: Rony Verch
 * File Name: ProfileStatsHelper.cs
 * Project Name: Shooter From The Sky
 * Creation Date: January 6, 2015
 * Modified Date: January 20, 2015
 * Description: A helper class to save profile statistics data onto an online server.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class ProfileStatsHelper
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //The suffix's and file type for the profile stats files
        public const string FILE_NAME_SUFFIX = "_Stats.txt";
        public const string FILE_PATH_SUFFIX = "_Stats.txt";
        public const string FILE_TYPE = "TXT";
        public const string FILE_DESCRIPTION = "Profile Stats";


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Variable for the online helper 
        private OnlineStorageHelper onlineHelper;

        //Properties for the amount of enemies killed, the amount of health lost, and the amount of deaths a user encountered
        public int EnemiesKilled { get; set; }
        public int HealthLost { get; set; }
        public int DeathAmount { get; set; }
        public float TotalGameTime { get; set; }
        public int BulletsShot { get; set; }

        //Variables for the stream reader and writer
        private StreamReader reader;
        private StreamWriter writer;


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The online helper
        //Post: The profile stats helper is created
        //Desc: A constructor for creating the profile stats helper
        public ProfileStatsHelper(OnlineStorageHelper onlineHelper)
        {
            //Sets the online helper
            this.onlineHelper = onlineHelper;
        }


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// METHODS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The user profile
        //Post: The profile stats file is added
        //Desc: A method for adding the profile stats file
        public void AddProfileStats(string userProfile)
        {
            //A stream writer is created to writer data to the file
            writer = new StreamWriter(userProfile + FILE_PATH_SUFFIX);

            //A 0 is written for every value that needs to be written because the stats were just created 
            writer.WriteLine(0);
            writer.WriteLine(0);
            writer.WriteLine(0);
            writer.WriteLine(0);
            writer.WriteLine(0);

            //The stream writer is closed
            writer.Close();

            //The file is uploaded online
            onlineHelper.UpdateFile(userProfile + FILE_NAME_SUFFIX, userProfile + FILE_PATH_SUFFIX, FILE_TYPE, FILE_DESCRIPTION);
        }


        //Pre: The user profile, the amount of enemies killed, the amount of health lost, whether the player died again, the amount of extra game time, and the amount of projecitles shot
        //Post: A boolean for whether the profile stats were updated
        //Desc: A method for updating the profile stats for a specific profile
        public bool UpdateProfileStats(string userProfile, int extraEnemiesKilled, int extraHealthLost, int extraDeath, float extraGameTime, int extraBulletsShot)
        {
            //If the file for the profile stats exists
            if (onlineHelper.CheckFileExists(userProfile + FILE_NAME_SUFFIX))
            {
                //If the file is downloaded
                if (onlineHelper.DownloadFile(userProfile + FILE_PATH_SUFFIX, userProfile + FILE_NAME_SUFFIX))
                {
                    //A stream reader is created to read the file
                    reader = new StreamReader(userProfile + FILE_PATH_SUFFIX);

                    //All the data from the file is read
                    EnemiesKilled = Convert.ToInt32(reader.ReadLine());
                    HealthLost = Convert.ToInt32(reader.ReadLine());
                    DeathAmount = Convert.ToInt32(reader.ReadLine());
                    TotalGameTime = (float)Convert.ToDouble(reader.ReadLine());
                    BulletsShot = Convert.ToInt32(reader.ReadLine());

                    //The stream reader is closed
                    reader.Close();

                    //All the extra values are added to the total values for the profile
                    EnemiesKilled += extraEnemiesKilled;
                    HealthLost += extraHealthLost;
                    DeathAmount += extraDeath;
                    TotalGameTime += extraGameTime;
                    BulletsShot += extraBulletsShot;

                    //A stream writer is created to save all the data in a new profile stats file
                    writer = new StreamWriter(userProfile + FILE_PATH_SUFFIX);

                    //All the data is writtern to the file
                    writer.WriteLine(EnemiesKilled);
                    writer.WriteLine(HealthLost);
                    writer.WriteLine(DeathAmount);
                    writer.WriteLine(TotalGameTime);
                    writer.WriteLine(BulletsShot);

                    //The stream writer is closed
                    writer.Close();

                    //If the new file is uploaded
                    if (onlineHelper.UpdateFile(userProfile + FILE_NAME_SUFFIX, userProfile + FILE_PATH_SUFFIX, FILE_TYPE, FILE_DESCRIPTION))
                    {
                        //Returns that the profile stats were updated for the current profile
                        return true;
                    }
                }
                //if the file was not downloaded
                else
                {
                    //Returns that the profile stats were not updated for the current profile
                    return false;
                }
            }
            //If the profile stats for the current profile do not exist
            else
            {
                //A stream writer is created to write all the data to a file
                writer = new StreamWriter(userProfile + FILE_PATH_SUFFIX);

                //All the data is written to the profile stats file
                writer.WriteLine(extraEnemiesKilled);
                writer.WriteLine(extraHealthLost);
                writer.WriteLine(extraDeath);
                writer.WriteLine(extraGameTime);
                writer.WriteLine(extraBulletsShot);

                //The stream writer is closed
                writer.Close();

                //If the file is uploaded onto the server
                if (onlineHelper.UpdateFile(userProfile + FILE_NAME_SUFFIX, userProfile + FILE_PATH_SUFFIX, FILE_TYPE, FILE_DESCRIPTION))
                {
                    //Returns that the profile stats were updated for the current profile
                    return true;
                }
            }

            //Returns that the profile stats were not updated for the current profile
            return false;
        }


        //Pre: The user that the profile is being loaded in for
        //Post: Boolean for whether the profile was read
        //Desc: A method that downloads and reads the profile stats for a user
        public bool ReadProfileStats(string userProfile)
        {
            //If the file is downloaded
            if (onlineHelper.DownloadFile(userProfile + FILE_PATH_SUFFIX, userProfile + FILE_NAME_SUFFIX))
            {
                //A stream reader is created to read the file
                reader = new StreamReader(userProfile + FILE_PATH_SUFFIX);

                //All the data from the file is read
                EnemiesKilled = Convert.ToInt32(reader.ReadLine());
                HealthLost = Convert.ToInt32(reader.ReadLine());
                DeathAmount = Convert.ToInt32(reader.ReadLine());
                TotalGameTime = (float)Convert.ToDouble(reader.ReadLine());
                BulletsShot = Convert.ToInt32(reader.ReadLine());

                //The stream reader is closed
                reader.Close();

                //Return that the profile was read
                return true;
            }

            //Return that the profile was not read
            return false;
        }
    }
}
