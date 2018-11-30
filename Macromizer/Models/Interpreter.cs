namespace Macromizer.Models
{
    using System;
    using System.Windows.Forms;
    using WindowsInput;
    using WindowsInput.Native;

    public static class Interpreter
    {
        private static InputSimulator InputSimulator
        {
            get
            {
                return new InputSimulator();
            }
        }

        public static decimal _ScreenWidthMax = 1367;
        public static decimal _ScreenHeightMax = 767;

        public static void RunAction(Macro iMacro, params string[] arguments)
        {
            string cmd = arguments[0];
            switch (cmd)
            {
                case "LMC":
                    Abilities.LeftMouseClick();
                    break;

                case "DLC":
                    Abilities.DoubleLeftMouseClick();
                    break;

                case "RMC":
                    Abilities.RightMouseClick();
                    break;

                case "DRC":
                    Abilities.DoubleRightMouseClick();
                    break;

                case "MMC":
                    throw new NotImplementedException();
                    break;

                case "MMB":

                    break;

                case "MMTP":
                    int x, y;
                    int.TryParse(arguments[1], out x);
                    int.TryParse(arguments[2], out y);

                    //Parse mouse coordinates from aruments.
                    Abilities.MoveMouseToPoint(x, y);
                    break;

                case "LMCD":
                    Abilities.LeftMouseButtonDown();
                    break;

                case "LMCU":
                    Abilities.LeftMouseButtonUp();
                    break;

                case "RMCD":
                    Abilities.RightMouseButtonDown();
                    break;

                case "RMCU":
                    Abilities.RightMouseButtonUp();
                    break;

                case "MSU":
                    Abilities.MouseScrollUp();
                    break;

                case "MSD":
                    Abilities.MouseScrollDown();
                    break;

                case "MCUT":
                    Abilities.ManualCut();
                    break;

                case "MCPY":
                    Abilities.ManualCopy();
                    break;

                case "MPST":
                    Abilities.ManualPaste();
                    break;

                case "TYPE":
                    string sentence = arguments[1];
                    Abilities.Type(sentence);
                    break;

                case "CPYF":
                    //Parse 2 arguments, "from" and "to" locations.
                    string from = arguments[1];
                    string to = arguments[2];
                    Abilities.CopyFile(from, to);
                    break;

                case "DELF":
                    Abilities.DeleteFile("");
                    break;

                case "KEYD":
                    //Parse key argument.
                    //VirtualKeyCode lKey = MethodToParseKeyFromString();
                    // InputSimulator.Keyboard.KeyDown(lKey);
                    char lCharacter = ' ';
                    string key = arguments[1];
                    if (char.TryParse(key, out lCharacter))
                    {
                        bool requiresShift = false;
                        VirtualKeyCode lKey = Abilities.TranslateCharToKeyCode(lCharacter, out requiresShift);
                        Abilities.PressDownKey(lKey);
                    }
                    break;

                case "KEYU":
                    //Parse key argument.
                    lCharacter = ' ';
                    key = arguments[1];
                    if (char.TryParse(key, out lCharacter))
                    {
                        bool requiresShift = false;
                        VirtualKeyCode lKey = Abilities.TranslateCharToKeyCode(lCharacter, out requiresShift);
                        Abilities.LiftKeyPress(lKey);
                    }
                    break;

                case "KEYP":
                    Abilities.PressKey(VirtualKeyCode.SPACE);
                    break;

                case "RUNJ":
                    //Parse job name.
                    string jobName = arguments[1];
                    Abilities.RunJob(jobName);
                    break;

                case "WAIT":
                    //Parse argument here.
                    int.TryParse(arguments[1], out x);
                    Abilities.WaitFor(x);
                    break;

                case "WFKP":
                    //Parse arugument for keypress.
                    key = arguments[1];
                    lCharacter = ' ';

                    if (char.TryParse(key, out lCharacter))
                    {
                        bool requiresShift = false;
                        VirtualKeyCode lRequestedKey = Abilities.TranslateCharToKeyCode(lCharacter, out requiresShift);
                        Abilities.WaitForKeyPress(lCharacter, lRequestedKey);
                    }
                    break;

                case "WFPTR":
                    //Parse process name
                    string processName = arguments[1];
                    Abilities.WaitForProcessToRespond(processName);
                    break;

                case "OWFL":
                    //Parse arugument for file location.
                    Abilities.OpenWindowsFileLocation("");
                    break;

                case "SP":
                    string iProcess = arguments[1];
                    if (!iProcess.EndsWith(".exe"))
                    {
                        iProcess = iProcess + ".exe";
                    }
                    Abilities.StartProcess(iProcess);
                    break;

                case "EP":
                    iProcess = arguments[1].Replace(".exe", "");
                    Abilities.EndProcess(iProcess);
                    break;

                case "LOG":
                    sentence = "";
                    if (arguments.Length > 1)
                    {
                        sentence = arguments[1];
                    }
                    Console.WriteLine(sentence);
                    break;

                case "CBTTM":
                    int index = 0;
                    string argument = arguments[0];
                    if(int.TryParse(argument, out index) && index >= 0 && index < 10)
                    {
                        iMacro.Memory[index] = Clipboard.GetText();
                    }
                    break;

                case "MTCB":
                    index = 0;
                    argument = arguments[1];
                    if(int.TryParse(argument, out index) && index >= 0 && index < 10)
                    {
                        if(iMacro.Memory[index] != null)
                        {
                            Clipboard.SetText((string)iMacro.Memory[index]);
                        }
                    }
                    break;

                default:

                    break;
            }
        }
    }
}
