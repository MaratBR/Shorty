using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shorty.Utils
{
    public static class ShortUrlId
    {
        public static readonly char[] Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
        
        public static long? GetLongIdOrNull(string shortId)
        {
            long id = 0;
            for (int i = shortId.Length - 1; i >= 0; i--)
            {
                id *= 62;
                var ch = shortId[i];

                if (ch >= '0' && ch <= '9')
                    id += ch - '0';
                else if (ch >= 'A' && ch <= 'Z')
                    id += ch - 'A' + 10;
                else if (ch >= 'a' && ch <= 'z')
                    id += ch - 'a' + 10 + 26;
                else
                    return null;
            }

            return id;
        }
        
        public static string GetShortId(long value)
        {
            if (value < 0)
                value = -value;
            if (value == 0)
                return Chars[0].ToString();
            var chars = new List<Char>();

            while (value > 0)
            {
                var ch = Chars[value % Chars.Length];
                chars.Add(ch);
                value /= Chars.Length;
            }

            chars.Reverse();
            
            return new string(chars.ToArray());
        }
    }
}