#pragma warning disable IDE0130
namespace RunaString.RegularExpressions;
#pragma warning restore IDE0130

internal class HexConverter
{
    private const byte _x_ = 0xFF;

    public static ReadOnlySpan<byte> CharToHexLookup => [
            _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_,
            _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_,
            _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_,
            0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, _x_, _x_, _x_, _x_, _x_, _x_,
            _x_, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_,
            _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_,
            _x_, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_,
            _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_, _x_,
        ];

    public static int FromChar(int runeValue) =>
        (runeValue >= CharToHexLookup.Length)
        ? _x_
        : CharToHexLookup[runeValue];
}
