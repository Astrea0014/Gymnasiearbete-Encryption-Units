using System.IO.Pipes;
using System.Runtime.Versioning;

namespace Unit_X_Common
{
    public abstract class ThreadPool
    {
        private Thread[] _pool;
        private NamedPipeServerStream?[] _workItems;

        protected Logger log;

        public bool IsRunning { get; private set; }
        
        public ThreadPool(int workerCount)
        {
            log = new Logger();
            log.SetThreadContext("Main thread");

            _pool = new Thread[workerCount];

            for (int i = 0; i < workerCount; i++)
            {
                _pool[i] = new Thread(Worker);
                _pool[i].Start(i);
            }

            _workItems = new NamedPipeServerStream?[workerCount];
        }

        public void Stop() => IsRunning = false;

        protected abstract void HandleConnection(NamedPipeServerStream npss);

        public void Worker(object? param)
        {
            int workerIndex = param as int? ?? throw new ArgumentNullException(nameof(param));
            log.SetThreadContext($"Worker #{workerIndex}");

            while (IsRunning)
            {
                if (_workItems[workerIndex] is not null)
                {
                    HandleConnection(_workItems[workerIndex]!);
                    log.Log("Connection handled successfully.");
                }

                Thread.Sleep(50);
            }
        }
    }

    public abstract class ServerRuntime() : ThreadPool(64), IPipeRuntime
    {
        private readonly List<Task> _connections = [];

        public abstract string PipeName { get; }


        [SupportedOSPlatform("windows")]
        public void Start()
        {
            Log("Started server");

            while (IsRunning)
            {
                NamedPipeServerStream npss = new NamedPipeServerStream(PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message);

                Log("Awaiting client connection...");
                npss.WaitForConnection();
            }
        }
    }
}
