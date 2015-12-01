using System;
using System.Text.RegularExpressions;

namespace KernowCode.KTest.Logging
{
    /// <summary>
    /// Extensions of any Object.  Use to name instances.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// <para>Turns text in to readable text</para>
        /// <para>i.e. 'myMethodName' becomes 'my method name'</para>
        /// </summary>
        /// <param name="text">string to expand. i.e. 'myMethodName'</param>
        /// <returns>expanded string. i.e. 'my method name'</returns>
        public static string ExpandToReadable(this string text)
        {
            const string regularExpressionToMatchWhiteSpace = @"[ ]{2,}";
            text = Regex.Replace(text, regularExpressionToMatchWhiteSpace, x => " ");

            const string regularExpressionToMatchUppercaseFollowingUpperOrLowercaseCharacter =
                @"(?<=[\p{Ll}]|[\p{Lu}])([\p{Lu}])";
            text = Regex.Replace(text, regularExpressionToMatchUppercaseFollowingUpperOrLowercaseCharacter,
                                 x => " " + x.ToString().ToLower());

            const string regularExpressionToMatchLettersFollowingNumbers = @"(?<=[0-9])([\p{L}])";
            text = Regex.Replace(text, regularExpressionToMatchLettersFollowingNumbers,
                                 x => " " + x.ToString().ToLower());

            const string regularExpressionToMatchNumbersFollowingLetters = @"(?<=[\p{L}])([0-9])";
            text = Regex.Replace(text, regularExpressionToMatchNumbersFollowingLetters,
                                 x => " " + x.ToString().ToLower());

            return text;
        }

        public static string CapitaliseInitial(this string text)
        {
            if (text.Length == 0) return text;
            return Char.ToUpperInvariant(text[0]) + text.Substring(1);
        }

        public static string DecapitaliseInitial(this string text)
        {
            if (text.Length == 0) return text;
            return Char.ToLowerInvariant(text[0]) + text.Substring(1);
        }
    }
}