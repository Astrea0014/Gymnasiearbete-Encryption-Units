using System.Security.Cryptography;

namespace Unit_X_Common
{
    public enum MessageType : byte
    {
        ServerResponse,
        Greeting,
        Secret
    }

    public interface IMessage<T> where T : IMessage<T>
    {
        public MessageType MessageType { get; set; }

        public byte[] ToBytes();
        public static abstract T FromBytes(byte[] bytes, int size);
        public static abstract T FromBytes(byte[] bytes);
    }

    namespace Unit1
    {
        public struct Message : IMessage<Message>
        {
            public MessageType MessageType { get; set; }
            public byte[] Payload;

            public readonly byte[] ToBytes() => [(byte)MessageType, .. Payload];

            public static Message FromBytes(byte[] bytes, int size) => new()
            {
                MessageType = (MessageType)bytes[0],
                Payload = bytes[1..size]
            };
            public static Message FromBytes(byte[] bytes) => FromBytes(bytes, bytes.Length);
        }
    }

    namespace Unit2
    {
        public struct Message : IMessage<Message>
        {
            public MessageType MessageType { get; set; }
            public byte[] Payload;

            public readonly byte[] ToBytes() => [(byte)MessageType, .. Payload];

            public static Message FromBytes(byte[] bytes, int size) => new()
            {
                MessageType = (MessageType)bytes[0],
                Payload = bytes[1..size]
            };
            public static Message FromBytes(byte[] bytes) => FromBytes(bytes, bytes.Length);
        }
    }

    namespace Unit3
    {
        public struct Message : IMessage<Message>
        {
            public MessageType MessageType { get; set; }
            public int EncryptedLength;
            public byte[] Encrypted;

            public readonly byte[] ToBytes() => [(byte)MessageType, .. BitConverter.GetBytes(EncryptedLength), .. Encrypted];
            public static Message FromBytes(byte[] bytes, int size) => new()
            {
                MessageType = (MessageType)bytes[0],
                EncryptedLength = BitConverter.ToInt32(bytes, 1),
                Encrypted = bytes[5..size]
            };

            public static Message FromBytes(byte[] bytes) => FromBytes(bytes, bytes.Length);
        }
    }

    namespace Unit4
    {
        public struct Message : IMessage<Message>
        {
            public const int SignatureSize = 256 / 8;

            public MessageType MessageType { get; set; }
            public int RSAPublicKeyLength;
            public byte[] Security;
            public byte[] Encoded;

            public readonly byte[] ToBytes() => [(byte)MessageType, .. BitConverter.GetBytes(RSAPublicKeyLength), .. Security, .. Encoded];
            public static Message FromBytes(byte[] bytes, int size)
            {
                int security = BitConverter.ToInt32(bytes, 1);  // The RSAPublicKeyLength field, which dictates
                                                                // what the Security field contains.
                                                                // If RSAPublicKeyLength == 0, the Security field contains
                                                                // a signature. If it is not null, it contains the other
                                                                // entity's corresponding public key.

                int securityOffset = (security != 0 ? security : SignatureSize) + 5;

                return new Message()
                {
                    MessageType = (MessageType)bytes[0],
                    RSAPublicKeyLength = security,
                    Security = bytes[5..securityOffset],
                    Encoded = bytes[securityOffset..size]
                };
            }

            public static Message FromBytes(byte[] bytes) => FromBytes(bytes, bytes.Length);
        }
    }
}
