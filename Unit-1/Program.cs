using System.IO.Pipes;
using Unit_X_Common;

new Selector<Unit1Server, Unit1Client>(
    1,
    @"In unit 1, the communication is ciphered using the widely known caesar cipher method.
This unit features weak message privacy and no message authenticity."
).Start();

class Unit1Server : ServerRuntime
{
    protected override void HandleConnection(NamedPipeServerStream npss)
    {

    }

    public override string PipeName => "Unit1NamedPipe";
}
class Unit1Client : ClientRuntime
{
    protected override void HandleConnection(NamedPipeClientStream npcs)
    {

    }

    public override string PipeName => "Unit1NamedPipe";
}