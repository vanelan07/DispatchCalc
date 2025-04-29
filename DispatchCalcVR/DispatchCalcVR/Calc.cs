using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DispatchCalcVR
{
    /// <summary>
    /// Calculator that maintains state (ans), performs calculations,
    /// manages named variables, undo history, and computes Fibonacci numbers.
    /// Implements:
    /// Add, Subtract, Multiply, Divide, Mod, Power
    /// Persistent state using Ans
    /// Clear state
    /// Save/Load variables with lowercase only names
    /// Friendly errors for unknown variables
    /// Undo last operation
    /// Compute n-th Fibonacci
    /// </summary>
    public class Calc
    {
        private readonly Dictionary<string, double> variables = new();
        private readonly Stack<double> undoStack = new();
        private double ans = 0;
        public double Ans => ans;

        private void PushUndo() => undoStack.Push(ans);
        public void Add(double x) 
        { 
            PushUndo(); ans += x; 
        }

        public void Subtract(double x) 
        { 
            PushUndo(); ans -= x; 
        }

        public void Multiply(double x) 
        { 
            PushUndo(); ans *= x; 
        }

        public void Divide(double x) 
        { 
            PushUndo(); ans /= x; 
        }

        public void Mod(double x) 
        { 
            PushUndo(); ans %= x; 
        }

        public void Power(double x) 
        { 
            PushUndo(); ans = Math.Pow(ans, x);
        }

        public void Clear() 
        { 
            PushUndo(); ans = 0; 
        }

        public bool Undo() 
        { 
            if (undoStack.Count == 0) 
                return false; 
                
            ans = undoStack.Pop(); 
                return true; 
        }

        public void SaveVariable(string name)
        {
            if (!Regex.IsMatch(name, "^[a-z]+$"))
                throw new ArgumentException("Invalid variable name. Use lowercase letters only.");
            variables[name] = ans;
        }

        public void LoadVariable(string name)
        {
            if (!variables.ContainsKey(name))
                throw new KeyNotFoundException($"Unknown variable '{name}'.");
            
            PushUndo();
            ans = variables[name];
        }

        public void DeleteVariable(string name)
        {
            if (!variables.Remove(name))
                throw new KeyNotFoundException($"Unknown variable '{name}'.");
        }

        public double GetVariable(string name)
        {
            if (!variables.ContainsKey(name))
                throw new KeyNotFoundException($"Unknown variable '{name}'.");
            
            return variables[name];
        }

        public void SetVariable(string name, double value) => variables[name] = value;

        public IEnumerable<string> ListVariables() => variables.Keys;

        private double ComputeFibonacci(int n)
        {
            if (n < 0) throw new ArgumentException("Fibonacci index must be non-negative.");
            if (n < 2) return n;

            double a = 0, b = 1;

            for (int i = 2; i <= n; i++)
            {
                double t = a + b;
                a = b;
                b = t;
            }

            return b;
        }

        public void Fibonacci()
        {
            int n = (int)ans;
            PushUndo();
            ans = ComputeFibonacci(n);
        }
    }



}
