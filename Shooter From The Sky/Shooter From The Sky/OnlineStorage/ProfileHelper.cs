/*
 * Author: Rony Verch
 * File Name: ProfileHelper.cs
 * Project Name: Shooter From The Sky
 * Creation Date: January 6, 2015
 * Modified Date: January 20, 2015
 * Description: A helper class to save profile data onto an online server.
 */

using com.shephertz.app42.paas.sdk.csharp.upload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Shooter_From_The_Sky
{
    class ProfileHelper
    {
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// CONSTANTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //The suffix's and file type for the profile files
        public const string FILE_NAME = "Profiles.txt";
        public const string FILE_PATH = "Profiles.txt";
        public const string FILE_TYPE = "TXT";
        public const string FILE_DESCRIPTION = "In-game profiles";


        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /////////////////////////////////////// VARIABLES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Variable for the online helper and profile stats helper
        private OnlineStorageHelper onlineHelper;
        private ProfileStatsHelper profileStatsHelper;

        //Lists for the usersnames and the password for every username that was ever created
        private List<string> userNames = new List<string>();
        private List<string> passWords = new List<string>();

        //Variables for the stream reader and writer
        private StreamReader reader;
        private StreamWriter writer;


        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////// CONSTRUCTOR \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ///////////////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        //Pre: The online helper and profile stats helper objects
        //Post: The profile helper is created
        //Desc: A constructor for the profile helper
        public ProfileHelper(OnlineStorageHelper onlineHelper, ProfileStatsHelper profileStatsHelper)
        {
            //The online helper and profile stats helper are set
            this.onlineHelper = onlineHelper;
            this.profileStatsHelper = profileStatsHelper;
        }


        //Pre: The user and their password
        //Post: A boolean for whether the profile was found
        //Desc: A method for checking for a profile
        public bool CheckForProfile(string user, string pass)
        {
            //If the profiles are read
            if (ReadProfiles())
            {
                //Loop for every username
                for (int i = 0; i < userNames.Count; i++)
                {
                    //If the username is found
                    if (user == userNames[i])
                    {
                        //If the passwords match
                        if (pass == passWords[i])
                        {
                            //Returns that the profile is valid
                            return true;
                        }
                        //If the passwords do not match
                        else
                        {
                            //Returns that the profile is invalid
                            return false;
                        }
                    }
                }
            }

            //Returns that the profile is not valid
            return false;
        }


        //Pre: The new user name and password
        //Post: A boolean for whether the profile was added
        //Desc: A method for adding a profile
        public bool AddProfile(string newUser, string newPass)
        {
            //Boolean for whether the profiles were read
            bool profileAdded = ReadProfiles();

            //If the profile added boolean is true
            if (profileAdded)
            {
                //Loop for every user name
                for (int i = 0; i < userNames.Count; i++)
                {
                    //If the username already exists, set the profile added boolean to false
                    if (newUser == userNames[i])
                    {
                        profileAdded = false;
                    }
                }

                //If the profile is still considered as added
                if (profileAdded)
                {
                    //Adds the new username and password to the lists
                    userNames.Add(newUser);
                    passWords.Add(newPass);

                    //Adds the profile stats for the new user
                    profileStatsHelper.AddProfileStats(newUser);

                    //Writes the profiles to a file
                    WriteProfiles();

                    //Updates the new profiles file onto the server
                    onlineHelper.UpdateFile(FILE_NAME, FILE_PATH, FILE_TYPE, FILE_DESCRIPTION);
                }
            }
            //If the profile added boolean is not true
            else
            {
                //Clears the usernames and passwords
                userNames.Clear();
                passWords.Clear();

                //Adds the new username and password
                userNames.Add(newUser);
                passWords.Add(newPass);

                //Adds the profile stats for the new user
                profileStatsHelper.AddProfileStats(newUser);

                //Writes the profiles to a file
                WriteProfiles();

                //If the new profiles file is updated onto the server
                if (onlineHelper.UpdateFile(FILE_NAME, FILE_PATH, FILE_TYPE, FILE_DESCRIPTION))
                {
                    //The profile is set to added
                    profileAdded = true;
                }
            }

            //Return whether the profile was added or not
            return profileAdded;
        }


        //Pre: None
        //Post: A boolean for whether the profiles were read
        //Desc: A method for reading all the profiles file
        public bool ReadProfiles()
        {
            //If the file with all the profiles is downloaded
            if (onlineHelper.DownloadFile(FILE_PATH, FILE_NAME))
            {
                //Creates a stream reader to read all the profiles from the file
                reader = new StreamReader(FILE_PATH);

                //Clears the usernames and passwords currently saved
                userNames.Clear();
                passWords.Clear();

                //While the file still contains usernames and passwords to be read
                while (!reader.EndOfStream)
                {
                    //The current username and password is read from the file and added to the lists
                    userNames.Add(reader.ReadLine());
                    passWords.Add(reader.ReadLine());
                }

                //The stream reader is closed
                reader.Close();

                //True is returned representing that the profiles were read
                return true;
            }

            //False is returned representing that the profiles were not read
            return false;
        }


        //Pre: None
        //Post: The new profiles are written
        //Desc: A method for writing the profiles to a file
        public void WriteProfiles()
        {
            //A stream writer is created to write all the profiles to a file
            writer = new StreamWriter(FILE_PATH);

            //Loop through every username
            for (int i = 0; i < userNames.Count; i++)
            {
                //The username and the password for the current username are written to the file
                writer.WriteLine(userNames[i]);
                writer.WriteLine(passWords[i]);
            }

            //The stream writer is closed
            writer.Close();
        }
    }
}
