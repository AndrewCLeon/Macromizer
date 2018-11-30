namespace Macromizer
{
    using Custom;
    using Models;
    using System;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    class Program
    {
        private static readonly string _JobsLocation = LookForJobsFolder(Application.ExecutablePath);

        private static void Init()
        {
            Point lPreviously = Cursor.Position;
            Abilities.MoveMouseToPoint(65535, 65535, 65535, 65535);
            Point max = Cursor.Position;
            Console.WriteLine("Screen adjusted to {0}x{1}", max.X + 1, max.Y + 1);
            Interpreter._ScreenHeightMax = max.X;
            Interpreter._ScreenWidthMax = max.Y + 0.05m;
            Abilities.MoveMouseToPoint(lPreviously.X, lPreviously.Y, max.X, max.Y);
            Console.SetWindowSize(Console.BufferWidth, 50);
            Console.WriteLine();
            Display.HitAKey(String.Format("Screen size adjusted to {0}x{1}", max.X, max.Y));
            //lMacro.Instructions.Add(1, () => Console.WriteLine("Hello World!"));
            //lMacro.Instructions.Add(2, () => Console.WriteLine("Hello World!"));
            //lMacro.Instructions.Add(3, () => Console.WriteLine("Hello World!"));
            //lMacro.Instructions.Add(4, () => Console.WriteLine("Hello World!"));
            //lMacro.Instructions.Add(5, () => Console.WriteLine("Hello World!"));

            //lMacro.Instructions.Add(6, Abilities.LeftClick);
            //lMacro.Instructions.Add(7, () => Thread.Sleep(500));
            //lMacro.Instructions.Add(8, Abilities.LeftClick);
            //lMacro.Instructions.Add(9, () => Thread.Sleep(500));
            //lMacro.Instructions.Add(10, Abilities.LeftClick);
            //lMacro.Instructions.Add(11, () => Thread.Sleep(500));
        }

        [STAThread]
        static void Main(string[] args)
        {
            Init();
            MainMenu();
        }

        //CCBTM-0

        //Set current cursor coordinates to storage.

        //MMB-XXXX-XXXX MMB-50+-50 - Move mouse by. Figuring out negatives.

        //CBTTM-10
        private static void MainMenu()
        {
            bool menuRunning = true;
            while (menuRunning)
            {
                Console.Clear();
                Display.DisplayColorLegend(ConsoleColor.Green);
                Console.WriteLine();
                C.NextLine();
                Display.MainMenu(false);
                int optionIndex = ObtainSafeChoice();

                switch (optionIndex)
                {
                    case 1:
                        Console.Clear();
                        Display.CreateMacro();
                        break;

                    case 2:
                        //Obtain macro name.
                        Display.DisplayMacroList(_JobsLocation);
                        break;

                    case 3:
                        Display.RunMacro();
                        break;

                    case 4:
                        //Edit Macro
                        //Display macro.
                        //Choose index 
                        break;

                    case 5:
                        Display.SaveMacro();
                        Console.WriteLine();
                        Console.WriteLine("Macro saved...");
                        break;

                    case 7:
                        Display.UnloadMacro();
                        break;

                    case 8:
                        Display.ViewMacro();
                        break;

                    default:
                        break;
                }
            }
        }

        private static int ObtainSafeChoice()
        {
            int oResponse = 0;
            bool invalid = true;
            do
            {
                if (!int.TryParse(Console.ReadLine(), out oResponse))
                {
                    Console.WriteLine("Please try again.");
                }
                else
                {
                    invalid = false;
                }
            } while (invalid);
            return oResponse;
        }

        private static string LookForJobsFolder(string currentDirectory)
        {
            string oResponse = "";
            if (!currentDirectory.EndsWith("Macromizer\\"))
            {
                //Remove last directory.
                string[] temp = currentDirectory.Split('\\').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                StringBuilder lBuilder = new StringBuilder();
                for (int i = 0; i < temp.Length - 1; i++)
                {
                    lBuilder.Append(temp[i] + "\\");
                }
                oResponse = LookForJobsFolder(lBuilder.ToString());
            }
            else
            {
                oResponse = currentDirectory + @"Jobs\";
            }
            return oResponse;
        }

        //Formatting
        //Configurations are always placed above actions.
        //Configuration lines start with CNFG-{0}
        //Example CNFG-NAME-"This is the Job Name"

        //Load job
        //Run job, must load a job, if none are loaded, present a list of them.
        //Save job, can only be done if a job is loaded.
        //Confiure Job, can only be done if a job is loaded.
        //Unload job, occurs after running or can be manually unloaded.
        //Create job, cannot be done if job is loaded.

        //Each job is a series of instructions
        //Jobs can be ran manually
        //Jobs can be set to start at a certain time.
        //When the user creates a job, the program captures the instructions from the user.
        //The time in-between 2 instructions may be recorded, set to a standard delay, ran instantly or set manually.


        //Instruction Configurations
        //Name - "Job Name"
        //Save Location - "Location"
        //Auto Start - Date and Time, or just time.
        //Loop - True / False
        //Loop Specified - 10 times


        //Instruction Actions
        //Single Left Click
        //Double Left Click
        //Right Click
        //Double Right Click
        //Middle mouse button click

        //Move mouse to Point
        //Left Mouse Click down
        //Left Mouse Click up
        //Manual Copy  - Ctrl + C
        //Manual Paste - Ctrl + V
        //Manual Cut   - Ctrl + X
        //Type Sentence - s
        //Wait for x milliseconds
        //Start Process - "Process Name"
        //End Process - "Process Name"
        //Wait for key press
        //Copy File
        //Delete File
        //Key down
        //Key up

        //AUTOS-00/00/0000-00:00:00
        //RC
        //DRC
        //LC
        //DLC
        //MM-1254-0039

        //LCD
        //LCU

        //RCD
        //RCU

        //MCPY
        //MCUT
        //MPST

        //TYPS-"Sentence"
        //WAIT-1252

        //SP-"Toad"
        //EP-"Toad"

        //WFKP-65
    }
}
