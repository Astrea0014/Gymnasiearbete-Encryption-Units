using System.IO.Pipes;
using System.Runtime.Versioning;

namespace Unit_X_Common
{
    public abstract class ThreadPool
    {
        private readonly Thread[] _pool;
        private readonly NamedPipeServerStream?[] _workItems;
        private readonly Queue<NamedPipeServerStream> _queuedItems;

        protected Logger log;

        private int GetFirstFreeWorkerIndex()
        {
            for (int i = 0; i < _workItems.Length; i++)
                if (_workItems[i] == null)
                    return i;
            return -1;
        }

        private void AcceptNewWork(int workerIndex)
        {
            if (_queuedItems.Count == 0)
                return;
            if (_workItems[workerIndex] == null)
                return;

            _workItems[workerIndex] = _queuedItems.Dequeue();
        }

        private void Worker(object? param)
        {
            int workerIndex = param as int? ?? throw new ArgumentNullException(nameof(param));
            log.SetThreadContext($"Worker #{workerIndex}");

            while (IsRunning)
            {
                if (_workItems[workerIndex] is not null)
                {
                    log.Log("Received work item; calling HandleConnection");
                    using (_workItems[workerIndex])
                    {
                        HandleConnection(_workItems[workerIndex]!);
                        _workItems[workerIndex]!.Disconnect();
                    }
                    log.Log("Connection handled successfully.");
                    AcceptNewWork(workerIndex);
                }

                Thread.Sleep(50);
            }
        }

        protected void EnqueueWork(NamedPipeServerStream npss)
        {
            int i = GetFirstFreeWorkerIndex();

            if (i != -1)
                _workItems[i] = npss;
            else
                _queuedItems.Enqueue(npss);
        }

        protected abstract void HandleConnection(NamedPipeServerStream npss);

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
            _queuedItems = [];
        }

        public void Stop() => IsRunning = false;
    }

    public abstract class ServerRuntime() : ThreadPool(32), IPipeRuntime
    {
        public abstract string PipeName { get; }

        [SupportedOSPlatform("windows")]
        public void Start()
        {
            log.SetThreadContext("Main thread");
            log.Log("Started server");

            while (IsRunning)
            {
                NamedPipeServerStream npss = new NamedPipeServerStream(PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message);

                log.Log("Awaiting client connection...");
                npss.WaitForConnection();

                log.Log("Connection received! Passing to threadpool worker...");
                EnqueueWork(npss);
            }
        }
    }
}
