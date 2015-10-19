using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace WaveInfo.Extensions
{
    public static class StringExtensions
    {
        public static string ToFixedLength(this string value)
        {
            return $"{value,-20}";
        }

        public static string ToColumns(this string header, object value)
        {
            var text =  $"{value}".Escape();
            if (text.Length < 55)
            {
                return $"{header,-20}{text}";
            }
            
            var parts = Split(text, 55).ToArray();
            
            var builder = new StringBuilder();
            for (var i = 0; i < parts.Length; i++)
            {
                builder.AppendLine(i == 0 
                    ? $"{header,-20}{parts[i]}" 
                    : $"{new string(' ', 20)}{parts[i]}");
            }

            return builder.ToString();
        }

        public static string ToColumns(this AbstractChunk chunk, string property)
        {
            var info = chunk.GetType().GetProperty(property);
            if (info == null)
            {
                return "";
            }

            return property.ToColumns(info.GetValue(chunk));
        }

        public static string AppendEnd(this string value)
        {
            return $"{value}[end]";
        }

        public static string Escape(this string value)
        {
            return value.Replace('\0', '_').Replace('\r', '|').Replace('\n', '|');
        }

        public static string Escape(this char[] chars)
        {
            var builder=  new StringBuilder();
            foreach (var value in chars)
            {
                switch (value)
                {
                    case '\0':
                        builder.Append("*");
                        break;
                    case '\r':
                        builder.Append("*");
                        break;
                    case '\n':
                        builder.Append("*");
                        break;
                    default:
                        builder.Append(value);
                        break;
                }

            }

            return builder.ToString();
        }

        static IEnumerable<string> Split(string str, int chunkSize)
        {
            for (var i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }
    }
}
