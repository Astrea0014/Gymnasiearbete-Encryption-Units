using System.Text;
using System.Security.Cryptography;

namespace Unit_X_Common
{
    public static class UnitX
    {
        public const int MaximumMessageSize = 4096;

        public static readonly string ServerName = ".";
        public static readonly Encoding TextEncoding = Encoding.Unicode;

        public static string FormatByteArray(byte[] bytes, int offset, int length)
        {
            string ret = "[";
            foreach (byte b in bytes[offset..(offset + length - 2)])
                ret += $"0x{b:X2}, ";
            return ret + $"0x{bytes[offset + length - 1]:X2}]";
        }
        public static string FormatByteArrayAsHexString(byte[] bytes, int offset, int length)
        {
            string ret = "";
            foreach (byte b in bytes[offset..(offset + length)])
                ret += $"{b:2X}";
            return ret;
        }
    }

    namespace Unit1
    {
        /// <summary>
        /// Unit 1 features no guarantee for data security, privacy or authenticity.
        /// </summary>
        public static class Unit
        {
            public static readonly string PipeName = "Unit1Pipe";
            public static readonly string ServerName = UnitX.ServerName;
            public static readonly Encoding TextEncoding = UnitX.TextEncoding;
        }
    }

    namespace Unit2
    {
        /// <summary>
        /// Unit 2 features a basic guarantee for data privacy and security but features no guarantee for data authenticity.
        /// </summary>
        public static class Unit
        {
            public static readonly string PipeName = "Unit2Pipe";
            public static readonly string ServerName = UnitX.ServerName;
            public static readonly Encoding TextEncoding = UnitX.TextEncoding;

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

    namespace Unit3
    {
        /// <summary>
        /// <para>
        /// Unit 3 features strong guarantee for data privacy and security for devices
        /// that already know each other, but features no guarantee for data authenticity.
        /// </para>
        /// <para>
        /// It accomplishes this by using a fully symmetrical encryption algorithm like AES.
        /// </para>
        /// </summary>
        public static class Unit
        {
            public static readonly string PipeName = "Unit3Pipe";
            public static readonly string ServerName = UnitX.ServerName;
            public static readonly Encoding TextEncoding = UnitX.TextEncoding;

            public static readonly byte[] AesKey = [0xEB, 0x61, 0x84, 0x94, 0xDB, 0x49, 0x65, 0xAB, 0xA7, 0x9E, 0x70, 0xEB, 0x64, 0xBB, 0xFE, 0xDA, 0xDF, 0x6F, 0xA1, 0x91, 0x81, 0x7A, 0xF5, 0x16, 0x36, 0xF3, 0xE8, 0x1B, 0x6D, 0xD9, 0xBC, 0x5D];
            public static readonly byte[] AesInitializationVector = [0x6C, 0x83, 0x9C, 0x28, 0xED, 0xDE, 0x94, 0x8A, 0x76, 0x61, 0x66, 0x60, 0x10, 0x2B, 0xC5, 0xC3];

            public static byte[] EncryptString(string text)
            {
                ArgumentException.ThrowIfNullOrEmpty(text, nameof(text));

                byte[] encrypted;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = AesKey;
                    aes.IV = AesInitializationVector;

                    ICryptoTransform encryptor = aes.CreateEncryptor();

                    using MemoryStream ms = new();
                    using (CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using StreamWriter sw = new(cs);
                        sw.Write(text);
                    }

                    encrypted = ms.ToArray();
                }

                return encrypted;
            }

            public static string DecryptString(byte[] encrypted, int length)
            {
                if (encrypted == null || encrypted.Length <= 0)
                    throw new ArgumentNullException(nameof(encrypted));

                string decrypted;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = AesKey;
                    aes.IV = AesInitializationVector;

                    ICryptoTransform decryptor = aes.CreateDecryptor();

                    using MemoryStream ms = new(encrypted, 0, length);
                    using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
                    using StreamReader sr = new(cs);
                    decrypted = sr.ReadToEnd();
                }

                return decrypted;
            }
        }
    }

    public class RsaKeypair
    {
        public byte[] Public = [];
        public byte[] Private = [];
    }

    namespace Unit4
    {
        /// <summary>
        /// <para>
        /// Unit 4 features no guarantee for data privacy, but features a very strong guarantee for data authenticity.
        /// </para>
        /// <para>
        /// It does this by using an asymmetric algorithm (RSA with SHA-256) to create a digital signature derived from a private key,
        /// that can later be verified using the respective public key.
        /// </para>
        /// </summary>
        public static class Unit
        {
            public static readonly string PipeName = "Unit4Pipe";
            public static readonly string ServerName = UnitX.ServerName;
            public static readonly Encoding TextEncoding = UnitX.TextEncoding;

            public static readonly HashAlgorithmName AlgorithmName = HashAlgorithmName.SHA256;
            public static readonly RSASignaturePadding SignaturePadding = RSASignaturePadding.Pkcs1;

            public static byte[] SignString(byte[] encoded, RsaKeypair keypair)
            {
                if (encoded == null || encoded.Length <= 0)
                    throw new ArgumentNullException(nameof(encoded));

                byte[] signature;

                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportRSAPrivateKey(keypair.Private, out int _); // Importing the private key to perform the hashing operation.
                    signature = rsa.SignData(encoded, AlgorithmName, SignaturePadding);
                }

                return signature;
            }

            public static bool VerifyString(byte[] encoded, byte[] signature, RsaKeypair keypair)
            {
                if (encoded == null || encoded.Length <= 0)
                    throw new ArgumentNullException(nameof(encoded));

                using RSA rsa = RSA.Create();
                rsa.ImportRSAPublicKey(keypair.Public, out int _);  // Importing the public key to perform the verification operation.
                return rsa.VerifyData(encoded, signature, AlgorithmName, SignaturePadding);
            }
        }
    }

    namespace Unit5
    {
        public static class Unit
        {
            public static readonly string PipeName = "Unit5Pipe";
            public static readonly string ServerName = UnitX.ServerName;
            public static readonly Encoding TextEncoding = UnitX.TextEncoding;

            public static readonly RSAEncryptionPadding EncryptionPadding = RSAEncryptionPadding.OaepSHA512;

            public static byte[] EncryptString(byte[] encoded, RsaKeypair keypair)
            {
                if (encoded == null || encoded.Length <= 0)
                    throw new ArgumentNullException(nameof(encoded));

                using RSA rsa = RSA.Create();
                rsa.ImportRSAPublicKey(keypair.Public, out int _);
                return rsa.Encrypt(encoded, EncryptionPadding);
            }
            public static byte[] DecryptString(byte[] encrypted, RsaKeypair keypair)
            {
                if (encrypted == null || encrypted.Length <= 0)
                    throw new ArgumentNullException(nameof(encrypted));

                using RSA rsa = RSA.Create();
                rsa.ImportRSAPrivateKey(keypair.Private, out int _);
                return rsa.Decrypt(encrypted, EncryptionPadding);
            }
        }
    }
}
