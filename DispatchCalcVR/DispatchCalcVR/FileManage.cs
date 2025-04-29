using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DispatchCalcVR
{
    /// <summary>
    /// Manages saving and loading of variables (and ans) to/from a text file.
    /// </summary
    public class FileManager
    {
        public void SaveVariables(string path, Calc calc)
        {
            using var writer = new StreamWriter(path);
            
            writer.WriteLine($"ans={calc.Ans}");
           
            foreach (var name in calc.ListVariables())
                writer.WriteLine($"{name}={calc.GetVariable(name)}");
           
            Console.WriteLine($"Variables saved to {path}");
        }

        public void LoadVariables(string path, Calc calc)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            foreach (var line in File.ReadAllLines(path))
            {
                var parts = line.Split('=', 2);
                if (parts.Length == 2 && double.TryParse(parts[1], out var val))
                {
                    if (parts[0] == "ans")
                    {
                        calc.Clear();
                        calc.Add(val);
                    }
                    else
                    {
                        calc.SetVariable(parts[0], val);
                    }
                }
            }
            Console.WriteLine($"Variables loaded from {path}");
        }
    }
}
