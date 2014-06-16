using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// Extensions of any Object.  Use to name instances.
    /// </summary>
    public static class ObjectExtensions
    {
        private static readonly Dictionary<object, string> NamedInstances = new Dictionary<object, string>();

        /// <summary>
        /// <para>Names an object instance using the code variable name</para>
        /// <para>Example</para>
        /// <para> var myVariableToBeNamed = 123;</para>
        /// <para> myVariableToBeNamed.Named(() => myVariableToBeNamed);</para>
        /// </summary>
        /// <typeparam name="T">object instance type (supplied automatically)</typeparam>
        /// <param name="instance">object instance (supplied automatically)</param>
        /// <param name="expressionContainingOnlyYourInstance">i.e. () => myVariableToBeNamed</param>
        public static void Named<T>(this T instance, Expression<Func<T>> expressionContainingOnlyYourInstance)
        {
            var name = ((MemberExpression) expressionContainingOnlyYourInstance.Body).Member.Name.ExpandToReadable();
            instance.Named(name);
        }

        /// <summary>
        /// Names an object instance
        /// </summary>
        /// <typeparam name="T">object instance type (supplied automatically)</typeparam>
        /// <param name="instance">object instance (supplied automatically)</param>
        /// <param name="named">Name for the instance</param>
        /// <returns>returns the original instance (with a name recorded against it)</returns>
        public static T Named<T>(this T instance, string named)
        {
            if (NamedInstances.ContainsKey(instance)) NamedInstances[instance] = named;
            else NamedInstances.Add(instance, named);
            return instance;
        }
        
        /// <summary>
        /// Gets the recorded name for the instance, or the instance type name if not previously recorded using .Named extension
        /// </summary>
        /// <typeparam name="T">object instance type (supplied automatically)</typeparam>
        /// <param name="instance">object instance (supplied automatically)</param>
        /// <returns>The recorded name for the instance (previously set using the .Named extension method)</returns>
        public static string Name<T>(this T instance)
        {
            if (NamedInstances.ContainsKey(instance)) return NamedInstances[instance];
            return instance.GetType().Name;
        }

        /// <summary>
        /// <para>Turns text in to readable text</para>
        /// <para>i.e. 'myMethodName' becomes 'my method name'</para>
        /// </summary>
        /// <param name="text">string to expand. i.e. 'myMethodName'</param>
        /// <returns>expanded string. i.e. 'my method name'</returns>
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
