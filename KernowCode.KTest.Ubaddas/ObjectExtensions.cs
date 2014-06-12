using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace KernowCode.KTest.Ubaddas
{
    public static class ObjectExtensions
    {
        private static readonly Dictionary<object, string> namedInstances = new Dictionary<object, string>();

        public static void Named<T>(this T instance, Expression<Func<T>> expressionContainingOnlyYourInstance)
        {
            var name = ((MemberExpression) expressionContainingOnlyYourInstance.Body).Member.Name.ExpandToReadable();
            instance.Named(name);
        }

        public static T Named<T>(this T instance, string named)
        {
            if (namedInstances.ContainsKey(instance)) namedInstances[instance] = named;
            else namedInstances.Add(instance, named);
            return instance;
        }
        
        public static string Name<T>(this T instance)
        {
            if (namedInstances.ContainsKey(instance)) return namedInstances[instance];
            return instance.GetType().Name;
        }

        public static string ExpandToReadable(this string text)
        {
            const string regularExpressionToMatchUppercaseFollowingUpperOrLowercaseCharacter = @"(?<=[\p{Ll}]|[\p{Lu}])([\p{Lu}])";
            text = Regex.Replace(text, regularExpressionToMatchUppercaseFollowingUpperOrLowercaseCharacter, x => " " + x.ToString().ToLower());

            const string regularExpressionToMatchLettersFollowingNumbers = @"(?<=[0-9])([\p{L}])";
            text = Regex.Replace(text, regularExpressionToMatchLettersFollowingNumbers, x => " " + x.ToString().ToLower());

            const string regularExpressionToMatchNumbersFollowingLetters = @"(?<=[\p{L}])([0-9])";
            text = Regex.Replace(text, regularExpressionToMatchNumbersFollowingLetters, x => " " + x.ToString().ToLower());

            const string regularExpressionToMatchWhiteSpace = @"[ ]{2,}";
            text = Regex.Replace(text, regularExpressionToMatchWhiteSpace, x => " ");

            return text;
        }
    }
}
