using System.Text;

namespace RunaString.Test;

public class RuneHelpersTest
{
    [Fact]
    public void DecodeFromUtf8At_SuccessPopFromValidSequence()
    {
        static IEnumerable<Rune> enumerateAssignedScalarValues()
        {
            for (var i = 0; i <= 0x10FFFF; ++i)
            {
                if (Rune.IsValid(i))
                {
                    yield return new Rune(i);
                }
            }
        }

        var buffer = (stackalloc byte[16]);
        foreach (var c in enumerateAssignedScalarValues())
        {
            {
                // from sequence head
                var expectedBytesConsumed = c.EncodeToUtf8(buffer);
                Assert.True(RuneHelpers.DecodeFromUtf8At(buffer, 0, out var decodedRune, out var bytesConsumed));
                Assert.Equal(expectedBytesConsumed, bytesConsumed);
                Assert.Equal(c, decodedRune);
            }
            {
                // from sequence middle
                var expectedBytesConsumed = c.EncodeToUtf8(buffer.Slice(8));
                Assert.True(RuneHelpers.DecodeFromUtf8At(buffer, 8, out var decodedRune, out var bytesConsumed));
                Assert.Equal(expectedBytesConsumed, bytesConsumed);
                Assert.Equal(c, decodedRune);
            }
        }
    }

    [Fact]
    public void DecodeFromUtf8At_SuccessPopFromInvalidSequence()
    {
        static IEnumerable<byte[]> enumerateInvalidUtf8()
        {
            // not enough bytes length for 2 byte character
            yield return [ 0xC2 ];

            // not continue byte on 2nd byte
            yield return [ 0xC2, 0x41 ]; // 'A'

            // overlong
            yield return [ 0xC0, 0x80 ];

            // not enough bytes length for 3 byte character
            yield return [ 0xE2 ];

            // no continue byte on 2nd byte
            yield return [ 0xE2, 0x41, 0x80 ];

            // overlong
            yield return [ 0xE0, 0x80, 0x80 ];

            // surrogate
            yield return [0xED, 0xA0, 0x80 ];

            // not enough bytes length for 4 byte character
            yield return [ 0xF0 ];

            // no continue byte on 2nd byte
            yield return [ 0xF0, 0x28, 0x80, 0x80 ];

            // overlong
            yield return [ 0xF0, 0x80, 0x80, 0x80 ];

            // > U+10FFFF
            yield return [ 0xF5, 0x80, 0x80, 0x80 ];
            yield return [ 0xF4, 0x90, 0x80, 0x80 ];

            yield break;
        }

        foreach(var buffer in enumerateInvalidUtf8())
        {
            Assert.True(RuneHelpers.DecodeFromUtf8At(buffer, 0, out var decodedRune, out var bytesConsumed));
            Assert.Equal(1, bytesConsumed);
            Assert.Equal(Rune.ReplacementChar, decodedRune);
        }
    }

    [Fact]
    public void DecodeFromUtf8At_FailedAtTail()
    {
        var buffer = (stackalloc byte[0]);
        Assert.False(RuneHelpers.DecodeFromUtf8At(buffer, 0, out var decodedRune, out var bytesConsumed));
        Assert.Equal(0, bytesConsumed);
        Assert.Equal(default, decodedRune);
    }
}
