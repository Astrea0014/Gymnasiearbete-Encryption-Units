using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Unit_X_Common;

string[] description = [
    "In unit 5, the communication is end-to-end encrypted using an",
    "asymmetric encryption algorithm (RSA).",
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

        log.Log("Awaiting encrypted message from client...");
        byte[] text = RuntimeHelper.ReadPipe(npss);

        log.Log("Received message from client");
        log.Log($"\tEncrypted: {RuntimeHelper.FormatArray(text)}");
        log.Log($"\tDecrypted: {Unit5.DecryptText(myRsa, text)}");

        log.Log("Respionding with encrypted message...");
        npss.Write(Unit5.EncryptText(clientRsa, $"Hello from {log.GetThreadContext()} on server!"));
        log.Log("Response sent.");
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

        log.Log("Sending encrypted message to server...");
        npcs.Write(Unit5.EncryptText(serverRsa, "Hello from client!"));
        log.Log("Message sent.");

        log.Log("Awaiting encrypted response from server...");
        byte[] text = RuntimeHelper.ReadPipe(npcs);

        log.Log("Received encrypted response from server");
        log.Log($"\tEncrypted: {RuntimeHelper.FormatArray(text)}");
        log.Log($"\tDecrypted: {Unit5.DecryptText(myRsa, text)}");
    }
}

static class Unit5
{
    public static byte[] EncryptText(RSA rsa, string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        return rsa.Encrypt(Encoding.UTF8.GetBytes(text), RSAEncryptionPadding.Pkcs1);
    }
    public static string DecryptText(RSA rsa, byte[] encrypted)
        => Encoding.UTF8.GetString(rsa.Decrypt(encrypted, RSAEncryptionPadding.Pkcs1));
}

[SupportedOSPlatform("windows")]
partial class Program { }