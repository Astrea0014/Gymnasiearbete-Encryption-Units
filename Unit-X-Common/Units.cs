using System.Text;

namespace Unit_X_Common
{
    namespace UnitX
    {
        public static class Unit
        {
            public static readonly string ServerName = "DLSCHOOL";
            public static readonly Encoding TextEncoding = Encoding.Unicode;
        }
    }

    namespace Unit1
    {
        public static class Unit
        {
            public static readonly string PipeName = "Unit1Pipe";
            public static readonly string ServerName = UnitX.Unit.ServerName;
            public static readonly Encoding TextEncoding = UnitX.Unit.TextEncoding;
        }
    }

    namespace Unit2
    {
        public static class Unit
        {
            public static readonly string PipeName = "Unit2Pipe";
            public static readonly string ServerName = UnitX.Unit.ServerName;
            public static readonly Encoding TextEncoding = UnitX.Unit.TextEncoding;

            public const int CipherShift = 12;
            // To increase the obfuscation of the set it can theoretically be mangled and scrambled. However, this does not represent what Julius Caesar originally did.
            public static readonly string[] CipherTranslationSets = ["ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ", "abcdefghijklmnopqrstuvwxyzåäö", "1234567890", "!\"@#£¤$%&/{([)]=}?+\\<>|.:,;-_'*¨~^§"];

            public static string Cipher(string text, int shift)
            {
                string ret = string.Empty;

                foreach (char c in text)
                {
                    bool processed = false;

                    foreach (string translation in CipherTranslationSets)
                    {
                        int cIndex = translation.IndexOf(c);                            // Index of string character in translation set.
                        if (cIndex == -1)                                               // If the character is not featured in the set,
                            continue;                                                   // continue to the next set.

                        int translationIndex = (cIndex + shift) % translation.Length;   // Temporary for storing a value.
                        if (translationIndex < 0)                                       // Make sure index is not negative
                            translationIndex += translation.Length;                     // by making it wrap around.

                        ret += translation[translationIndex];                           // Add the translated character onto the return.
                        processed = true;                                               // Indicate that the character could be processed.
                        break;
                    }

                    if (!processed)                                                     // If the character could not be processed
                                                                                        // (e.g. it is not listed in the cipher sets),
                        ret += c;                                                       // add it as a plain character instead.
                }

                return ret;
            }
            public static string Decipher(string text, int shift) => Cipher(text, -shift); // Valid since negative shifts undo positive shifts.
        }
    }
}
