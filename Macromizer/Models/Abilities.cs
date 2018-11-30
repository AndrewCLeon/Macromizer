namespace Macromizer.Models
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using WindowsInput;
    using WindowsInput.Native;

    public static class Abilities
    {
        private static IInputSimulator _InputSim = new InputSimulator();

        private static Hashtable _CharacterMap;
        private static Hashtable CharacterMap
        {
            get
            {
                if (_CharacterMap == null)
                {
                    _CharacterMap = new Hashtable();
                    _CharacterMap.Add('a', VirtualKeyCode.VK_A);
                    _CharacterMap.Add('b', VirtualKeyCode.VK_B);
                    _CharacterMap.Add('c', VirtualKeyCode.VK_C);
                    _CharacterMap.Add('d', VirtualKeyCode.VK_D);
                    _CharacterMap.Add('e', VirtualKeyCode.VK_E);
                    _CharacterMap.Add('f', VirtualKeyCode.VK_F);
                    _CharacterMap.Add('g', VirtualKeyCode.VK_G);
                    _CharacterMap.Add('h', VirtualKeyCode.VK_H);
                    _CharacterMap.Add('i', VirtualKeyCode.VK_I);
                    _CharacterMap.Add('j', VirtualKeyCode.VK_J);
                    _CharacterMap.Add('k', VirtualKeyCode.VK_K);
                    _CharacterMap.Add('l', VirtualKeyCode.VK_L);
                    _CharacterMap.Add('m', VirtualKeyCode.VK_M);
                    _CharacterMap.Add('n', VirtualKeyCode.VK_N);
                    _CharacterMap.Add('o', VirtualKeyCode.VK_O);
                    _CharacterMap.Add('p', VirtualKeyCode.VK_P);
                    _CharacterMap.Add('q', VirtualKeyCode.VK_Q);
                    _CharacterMap.Add('r', VirtualKeyCode.VK_R);
                    _CharacterMap.Add('s', VirtualKeyCode.VK_S);
                    _CharacterMap.Add('t', VirtualKeyCode.VK_T);
                    _CharacterMap.Add('u', VirtualKeyCode.VK_U);
                    _CharacterMap.Add('v', VirtualKeyCode.VK_V);
                    _CharacterMap.Add('w', VirtualKeyCode.VK_W);
                    _CharacterMap.Add('x', VirtualKeyCode.VK_X);
                    _CharacterMap.Add('y', VirtualKeyCode.VK_Y);
                    _CharacterMap.Add('z', VirtualKeyCode.VK_Z);
                    _CharacterMap.Add(' ', VirtualKeyCode.SPACE);
                    _CharacterMap.Add('.', VirtualKeyCode.OEM_PERIOD);
                }
                return _CharacterMap;
            }
        }

        #region Mouse

        public static void LeftMouseClick()
        {
            _InputSim.Mouse.LeftButtonClick();
        }

        public static void DoubleLeftMouseClick()
        {
            _InputSim.Mouse.LeftButtonDoubleClick();
        }

        public static void RightMouseClick()
        {
            _InputSim.Mouse.RightButtonClick();
        }

        public static void DoubleRightMouseClick()
        {
            _InputSim.Mouse.RightButtonDoubleClick();
        }

        public static void MouseScrollUp(int iScrollAmount = 10)
        {
            _InputSim.Mouse.VerticalScroll(iScrollAmount);
        }

        public static void MouseScrollDown(int iScrollAmount = 10)
        {
            _InputSim.Mouse.VerticalScroll(iScrollAmount);
        }

        public static void MoveMouseToPoint(int x, int y, double maxX = 1365, double maxY = 767.5)
        {
            Console.WriteLine("Moving mouse to {0:0000} {1:0000}", x, y);
            _InputSim.Mouse.MoveMouseTo(x * (65535 / maxX), y * (65535 / maxY));
        }

        public static void MoveMouseBy(int deltaX, int deltaY, double maxX = 1365, double maxY = 767.5)
        {
            //Get the current location of the mouse.
            Point cursorPosition = Cursor.Position;

            //Calculate the delta
            cursorPosition.X = cursorPosition.X + (int)((65535 / maxX) * deltaX);
            cursorPosition.Y = cursorPosition.Y + (int)((65535 / maxY) * deltaY);

            //Move Mouse To Point
            MoveMouseToPoint(cursorPosition.X, cursorPosition.Y, maxX, maxY);
        }

        public static void LeftMouseButtonDown()
        {
            _InputSim.Mouse.LeftButtonDown();
        }

        public static void LeftMouseButtonUp()
        {
            _InputSim.Mouse.LeftButtonUp();
        }

        public static void RightMouseButtonDown()
        {
            _InputSim.Mouse.RightButtonDown();
        }

        public static void RightMouseButtonUp()
        {
            _InputSim.Mouse.RightButtonUp();
        }

        #endregion

        #region System

        public static void ManualCut()
        {
            _InputSim.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
            Thread.Sleep(100);
            _InputSim.Keyboard.KeyPress(VirtualKeyCode.VK_X);
            Thread.Sleep(100);
            _InputSim.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            Thread.Sleep(300);
        }

        public static void ManualCopy()
        {
            _InputSim.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
            Thread.Sleep(100);
            _InputSim.Keyboard.KeyPress(VirtualKeyCode.VK_C);
            Thread.Sleep(100);
            _InputSim.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            Thread.Sleep(300);
        }

        public static void ManualPaste()
        {
            _InputSim.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
            Thread.Sleep(100);
            _InputSim.Keyboard.KeyPress(VirtualKeyCode.VK_V);
            Thread.Sleep(100);
            _InputSim.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
        }

        public static void WaitFor(int iMilliseconds)
        {
            Console.WriteLine("Waiting for {0:0000} ms", iMilliseconds);
            Thread.Sleep(iMilliseconds);
        }
        #endregion

        #region Files

        public static void CopyFile(string iFrom, string iTo)
        {
            File.Copy(iFrom, iTo);
        }

        public static void DeleteFile(string iFilePath)
        {
            File.Delete(iFilePath);
        }
        #endregion

        #region Keyboard

        public static void PressDownKey(VirtualKeyCode iKey)
        {
            _InputSim.Keyboard.KeyPress(iKey);
        }

        public static void LiftKeyPress(VirtualKeyCode iKey)
        {
            _InputSim.Keyboard.KeyUp(iKey);
        }

        public static void PressKey(VirtualKeyCode iKey)
        {
            _InputSim.Keyboard.KeyPress(iKey);
        }

        public static void WaitForKeyPress(char iChar, VirtualKeyCode iKey)
        {
            Console.WriteLine("Wait for {0} to be pressed.", iChar);
            bool waiting = true;
            while (waiting)
            {
                if (_InputSim.InputDeviceState.IsHardwareKeyDown(iKey))
                {
                    waiting = false;
                }
            }
            Console.WriteLine("{0} was pressed", iChar);
        }

        public static void Type(string iSentence)
        {
            for (int i = 0; i < iSentence.Length; i++)
            {
                bool requiresShift = false;
                PressKey(TranslateCharToKeyCode(iSentence[i], out requiresShift));
            }
        }
        #endregion

        #region Routines

        public static void RunJob(string iJobName)
        {
            //Pull job from storage.
            Macro lMacro = new Macro();
            string location = @"C:\Users\Andrew Leon\Documents\Visual Studio 2015\Projects\Macromizer\Macromizer\Jobs\";
            string lFullPath = location + iJobName + ".macro";
            Console.WriteLine("Looking for macro {0}", iJobName);
            if (File.Exists(lFullPath))
            {
                lMacro.Subroutine = true;
                //Load job
                Console.WriteLine("Macro found, loading it in now...");
                using (StreamReader lReader = new StreamReader(lFullPath))
                {
                    lMacro.Load(lReader.ReadToEnd().Split(Environment.NewLine.ToCharArray()));
                    lReader.Close();
                }
                Thread.Sleep(250);
                Console.WriteLine("Macro loaded and ready to run...");
                //return job execution as action.
                lMacro.Run();
            }
            else
            {
                Console.WriteLine("No such macro found.");
            }
        }

        #endregion

        #region Processes

        public static void StartProcess(string iProcess)
        {
            Console.WriteLine("Checking if process is already started");
            if (Process.GetProcessesByName(iProcess).Length == 0)
            {
                try
                {
                    Console.WriteLine("Process is not running, attempting to start it.");
                    Process.Start(iProcess);
                }
                catch (Win32Exception lEx)
                {
                    if(lEx.ErrorCode == 0x8007005)
                    {
                        Console.WriteLine("Access denied, unable to start process {0}.", iProcess);
                    }
                }
                Console.WriteLine("Started {0} at {1}", iProcess, DateTime.Now);
            }
            else
            {
                Console.WriteLine("{0} is already running...");
            }
        }

        public static void EndProcess(string iProcess)
        {
            int attempt = 1;
            Console.WriteLine("Attempting to kill {0}", iProcess);
            while (Process.GetProcessesByName(iProcess).Length > 0)
            {
                try
                {
                    Process.GetProcessesByName(iProcess)[0].Kill();
                }
                catch (Win32Exception lException)
                {
                    if (lException.ErrorCode == 0x8007005)
                    {
                        Console.WriteLine("Access denied");
                    }
                }
                Thread.Sleep(250);
                Console.WriteLine("Attempt {0}", attempt++);
            }
        }

        public static void WaitForProcessToRespond(string iProcessName)
        {
            System.Collections.IEnumerator lEnum = Process.GetProcessesByName(iProcessName).GetEnumerator();
            if (lEnum.MoveNext())
            {
                Process lProcess = (Process)lEnum.Current;
                int attempt = 0;
                while (lProcess != null && !lProcess.Responding)
                {
                    attempt++;
                    Console.WriteLine("Waiting for process to respond {0:00}", attempt);
                }
            }
            else
            {
                Console.WriteLine("No process by the name of {0} was found running", iProcessName);
            }
        }

        public static void OpenWindowsFileLocation(string iLocation)
        {
            // ToDo: Test to make sure OpenWindowsFileLocation works.
            Process.Start(new ProcessStartInfo("run.exe", iLocation));
        }

        #endregion
        
        public static VirtualKeyCode TranslateCharToKeyCode(char iCharacter, out bool requiresShift)
        {
            //Todo: move hash table to new area of concern, should not exist in abilities.

            //Assign to requires shift first.
            requiresShift = char.IsUpper(iCharacter);

            //I only store lowercase letters in the hash table.
            char lLower = char.ToLower(iCharacter);

            //Obtain key code to press.
            VirtualKeyCode oKey = (VirtualKeyCode)CharacterMap[lLower];

            //Return key.
            return oKey;
        }

    }
}
