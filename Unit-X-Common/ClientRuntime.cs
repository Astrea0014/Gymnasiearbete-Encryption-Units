using System.IO.Pipes;

namespace Unit_X_Common
{
    public abstract class ClientRuntime<TMessage> where TMessage : IMessage<TMessage>
    {
        public async void Start(string pipeServer, string pipeName)
        {
            using NamedPipeClientStream pipe = new(pipeServer, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

            pipe.Connect();
            Console.WriteLine($"Connected to server instance of pipe '{pipeName}' on device '{pipeServer}'!");

            // Greeting the server and looking for a response.
            {
                Console.WriteLine("Sending greeting...");

                await pipe.WriteAsync(GetGreetingMessage("Hello from Client!").ToBytes());

                byte[] bytes = new byte[UnitX.MaximumMessageSize];
                int size = await pipe.ReadAsync(bytes);

                TMessage message = TMessage.FromBytes(bytes, size);
                ProcessGreetingResponseMessage(message);
            }

            // Sending the client's secret message and looking for a response.
            {
                Console.WriteLine("Sending secret...");

                await pipe.WriteAsync(GetSecretMessage("Widthdraw $10 from my bank account. Bank password: 1234").ToBytes());

                byte[] bytes = new byte[UnitX.MaximumMessageSize];
                int size = await pipe.ReadAsync(bytes);

                TMessage message = TMessage.FromBytes(bytes, size);
                ProcessSecretResponseMessage(message);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            await pipe.DisposeAsync();  // Disposes of the instance and severs the connection between this
                                        // client and its server counterpart.
        }

        public abstract TMessage GetGreetingMessage(string message);
        public abstract void ProcessGreetingResponseMessage(TMessage message);

        public abstract TMessage GetSecretMessage(string message);
        public abstract void ProcessSecretResponseMessage(TMessage message);
    }
}
