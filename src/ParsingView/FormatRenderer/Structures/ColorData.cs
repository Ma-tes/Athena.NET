using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.ParsingView.FormatRenderer.Structures;

public readonly struct ColorData
{
    private const int HexCharacterMinimum = 65;
    private const int HexCharacterMaximum = 70;

    private static readonly int hexDataLength = 6;

    public byte RedValue { get; }
    public byte GreenValue { get; }
    public byte BlueValue { get; }

    public ColorData(byte redValue, byte greenValue,
        byte blueValue)
    {
        RedValue = redValue;
        GreenValue = greenValue;
        BlueValue = blueValue;
    }

    public static ColorData CreateFromRange(Span<byte> colorBytes) =>
        new(colorBytes[0], colorBytes[1], colorBytes[2]);

    public static bool TryCreateFromHexColorData([NotNullWhen(true)]out ColorData returnData, string hexValue)
    {
        int hexValueLength = hexValue.Length;
        if (hexValueLength != hexDataLength)
            return NullableHelper.NullableOutValue(out returnData);

        int valuePairsLength = hexDataLength / 2;
        Span<byte> returnBytesRange = stackalloc byte[valuePairsLength];
        for (int i = 0; i < valuePairsLength; i++)
        {
            int relativeHexIndex = i * 2;
            if(!TryGetRelativeHexIndex(out int leftIndex, hexValue[relativeHexIndex]) ||
                !TryGetRelativeHexIndex(out int rightIndex, hexValue[relativeHexIndex + 1]))
                     return NullableHelper.NullableOutValue(out returnData);
            returnBytesRange[i] = CalculateRelativeColorIndex(leftIndex, rightIndex);
        }
        returnData = CreateFromRange(returnBytesRange);
        return true;
    }

    private static bool TryGetRelativeHexIndex(out int returnIndex, char hexCharacter)
    {
        if (hexCharacter >= HexCharacterMinimum
            && hexCharacter <= HexCharacterMaximum)
        {
            returnIndex = 10 + (hexCharacter - HexCharacterMinimum);
            return true;
        }
        return int.TryParse(hexCharacter.ToString(), out returnIndex);
    }

    private static byte CalculateRelativeColorIndex(int firstHexIndex, int secondHexIndex) =>
        (byte)((firstHexIndex << 4) | secondHexIndex);
}
