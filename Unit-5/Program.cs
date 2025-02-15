using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Security.Cryptography;
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
        log.Log("Initializing RSA...");
        using RSA myRsa = RSA.Create();
        log.Log("RSA initialized with");
        log.Log($"\tPublic key:  {RuntimeHelper.FormatArray(myRsa.ExportRSAPublicKey())}");
        log.Log($"\tPrivate key: {RuntimeHelper.FormatArray(myRsa.ExportRSAPrivateKey())}");

        log.Log("Awaiting client public key...");
        byte[] clientKey = RuntimeHelper.ReadPipe(npss);

        log.Log($"Received public key from client: {RuntimeHelper.FormatArray(clientKey)}");
        using RSA clientRsa = RSA.Create();
        clientRsa.ImportRSAPublicKey(clientKey, out int _);

        log.Log("Sending public key to client...");
        npss.Write(myRsa.ExportRSAPublicKey());
        log.Log("Public key sent.");
    }
}
class Unit5Client : ClientRuntime
{
    public override string PipeName => "Unit5PipeName";

    protected override void HandleConnection(NamedPipeClientStream npcs)
    {
        log.Log("Initializing RSA...");
        using RSA myRsa = RSA.Create();
        log.Log("RSA initialized with");
        log.Log($"\tPublic key:  {RuntimeHelper.FormatArray(myRsa.ExportRSAPublicKey())}");
        log.Log($"\tPrivate key: {RuntimeHelper.FormatArray(myRsa.ExportRSAPrivateKey())}");

        log.Log("Sending public key to server...");
        npcs.Write(myRsa.ExportRSAPublicKey());
        log.Log("Public key sent.");

        log.Log("Awaiting server public key...");
        byte[] serverKey = RuntimeHelper.ReadPipe(npcs);

        log.Log($"Received public key from server: {RuntimeHelper.FormatArray(serverKey)}");
        using RSA serverRsa = RSA.Create();
        serverRsa.ImportRSAPublicKey(serverKey, out int _);
    }
}

[SupportedOSPlatform("windows")]
partial class Program { }