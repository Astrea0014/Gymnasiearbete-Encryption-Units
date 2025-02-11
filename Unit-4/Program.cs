using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Unit_X_Common;

string[] description = [
    "In unit 4, the communication is completely unencrypted, but signed using a",
    "digital signature generated from an asymmetric algorithm.",
    "This unit features no message privacy and strong messatge authenticity."
];

new Selector<Unit4Server, Unit4Client>(4, description).Start();

class Unit4Server : ServerRuntime
{
    public override string PipeName => "Unit4NamedPipe";

    protected override void HandleConnection(NamedPipeServerStream npss)
    {
        log.Log("Initializing RSA...");
        using RSA myRsa = RSA.Create();
        log.Log("RSA initialized with");
        log.Log($"\tPublic key:  {RuntimeHelper.FormatArray(myRsa.ExportRSAPublicKey())}");
        log.Log($"\tPrivate key: {RuntimeHelper.FormatArray(myRsa.ExportRSAPrivateKey())}");

        // Not announced because formally this information should be fetched from a CA,
        // not from the client directly.

        byte[] clientKey = RuntimeHelper.ReadPipe(npss);
        using RSA clientRsa = RSA.Create();
        clientRsa.ImportRSAPublicKey(clientKey, out int _);

        log.Log("Awaiting message from client...");
        byte[] text = RuntimeHelper.ReadPipe(npss);

        log.Log($"Received message from client: {Encoding.UTF8.GetString(text)}");

        log.Log("Awaiting message signature from client...");
        byte[] signature = RuntimeHelper.ReadPipe(npss);

        log.Log($"Received digital signature from client: {RuntimeHelper.FormatArray(signature)}");
        if (clientRsa.VerifyData(text, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
            log.Log("Data signature is valid. Data is authored by the client.");
        else
        {
            log.Log("Data signature is not valid. Data is not authored by the client.");
            log.Log("Aborting operations...");
            return;
        }

        log.Log("Responding with signed message...");

        npss.Write(myRsa.ExportRSAPublicKey());
        byte[] myText = Encoding.UTF8.GetBytes("Hello from ");
        npss.Write(myText);
        npss.Write(myRsa.SignData(myText, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));

        log.Log("Response sent.");
    }
}
class Unit4Client : ClientRuntime
{
    public override string PipeName => "Unit4NamedPipe";

    protected override void HandleConnection(NamedPipeClientStream npcs)
    {
        log.Log("Initializing RSA...");
        using RSA myRsa = RSA.Create();
        log.Log("RSA initialized with");
        log.Log($"\tPublic key:  {RuntimeHelper.FormatArray(myRsa.ExportRSAPublicKey())}");
        log.Log($"\tPrivate key: {RuntimeHelper.FormatArray(myRsa.ExportRSAPrivateKey())}");

        log.Log("Sending signed message to server...");

        npcs.Write(myRsa.ExportRSAPublicKey());
        byte[] myText = Encoding.UTF8.GetBytes("Hello from ");
        npcs.Write(myText);
        npcs.Write(myRsa.SignData(myText, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));

        log.Log("Message sent.");

        // Not announced because formally this information should be fetched from a CA,
        // not from the client directly.

        byte[] serverKey = RuntimeHelper.ReadPipe(npcs);
        using RSA serverRsa = RSA.Create();
        serverRsa.ImportRSAPublicKey(serverKey, out int _);

        log.Log("Awaiting response from server...");
        byte[] text = RuntimeHelper.ReadPipe(npcs);

        log.Log($"Received response from server: {Encoding.UTF8.GetString(text)}");

        log.Log("Awaiting response signature from server...");
        byte[] signature = RuntimeHelper.ReadPipe(npcs);

        log.Log($"Received digital signature from server: {RuntimeHelper.FormatArray(signature)}");
        if (serverRsa.VerifyData(text, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
            log.Log("Data signature is valid. Data is authored by the server.");
        else
        {
            log.Log("Data signature is not valid. Data is not authored by the server.");
            log.Log("Aborting operations...");
            return;
        }
    }
}

[SupportedOSPlatform("windows")]
partial class Program { }