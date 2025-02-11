using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Text;
using Unit_X_Common;

string[] description = [
    "In unit 2, the communication is ciphered using the widely known caesar cipher method.",
    "This unit features weak message privacy and no message authenticity."
];

new Selector<Unit2Server, Unit2Client>(2, description).Start();

class Unit2Server : ServerRuntime
{
    public override string PipeName => "Unit2NamedPipe";

    protected override void HandleConnection(NamedPipeServerStream npss)
    {
        log.Log("Awaiting message from client...");

        byte[] payload = RuntimeHelper.ReadPipe(npss);

        string payloadText = Encoding.UTF8.GetString(payload);
        log.Log("Received message from client:");
        log.Log($"\tCiphered:   {payloadText}");
        log.Log($"\tDeciphered: {Unit2.CaesarDecipherText(payloadText, 3)}");

        log.Log("Responding...");
        npss.Write(Encoding.UTF8.GetBytes(Unit2.CaesarCipherText($"Hello from {log.GetThreadContext()} on server!", 3)));
        log.Log("Response sent.");
    }
}
class Unit2Client : ClientRuntime
{
    public override string PipeName => "Unit2NamedPipe";

    protected override void HandleConnection(NamedPipeClientStream npcs)
    {
        log.Log("Sending message to server...");
        npcs.Write(Encoding.UTF8.GetBytes(Unit2.CaesarCipherText("Hello from client!", 3)));
        log.Log("Message sent.");

        log.Log("Awaiting response from server...");

        byte[] payload = RuntimeHelper.ReadPipe(npcs);

        string payloadText = Encoding.UTF8.GetString(payload);
        log.Log("Received response from server.");
        log.Log($"\tCiphered:   {payloadText}");
        log.Log($"\tDeciphered: {Unit2.CaesarDecipherText(payloadText, 3)}");
    }
}

static class Unit2
{
    public static readonly string[] CaesarCipherTranslationSets = [
        "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ",
        "abcdefghijklmnopqrstuvwxyzåäö",
        "1234567890",
        $@"!{'"'}@#£¤$€%&/{'{'}{'}'}([)]=?+\<>|.:,;-_'*¨~^§"
    ];

    public static string CaesarCipherText(string input, int shift)
    {
        string ret = string.Empty;

        foreach (char c in input)
        {
            bool processed = false;

            foreach (string translation in CaesarCipherTranslationSets)
            {
                int cIndex = translation.IndexOf(c);    // Fetches index of string character in
                                                        // translation set.
                if (cIndex == -1)                       // If the character is not featured in
                                                        // the translation set;
                    continue;                           // continue to the next set.

                // DESIGN NOTE:
                // 'translation.Length' is added before the modulo operation to deal with edge cases.
                // An example of such an edge case is when the quantity of 'cIndex' is smaller than
                // the quantity of 'shift' and 'shift' has a negative sign. With traditional modulo,
                // this would not be a problem. However, as a consequence of how the remainder
                // operator is implemented, this will produce a negative result. For example; 
                // with traditional modulo, -4 mod 25 = 21, whereas with the remainder operator,
                // -4 % 25 = -4.
                //
                // By adding translation.Length before the remainder operation, we ensure
                // consistent behavior since if cIndex + shift ends up being positive, the
                // addition will be delt with by the remainder operation. E.g;
                // (-4 + 25) % 25 = 21 % 25 = 21, (4 + 25) % 25 = 29 % 25 = 4
                int translationIndex =                  // Calculates the index of the
                    (cIndex                             // corresponding shifted character
                    + shift                             // in the matching translation set.
                    + translation.Length)
                    % translation.Length;

                ret += translation[translationIndex];   // Add the translated character onto
                                                        // the return.
                processed = true;                       // Indicate that the character could
                                                        // be properly processed.
                break;
            }

            if (!processed)                             // If the character could not be
                                                        // processed (e.g. it is not listed
                                                        // in any translation set);
                ret += c;                               // add it onto the return as is.
        }

        return ret;
    }

    public static string CaesarDecipherText(string input, int shift)
        => CaesarCipherText(input, -shift);
}

[SupportedOSPlatform("windows")]
public partial class Program { }