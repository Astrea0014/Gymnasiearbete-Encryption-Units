namespace Unit_X_Common
{
    public interface IMessage<T> where T : IMessage<T>
    {
        public abstract byte[] ToBytes();
        public static abstract T FromBytes(byte[] bytes, int size);
        public static abstract T FromBytes(byte[] bytes);
    }

    namespace Unit1
    {
        public enum MessageType : byte
        {
            ServerResponse,
            Greeting,
            Secret
        }

        public struct Message : IMessage<Message>
        {
            public const int MaxSize = 4096;

            public MessageType MessageType;
            public byte[] Payload;

            public readonly byte[] ToBytes() => [(byte)MessageType, .. Payload];

            public static Message FromBytes(byte[] bytes, int size)
            {
                return new Message()
                {
                    MessageType = (MessageType)bytes[0],
                    Payload = bytes[1..size]
                };
            }
            public static Message FromBytes(byte[] bytes) => FromBytes(bytes, bytes.Length);
        }
    }

    namespace Unit2
    {
        public enum MessageType : byte
        {
            ServerResponse,
            Greeting,
            Secret
        }

        public struct Message : IMessage<Message>
        {
            public const int MaxSize = 4096;

            public MessageType MessageType;
            public byte[] Payload;

            public readonly byte[] ToBytes() => [(byte)MessageType, .. Payload];

            public static Message FromBytes(byte[] bytes, int size)
            {
                return new Message()
                {
                    MessageType =(MessageType)bytes[0],
                    Payload = bytes[1..size]
                };
            }
            public static Message FromBytes(byte[] bytes) => FromBytes(bytes, bytes.Length);
        }
    }

    namespace Unit3
    {
        public enum MessageType : byte
        {
            ServerResponse,
            Greeting,
            Secret
        }

        public struct Message : IMessage<Message>
        {
            public const int MaxSize = 4096;

            public MessageType MessageType;
            public int EncryptedLength;
            public byte[] Encrypted;

            public readonly byte[] ToBytes() => [(byte)MessageType, .. BitConverter.GetBytes(EncryptedLength), .. Encrypted];
            public static Message FromBytes(byte[] bytes, int size)
            {
                return new Message()
                {
                    MessageType = (MessageType)bytes[0],
                    EncryptedLength = BitConverter.ToInt32(bytes, 1),
                    Encrypted = bytes[5..]
                };
            }

            public static Message FromBytes(byte[] bytes) => FromBytes(bytes, bytes.Length);
        }
    }
}
