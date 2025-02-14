using System.IO.Pipes;
using System.Runtime.Versioning;
using Unit_X_Common;

string[] description = [
    "In unit 5, the communication is end-to-end encrypted using an",
    "asymmetric encryption algorithm.",
    "This unit features strong message privacy and weak message authenticity."
];

new Selector<Unit5Server, Unit5Client>(5, description).Start();

class Unit5Server : ServerRuntime
{
    public override string PipeName => "Unit5PipeName";

    protected override void HandleConnection(NamedPipeServerStream npss)
    {
        throw new NotImplementedException();
    }
}
class Unit5Client : ClientRuntime
{
    public override string PipeName => "Unit5PipeName";

    protected override void HandleConnection(NamedPipeClientStream npcs)
    {
        throw new NotImplementedException();
    }
}

[SupportedOSPlatform("windows")]
partial class Program { }