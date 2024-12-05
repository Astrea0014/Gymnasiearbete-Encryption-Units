using System.IO.Pipes;
using System.Runtime.Versioning;

namespace Unit_X_Common
{
    public abstract class ServerRuntime<TMessage> where TMessage : IMessage<TMessage>
    {
        [SupportedOSPlatform("windows")]
        public async void Start(string pipeName)
        {
            using NamedPipeServerStream pipe = new(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            Console.WriteLine($"Created instance of named pipe '{pipeName}'");

            pipe.WaitForConnection();
            Console.WriteLine($"Connected to pipe client. Awaiting contact...");

            CancellationTokenSource cts = new();
            CancellationToken ct = cts.Token;

            Task readTask = Task.Run(async () =>
            {
                ct.ThrowIfCancellationRequested();

                while (!ct.IsCancellationRequested)
                {
                    byte[] bytes = new byte[UnitX.MaximumMessageSize];
                    int size = await pipe.ReadAsync(bytes, ct);             // Call to PipeStream.ReadAsync is awaited until
                                                                            // either a message is sent through the pipe, or
                                                                            // the operation is cancelled.
                    TMessage message = TMessage.FromBytes(bytes, size);
                    LogIncomingMessage(message);

                    switch (message.MessageType)
                    {
                        case MessageType.Greeting:
                            TMessage greeting = GetGreetingResponseMessage("Hello from Server!");
                            Console.WriteLine("Greeting the client back...");
                            await pipe.WriteAsync(greeting.ToBytes(), ct);
                            break;
                        case MessageType.Secret:
                            TMessage secret = GetSecretResponseMessage(message, "Widthdrew $10 from your bank account with password: 1234");
                            Console.WriteLine("Updating the client about their requested task...");
                            await pipe.WriteAsync(secret.ToBytes(), ct);
                            break;
                    }
                }
            }, ct);

            while (pipe.IsConnected && !readTask.IsCompleted) ; // Wait for the pipe client to disconnect or for the task to stop.

            cts.Cancel();                                       // Cancel read task for redundancy.

            try { await readTask; }                             // Propagate any pending exceptions thrown in the task by awaiting it.
            catch { }

            cts.Dispose();
            pipe.Disconnect();
        }

        public abstract void LogIncomingMessage(TMessage message);
        public abstract TMessage GetGreetingResponseMessage(string message);
        public abstract TMessage GetSecretResponseMessage(TMessage incoming, string outgoing);
    }
}
