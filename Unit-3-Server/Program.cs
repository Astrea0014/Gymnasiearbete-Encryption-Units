using Unit_X_Common.Unit3;

byte[] encrypted = Unit.EncryptString("Hello world!");

Console.Write('[');
for (int i = 0; i < encrypted.Length; i++)
{
    Console.Write($"0x{encrypted[i]:X}");

    if (i == encrypted.Length - 1)
        Console.WriteLine(']');
    else
        Console.Write(", ");
}

Console.WriteLine($"Decrypted: {Unit.DecryptString(encrypted)}");