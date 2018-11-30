namespace Macromizer.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public class Macro
    {
        public object[] Memory = new object[10];
        public Macro()
        {
            LoopsLeft = 1;
            LoopDelay = 10;
            LoopCount = 1;
            Raw = new Dictionary<int, string>();
            ParameterizedInstructions = new Dictionary<int, string[]>();
            SaveLocation = @"C:\Users\Andrew Leon\Documents\Visual Studio 2015\Projects\Macromizer\Macromizer\Jobs\";
        }

        public bool Subroutine { get; set; }
        public string Name { get; set; }
        public string SaveLocation { get; set; }
        public DateTime AutoStartTime { get; set; }
        public int LoopsLeft { get; set; }
        public int LoopCount { get; set; }
        public int LoopDelay { get; set; }
        public Dictionary<int, string> Raw { get; set; }
        public Dictionary<int, string[]> ParameterizedInstructions { get; set; }

        public void Run()
        {
            Console.WriteLine("Job started: {0}", Name);
            LoopsLeft = LoopCount;
            while (LoopsLeft-- > 0)
            {
                foreach (KeyValuePair<int, string[]> command in ParameterizedInstructions)
                {
                    Interpreter.RunAction(this, command.Value);
                    Thread.Sleep(LoopDelay);
                }
            }
            if (!Subroutine)
            {
                Display.HitAKey(String.Format("The job {0} has ended...", Name));
            }
        }

        public void Load(string[] iCommands)
        {
            //Raw.Clear();
            int i = Raw.Count;
            foreach (string instruction in iCommands.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                Raw.Add(i, instruction);
                if (instruction.IndexOf("-") > 0 && instruction.Substring(0, instruction.IndexOf("-")) == "CNFG")
                {
                    string temp = instruction.Substring(instruction.IndexOf("-") + 1, instruction.Length - (instruction.IndexOf("-") + 1));
                    if (temp.StartsWith("NAME"))
                    {
                        Name = temp.Substring(temp.IndexOf("-") + 1, temp.Length - (temp.IndexOf("-") + 1));
                    }
                    else if (temp.StartsWith("LOOP"))
                    {
                        string tempCount = temp.Substring(temp.IndexOf("-") + 1, temp.Length - (temp.IndexOf("-") + 1));
                        int lTemp = 0;
                        if (int.TryParse(tempCount, out lTemp) && lTemp > 0)
                        {
                            LoopCount = lTemp;
                        }
                        else
                        {
                            LoopCount = 1;
                        }
                    }
                }
                else
                {
                    //Parse string for arguments.
                    string[] items = instruction.Split('-');
                    if (items != null && items.Length >= 1)
                    {
                        ParameterizedInstructions.Add(ParameterizedInstructions.Count, items);
                    }
                }
                i++;
            }
        }

        public void SaveToFile()
        {
            if (SaveLocation != null)
            {
                if (File.Exists(SaveLocation + Name + ".macro"))
                {
                    File.Delete(SaveLocation + Name + ".macro");
                }
                File.WriteAllLines(SaveLocation + Name + ".macro", Raw.Values);
            }
            else
            {
                Console.WriteLine("Please enter a save location");
                SaveLocation = Console.ReadLine();
            }
        }
    }
}
