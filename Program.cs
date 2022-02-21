using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ExcelReader
{
    class Program
    {
        //String that holds the Windows folder path that the application will look for the file name
        private static string strDirectoryPath = "";
        //String that will hold the file name the user looks up
        private static string strFileName = "";

        //The string list containers that will be used to store and track valid and invalid email addresses
        private static List<string> lsValidEmails = new List<string>();
        private static List<string> lsInvalidEmails = new List<string>();

        static void Main(string[] args)
        {
            //Boolean tracker to keep loop running until user submits valid response
            bool hasValidDirectory = Directory.Exists(strDirectoryPath);            

            //Loop until the Windows folder path user provides exists
            while(!hasValidDirectory)
            {
                //Request user input for Windows folder path
                Console.WriteLine("It appears that you don't have a valid file directory to search from...");
                Console.WriteLine("Please input a valid file directory...");
                //Get user input and store in DirectoryPath string variable
                strDirectoryPath = Console.ReadLine();

                //Directory.Exists will check if the folder path exists that the user provides and store it for verification
                hasValidDirectory = Directory.Exists(strDirectoryPath);

                //Print error message before looping if the directory is not valid
                if (!hasValidDirectory)
                    Console.WriteLine("That is not a valid directory, please try again...");
            }

            //Boolean tracker to keep loop running until user submits valid file name response
            bool isValidFile = false;

            //Loop until the file name provided exists within the Windows path provided
            while(!isValidFile)
            {
                //Request user input for file name (Case Sensitive and no file extension)
                Console.WriteLine("Please input a file name with no file extension (Case Sensitive)...");
                //Get user input and store in FileName string variable
                strFileName = Console.ReadLine();

                //File.Exists will check if the file exists via the buildFilePath method and store it for verification
                isValidFile = File.Exists(buildFilePath(strDirectoryPath, strFileName));

                //If file is not found then give the user an error message
                if (!isValidFile)
                    Console.WriteLine("That file does not exist, please try again...");

                //If the file is found begin processing the file and alert the user of the process
                else
                {
                    Console.WriteLine("File found...");
                    Console.WriteLine("Processing...");

                    //Wrapped in a try catch to prevent any issues with the Stream Reader and alert the user of such errors
                    try
                    {
                        //Open the Stream Reader for the csv file that was found earlier using buildFilePath function with users inputs
                        using (StreamReader sr = new StreamReader(buildFilePath(strDirectoryPath, strFileName)))
                        {
                            //String variable to store each line as it is received
                            String line;

                            //Simple traditional way to loop through a line based file
                            //Checks if the line is currently null, if it is the loop will exit
                            while ((line = sr.ReadLine()) != null)
                            {
                                //Split the csv by it's comma and store the third column (In this case the email)
                                //Traditionally this would be broken into an interface and then injected into a 
                                //class which would also handle the validation check, but in a small scale application
                                //I find it more worth doing the work inline since few changes are needed
                                string email = line.Split(',')[2];

                                //Uses the isValidEmail method to check whether the email should be added to the ValidEmail list object
                                if (isValidEmail(email))
                                    lsValidEmails.Add(email);

                                //If it's not valid then it is added to the InvalidEmail list object
                                else
                                    lsInvalidEmails.Add(email);
                            }

                            //Close the StreamReader object, the Using block normally takes care of this
                            //but it's worth having here as a precaution
                            sr.Close();
                        }

                        //If the count is greater than zero display the valid email addresses
                        if (lsValidEmails.Count > 0)
                        {
                            //Let the user know that the valid email addresses will be displayed
                            Console.WriteLine("Following is a list of valid email addresses...");
                            //Extra line for styling
                            Console.WriteLine();
                            //Loop through every index in the Valid Emails list object
                            foreach (string email in lsValidEmails)
                            {
                                //Write out the email found in the csv
                                Console.WriteLine(email);
                            }
                            //Extra line for styling
                            Console.WriteLine();
                        }
                        //If there are no entries in the ValidEmails list notify the user and move on
                        else
                        {
                            //Display the notification to the user
                            Console.WriteLine("There are no valid emails in the provided csv file...");
                            //Extra line for styling
                            Console.WriteLine();
                        }
                        //If the count is greater than zero display the invalid email addresses
                        if (lsInvalidEmails.Count > 0)
                        {
                            //Let the user know that the invalid email addresses will be displayed
                            Console.WriteLine("Following is a list of invalid email addresses...");
                            //Extra line for styling
                            Console.WriteLine();
                            //Loop through every index in the Invalid Emails list object
                            foreach (string email in lsInvalidEmails)
                            {
                                //Write out the invalid email found in the csv
                                Console.WriteLine(email);
                            }
                            //Extra line for styling
                            Console.WriteLine();
                        }
                        //If there are no entries in the InvalidEmails list notify the user and move on
                        else
                        {
                            //Display the notification to the user
                            Console.WriteLine("There are no invalid emails in the provided csv file...");
                            //Extra line for styling
                            Console.WriteLine();
                        }
                        //Wait for the user to hit a button to close the application
                        Console.ReadLine();

                    }
                    //Catches any exceptions caused by the file lookup or stream reader
                    //Normally a list of specific exceptions should be considered and accounted for
                    //But in this case I'm using generic error catching since it won't have much of an impact.
                    catch (Exception ex)
                    {
                        //Write out the error for the user
                        Console.WriteLine(ex.Message);
                        //Allows the user to view the error before the app closes
                        Console.ReadLine();
                    }
                }
            }
        }

        /*
         * METHOD NAME: isValidEmail
         * RETURN TYPE: Boolean
         * ARGUMENTS:
         *  string strEmail - The email that will be validated
         * METHOD TASK: Check if a given email is a valid format according to modern Regex standards         * 
         */
        static bool isValidEmail(string strEmail)
        {
            //Regex string pattern for email address validation
            string strPattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

            //If the email passes the Regex pattern, return true because email is valid
            if (Regex.IsMatch(strEmail, strPattern))
                return true;

            //If it fails the Regex pattern, return false because the email is not valid
            else
                return false;
        }

        /*
         * METHOD NAME: buildFilePath
         * RETURN TYPE: String
         * ARGUMENTS:
         *  string strDirectory - The Windows folder path
         *  string strFileName - The name of the file
         * METHOD TASK: Build the full path for the file including default extension from inputted arguments
         */
        static string buildFilePath(string strDirectory, string strFileName)
        {
            return strDirectory + '/' + strFileName + ".csv";
        }
    }
}
