using System.IO.Pipes;
using System.Security.Cryptography;
using System.Runtime.Versioning;
using Unit_X_Common;

string[] description = [
    "In unit 3, the communication is encrypted using a symmetrical encryption algorithm (AES).",
    "This unit features strong message privacy for two known devices and weak message authenticity."
];

new Selector<Unit3Server, Unit3Client>(3, description).Start();

class Unit3Server : ServerRuntime
{
    public override string PipeName => "Unit3NamedPipe";

    protected override void HandleConnection(NamedPipeServerStream npss)
    {
        log.Log("Initializing AES...");
        Aes aes = Unit3.CreateEncryptionInstance(Unit3.Key, Unit3.InitializationVector);
        log.Log("AES initialized with");
        log.Log($"\tKey: {RuntimeHelper.FormatArray(aes.Key)}");
        log.Log($"\tIV:  {RuntimeHelper.FormatArray(aes.IV)}");

        log.Log("Awaiting message from client...");
        byte[] payload = RuntimeHelper.ReadPipe(npss);

        log.Log("Received encrypted message from client.");
        log.Log($"\tEncrypted: {RuntimeHelper.FormatArray(payload)}");
        log.Log($"\tDecrypted: {Unit3.DecryptText(aes, payload)}");

        log.Log("Responding with encrypted message...");
        npss.Write(Unit3.EncryptText(aes, $"Hello from {log.GetThreadContext()} on server!"));
        log.Log("Response sent.");
    }
}
class Unit3Client : ClientRuntime
{
    public override string PipeName => "Unit3NamedPipe";

    protected override void HandleConnection(NamedPipeClientStream npcs)
    {
        log.Log("Initializing AES...");
        Aes aes = Unit3.CreateEncryptionInstance(Unit3.Key, Unit3.InitializationVector);
        log.Log("AES initialized with");
        log.Log($"\tKey: {RuntimeHelper.FormatArray(aes.Key)}");
        log.Log($"\tIV:  {RuntimeHelper.FormatArray(aes.IV)}");

        log.Log("Sending encrypted message to server...");
        npcs.Write(Unit3.EncryptText(aes, "Hello from client!"));
        log.Log("Message sent.");

        log.Log("Awaiting response from server...");

        byte[] payload = RuntimeHelper.ReadPipe(npcs);

        log.Log("Received encrypted response from server.");
        log.Log($"\tEncrypted: {RuntimeHelper.FormatArray(payload)}");
        log.Log($"\tDecrypted: {Unit3.DecryptText(aes, payload)}");
    }
}

static class Unit3
{
    public static readonly byte[] Key = [0xEB, 0x61, 0x84, 0x94, 0xDB, 0x49, 0x65, 0xAB, 0xA7, 0x9E, 0x70, 0xEB, 0x64, 0xBB, 0xFE, 0xDA, 0xDF, 0x6F, 0xA1, 0x91, 0x81, 0x7A, 0xF5, 0x16, 0x36, 0xF3, 0xE8, 0x1B, 0x6D, 0xD9, 0xBC, 0x5D];
    public static readonly byte[] InitializationVector = [0x6C, 0x83, 0x9C, 0x28, 0xED, 0xDE, 0x94, 0x8A, 0x76, 0x61, 0x66, 0x60, 0x10, 0x2B, 0xC5, 0xC3];

    public static Aes CreateEncryptionInstance(byte[] key, byte[] iv)
    {
        Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        return aes;
    }
    public static byte[] EncryptText(Aes aes, string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        ICryptoTransform encryptor = aes.CreateEncryptor();

        using MemoryStream ms = new MemoryStream();
        using (CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write))
        {
            using StreamWriter sw = new StreamWriter(cs);
            sw.Write(text);
        }

        return ms.ToArray();
    }
    public static string DecryptText(Aes aes, byte[] encrypted)
    {
        if (encrypted.Length == 0)
            throw new ArgumentNullException(nameof(encrypted));

        ICryptoTransform decryptor = aes.CreateDecryptor();

        using MemoryStream ms = new MemoryStream(encrypted);
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using StreamReader sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}

[SupportedOSPlatform("windows")]
partial class Program { }