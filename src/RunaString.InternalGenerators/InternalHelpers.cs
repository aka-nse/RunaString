using System.Text;

namespace RunaString.InternalGenerators;

internal static class InternalHelpers
{
    extension(string str)
    {
        public string Replace(ReadOnlySpan<char> oldValues, char newValue)
        {
            var sb = new StringBuilder(str.Length);
            int startIndex = 0;
            while (true)
            {
                int index = str.AsSpan(startIndex).IndexOfAny(oldValues);
                if (index == -1)
                {
                    sb.Append(str.AsSpan(startIndex));
                    return sb.ToString();
                }
                sb.Append(str.AsSpan(startIndex, index));
                sb.Append(newValue);
                startIndex += index + 1;
            }
        }


        public byte[] AsHexString()
        {
            var bytes = new byte[str.Length / 2];
            for (int i = 0, j = 0; i < str.Length; i += 2, j++)
            {
                if (char.IsWhiteSpace(str[i]))
                {
                    continue;
                }
                bytes[j] = Convert.ToByte(str.AsSpan(i, 2).ToString(), 16);
            }
            return bytes;
        }
    }


    extension(StringBuilder sb)
    {
        public void Append(ReadOnlySpan<char> value)
        {
            unsafe
            {
                fixed (char* pValue = value)
                {
                    sb.Append(pValue, value.Length);
                }
            }
        }
    }
}
