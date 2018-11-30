namespace Macromizer.Models
{
    using Custom;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using WindowsInput;
    using WindowsInput.Native;

    public static class Display
    {
        private static Macro _LoadedMacro;
        public static Macro Macro
        {
            get
            {
                return _LoadedMacro;
            }
        }

        private static void RewriteScreen()
        {
            //How to store screen and re-print after sub-screen.
        }

        public static void LoadMacro(Macro iMacro)
        {
            _LoadedMacro = iMacro;
        }

        public static void LoadMacroFromFile(string iFilePath)
        {
            if (File.Exists(iFilePath))
            {
                Console.WriteLine("Macro found, loading it in now...");
                Macro lMacro = new Macro();
                using (StreamReader lReader = new StreamReader(iFilePath))
                {
                    lMacro.Load(lReader.ReadToEnd().Split(Environment.NewLine.ToCharArray()));
                    lReader.Close();
                }
                LoadMacro(lMacro);
                Thread.Sleep(250);
                Console.WriteLine("Macro loaded...");
            }
            else
            {
                Console.WriteLine("No such macro found.");
            }
            Display.HitAKey();
        }

        public static void UnloadMacro()
        {
            _LoadedMacro = null;
        }

        public static void RunMacro()
        {
            if (_LoadedMacro != null && _LoadedMacro.ParameterizedInstructions.Count > 0)
            {
                _LoadedMacro.Run();
            }
        }

        private static void DisplayCreateMacroIntroduction()
        {
            Console.WriteLine("You have chosen to create a new Macro.");
            Console.WriteLine("A macro is a series of instructions ran in order.");
            Console.WriteLine();
        }

        private static void PrintCurrentMacroInstructions()
        {
            if (_LoadedMacro != null)
            {
                int i = 0;
                foreach (KeyValuePair<int, string> instruction in _LoadedMacro.Raw)
                {
                    Console.WriteLine("-{0}--", i);
                    i++;
                }
                Console.Write("-{0}--", i);
            }
        }

        public static void CreateMacro()
        {
            DisplayCreateMacroIntroduction();
            if (_LoadedMacro != null)
            {
                ConsoleColor prevColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Warning! Your macro is about to be overwritten.");
                Console.ForegroundColor = prevColor;
                Console.WriteLine("Press 'Y' or 'y' to continue, anything else exits.");
                ConsoleKeyInfo lKey = Console.ReadKey(true);
                if (lKey.Key != ConsoleKey.Y)
                    return;

                C.ClearLine(Console.CursorTop - 2);
                C.ClearLine(Console.CursorTop - 1);
                Console.SetCursorPosition(0, Console.CursorTop - 2);
            }
            _LoadedMacro = new Macro();

            Console.WriteLine("What is the name of the Macro?");
            _LoadedMacro.Name = Console.ReadLine();
            Console.WriteLine();
            _LoadedMacro.Raw.Add(0, String.Format("CNFG-NAME-{0}", _LoadedMacro.Name));

            Console.WriteLine("How many times should this macro repeat itself?");
            int loopCount = 0;
            int.TryParse(Console.ReadLine(), out loopCount);
            _LoadedMacro.LoopCount = loopCount;

            _LoadedMacro.Raw.Add(1, String.Format("CNFG-LOOP-{0}", _LoadedMacro.LoopCount));
            Console.WriteLine();
            Console.WriteLine("How long, in ms, should the program wait inbetween instructions?");
            int delay = 0;
            int.TryParse(Console.ReadLine(), out delay);
            _LoadedMacro.LoopDelay = delay;

            Console.Clear();
            Display.ViewMacroHeader();

            Console.WriteLine();
            int i = 0;
            bool writingMacro = true;
            bool previousInstructionEmpty = false;
            List<string> instructions = new List<string>();
            Display.DisplayCommands(ConsoleColor.Green, 25, 6);
            while (writingMacro)
            {
                if (!previousInstructionEmpty)
                {
                    Console.Write("-{0:00}--", i);
                }
                string command = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(command))
                {
                    command = FindAndAppendArguments(command.ToUpper());
                    instructions.Add(command);
                    if (command.ToUpper() == "HALT")
                    {
                        writingMacro = false;
                    }
                    i++;
                }
                else
                {
                    Console.SetCursorPosition(5, Console.CursorTop - 1);
                    previousInstructionEmpty = true;
                }
            }
            _LoadedMacro.Load(instructions.ToArray());
            Console.WriteLine();
            Console.SetCursorPosition(2, Console.CursorTop);
            if (_LoadedMacro.Raw.Count < 3)
            {
                Console.Write("WTF? Really!?!?");
                Console.SetCursorPosition(2, Console.CursorTop + 1);
                Console.Write("Hit any key.");
            }
            else if (_LoadedMacro.Raw.Count >= 3 && _LoadedMacro.Raw.Count < 7)
            {
                Console.Write("Just a test.");
                Console.SetCursorPosition(2, Console.CursorTop + 1);
                Console.Write("Hit any key.");
            }
            else if (_LoadedMacro.Raw.Count >= 7 && _LoadedMacro.Raw.Count < 13)
            {
                Console.Write("Looks Good!");
                Console.SetCursorPosition(2, Console.CursorTop + 1);
                Console.Write("Hit any key.");
            }
            else if (_LoadedMacro.Raw.Count > 20)
            {
                Console.Write("Now you are just showing off.");
                Console.SetCursorPosition(2, Console.CursorTop + 1);
                Console.Write("Hit any key.");
            }
            Console.ReadKey();
        }

        private static string FindAndAppendArguments(string cmd)
        {
            StringBuilder oCommand = new StringBuilder(cmd);
            List<string> arguments = CheckCommandForArguments(cmd);

            if (arguments.Count > 0)
            {
                foreach (string arument in arguments)
                {
                    oCommand.Append(String.Format("-{0}", arument));
                }
            }
            return oCommand.ToString();
        }

        private static void PrepareConsoleForArgument(string iCommand)
        {
            Console.CursorTop--;
            Console.CursorLeft += iCommand.Length + 5;
            Console.Write("-");
        }

        private static List<string> CheckCommandForArguments(string iCmd)
        {
            List<string> oArguments = new List<string>();
            switch (iCmd)
            {
                case "MMTP":
                    //X and Y
                    PrepareConsoleForArgument(iCmd);

                    bool capturingPoint = true;
                    InputSimulator simulator = new InputSimulator();
                    Point lastKnownMouseCoordinate = Cursor.Position;

                    while (capturingPoint)
                    {
                        Console.SetCursorPosition(iCmd.Length + 6, Console.CursorTop);
                        Console.Write("{0:0000}-{1:0000}", Cursor.Position.X, Cursor.Position.Y);
                        if (simulator.InputDeviceState.IsHardwareKeyDown(VirtualKeyCode.SPACE))
                        {
                            //Maybe capture cursor position as variable, incase it moves.
                            if (Cursor.Position.X != lastKnownMouseCoordinate.X || Cursor.Position.Y != lastKnownMouseCoordinate.Y)
                            {
                                oArguments.Add(String.Format("{0:0000}", Cursor.Position.X));
                                oArguments.Add(String.Format("{0:0000}", Cursor.Position.Y));
                                capturingPoint = false;
                            }
                        }
                        Thread.Sleep(10);
                    }
                    simulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
                    Console.Write("\n");
                    break;
                default:
                    break;
            }
            return oArguments;
        }

        public static void MainMenu(bool clear = true)
        {
            if (clear)
                Console.Clear();

            int optionIndex = 1;
            Console.WriteLine("{0}.) Create Macro", optionIndex++);
            Console.WriteLine("{0}.) Load Macro", optionIndex++);
            Console.WriteLine("{0}.) Run Macro", optionIndex++);

            if (_LoadedMacro != null)
            {
                C.SetTemporaryColor(ConsoleColor.Red, () =>
                {
                    Console.WriteLine("{0}.) Edit Macro", optionIndex++);
                    return true; //because I can't return void.
                });
                Console.WriteLine("{0}.) Save Macro", optionIndex++);
                C.SetTemporaryColor(ConsoleColor.Red, () =>
                {
                    Console.WriteLine("{0}.) Configure Macro", optionIndex++);
                    return true;
                });
                Console.WriteLine("{0}.) Unload Macro", optionIndex++);
                Console.WriteLine("{0}.) View Macro", optionIndex++);
            }
        }
        
        public static void DisplayActions()
        {
            Console.WriteLine("1.) Left Mouse Click");
            Console.WriteLine("2.) Right Mouse Click");
            Console.WriteLine("3.) Double Left Mouse Click");
            Console.WriteLine("4.) Double Right Mouse Click");
            Console.WriteLine("5.) Move Mouse to Point");
            Console.WriteLine("6.) Left Mouse Button Down");
            Console.WriteLine("7.) Left Mouse Button Up");
            Console.WriteLine("8.) Right Mouse Button Down");
            Console.WriteLine("9.) Right Mouse Button Up");
            Console.WriteLine("10.) CUT - CTRL + X");
            Console.WriteLine("11.) Copy - CTRL + C");
            Console.WriteLine("12.) Paste - CTRL + V");
            Console.WriteLine("13.) Type Text");
            Console.WriteLine("14.) Wait for a specified time");
            Console.WriteLine("15.) Start a process");
            Console.WriteLine("16.) End a process");
            Console.WriteLine("17.) Wait for Key press");
            Console.WriteLine("18.) Copy File");
            Console.WriteLine("19.) Delete File");
            Console.WriteLine("20.) Key Down");
            Console.WriteLine("21.) Key Up");
            Console.WriteLine("22.) Key Press");
            Console.WriteLine("23.) Run Job");
            Console.WriteLine("24.) Wait for process to respond");
            Console.WriteLine("25.) Open windows file location");
            Console.WriteLine("26.) Halt - Stops a job");
            Console.ReadKey();
        }

        private static void ViewMacroHeader()
        {
            Console.WriteLine();
            Console.Write("Name:  {0}\n", _LoadedMacro.Name);
            Console.Write("Loop:  {0}\n", _LoadedMacro.LoopCount);
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------");
        }

        public static void ViewMacro()
        {
            if (_LoadedMacro != null)
            {
                Console.Clear();
                ViewMacroHeader();
                if (_LoadedMacro != null && _LoadedMacro.Raw.Count > 0)
                {
                    foreach (KeyValuePair<int, string> instruction in _LoadedMacro.Raw)
                    {
                        Console.WriteLine("-{0:00}-- {1}", instruction.Key, instruction.Value);
                    }
                }
                else if (_LoadedMacro.Raw.Count == 0)
                {
                    Console.WriteLine("I'm sorry, but the macro does not have any instructions.");
                }
            }
            else
            {
                Console.WriteLine("I'm sorry, but there is no macro loaded into memory.");
            }
            HitAKey();
        }

        public static void SaveMacro()
        {
            if (_LoadedMacro != null)
            {
                _LoadedMacro.SaveToFile();
            }
        }

        public static void DisplayMacroList(string iJobsLocation)
        {
            Console.WriteLine("Please type in the name of a macro.");
            Console.WriteLine();

            FileInfo[] lFiles = Directory.GetFiles(iJobsLocation).
                Where(x => x.EndsWith(".macro")).Select(x => new FileInfo(x)).ToArray();

            for (int i = 0; i < lFiles.Length; i++)
            {
                Console.WriteLine("{0}.) {1}", i, lFiles[i].Name.Substring(0, lFiles[i].Name.IndexOf(".")));
            }

            string lName = Console.ReadLine();
            Display.LoadMacroFromFile(@"C:\Users\Andrew Leon\Documents\Visual Studio 2015\Projects\Macromizer\Macromizer\Jobs\" + lName + ".macro");
        }

        public static void HitAKey(string additionalMessage = "")
        {
            if (additionalMessage != "")
            {
                Console.WriteLine(additionalMessage);
                Console.WriteLine();
            }
            Console.WriteLine("Please hit a key to continue...");
            Console.ReadKey();
        }

        public static void DisplayCommands(ConsoleColor iColor, int howFarFromLeft, int howFarFromTop)
        {
            ConsoleColor prevBackgroundColor = Console.BackgroundColor;
            ConsoleColor prevForegroundColor = Console.ForegroundColor;
            int prevCursorPosX = Console.CursorLeft;
            int prevCursorPosY = Console.CursorTop;

            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            DisplayColorLegend(iColor);
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            C.SetTemporaryColor(ConsoleColor.DarkGreen, () => { Console.Write("---- Commands ----"); return true; });
            Console.ForegroundColor = iColor;
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("LMC  - Left Mouse Click");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("RMC  - Right Mouse Click");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("DLC  - Double Left Mouse Click");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("DRC  - Double Right Mouse Click");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("LMCD - Left Mouse Click Down");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("LMCU - Left Mouse Click Up");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("RMCD - Right Mouse Click Down");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("RMCU - Right Mouse Click Up");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);

            C.SetTemporaryColor(ConsoleColor.Blue, () => { Console.Write("MMTP "); return true; });
            Console.Write("- Move Mouse to Point. Press enter after MMTP...");

            Console.SetCursorPosition(howFarFromLeft + 7, howFarFromTop++);
            Console.Write("and press SPACE to capture your point.");
            Console.ForegroundColor = iColor;
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("WFKP - Wait For Key Press. Example 'WFKP-A.'");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("KEYD - Key Down. Presses down that key.");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("KEYU - Key Up. Lifts a key press, if any.");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("KEYP - Key Press. Single key press.");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("SP   - Start Process. Example 'mspaint'.");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.Write("EP   - End Process (All of them). Example 'mspaint'.");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("OWFL - Open Windows File Location.");
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);
            Console.SetCursorPosition(howFarFromLeft, howFarFromTop++);

            C.SetTemporaryColor(ConsoleColor.Blue, () => { Console.Write("HALT "); return true; });
            Console.ForegroundColor = iColor;
            Console.Write("- Halts the program and exits the editor.");

            Console.BackgroundColor = prevBackgroundColor;
            Console.ForegroundColor = prevForegroundColor;
            Console.SetCursorPosition(prevCursorPosX, prevCursorPosY);
        }

        public static void DisplayColorLegend(ConsoleColor iPrimary)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = iPrimary;
            Console.Write("Working  ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Broken  ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Special  ");
            Console.ForegroundColor = prevColor;
        }
    }
}
