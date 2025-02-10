using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_X_Common
{
    public abstract class ClientRuntime() : IPipeClientRuntime
    {
        protected readonly Logger log = new Logger();

        public string ServerName => ".";
        public abstract string PipeName { get; }

        protected abstract void HandleConnection(NamedPipeClientStream npcs);

        public void Start()
        {
            log.SetThreadContext("Main thread");

            using NamedPipeClientStream npcs = new NamedPipeClientStream(ServerName, PipeName, PipeDirection.InOut);
            npcs.Connect();

            log.Log("Connected to server; passing instance to handler.");
            HandleConnection(npcs);
        }
    }
}
