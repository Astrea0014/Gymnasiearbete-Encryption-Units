using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Text;
using Unit_X_Common;

string[] description = [
    "In unit 1, the communication unencrypted and unauthenticated.",
    "This unit features no message privacy nor authenticity."
];

new Selector<Unit1Server, Unit1Client>(1, description).Start();

class Unit1Server : ServerRuntime
{
    public override string PipeName => "Unit1NamedPipe";

    protected override void HandleConnection(NamedPipeServerStream npss)
    {
        log.Log("Awaiting message from client...");

        byte[] payload = RuntimeHelper.ReadPipe(npss);

        log.Log($"Received message from client: {Encoding.UTF8.GetString(payload)}");

        log.Log("Responding...");
        npss.Write(Encoding.UTF8.GetBytes($"Hello from {log.GetThreadContext()} on server!"));
        log.Log("Response sent.");
    }
}
class Unit1Client : ClientRuntime
{
    public override string PipeName => "Unit1NamedPipe";

    protected override void HandleConnection(NamedPipeClientStream npcs)
    {
        log.Log("Sending message to server...");
        npcs.Write(Encoding.UTF8.GetBytes("Hello from client!"));
        log.Log("Message sent.");

        log.Log("Awaiting response from server...");

        byte[] payload = RuntimeHelper.ReadPipe(npcs);

        log.Log($"Received response from server: {Encoding.UTF8.GetString(payload)}");
    }
}

[SupportedOSPlatform("windows")]
public partial class Program { }