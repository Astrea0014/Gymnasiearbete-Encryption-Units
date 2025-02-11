using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_X_Common
{
    public class Logger
    {
        private readonly Dictionary<int, string> _contexts = [];

        public bool HasThreadContext() => _contexts.ContainsKey(Environment.CurrentManagedThreadId);
        public string GetThreadContext() => _contexts[Environment.CurrentManagedThreadId];
        public void SetThreadContext(string name) => _contexts[Environment.CurrentManagedThreadId] = name;

        public void Log(string message) => Console.WriteLine($"{(HasThreadContext() ? $"[{GetThreadContext()}] " : "")}{message}");
    }
}
