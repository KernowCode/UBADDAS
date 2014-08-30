using System;
using System.Reflection;

namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// Provides BDD statement extensions including 'As' and 'And'
    /// </summary>
    public static class BehaviourExtensions
    {
        /// <summary>
        /// <para>Specifies the Persona to perform the following BDD statements as</para>
        /// </summary>
        /// <typeparam name="T">BDD Tense (Given,When,Then) (supplied automatically)</typeparam>
        /// <param name="behaviour">Current behaviour instance (supplied automatically)</param>
        /// <param name="persona">An instance of your personas that implements the IPersona interface</param>
        /// <returns>Current BDD Tense (Given, When, or Then)</returns>
        public static T As<T>(this T behaviour, IPersona persona) where T : IAs
        {
            behaviour.GetType().GetMethod("As", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                behaviour, new object[] { persona });
            return behaviour;
        }

        /// <summary>
        /// Allows another implementation of the current BDD tense (given, when or then)
        /// </summary>
        /// <typeparam name="T">BDD Tense (Given,When,Then) (supplied automatically)</typeparam>
        /// <param name="behaviour">Current behaviour instance (supplied automatically)</param>
        /// <param name="domainEntityCommand">The entitiy interface command method (without executing parenthesis)</param>
        /// <returns>Current BDD Tense (Given, When, or Then)</returns>
        public static T And<T>(this T behaviour, Action domainEntityCommand) where T : ITense
        {
            behaviour.GetType().GetMethod("DoBehaviour", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                behaviour, new object[] { "and", domainEntityCommand });
            return behaviour;
        }

        /// <summary>
        /// Allows another implementation of the current BDD tense (given, when or then)
        /// </summary>
        /// <typeparam name="T">BDD Tense (Given,When,Then) (supplied automatically)</typeparam>
        /// <param name="behaviour">Current behaviour instance (supplied automatically)</param>
        /// <param name="actionDelegate">A delegate containing a call to a method that will execute another set of behaviours. Specify 'ISet behaviour' as the first parameter of your method and use behaviour to perform another Given,When,Then</param>
        /// <returns>Current BDD Tense (Given, When, or Then)</returns>
        public static T AndWe<T>(this T behaviour, Action<ISet> actionDelegate) where T : ITense
        {
            behaviour.GetType().GetMethod("DoBehaviourSet", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                behaviour, new object[] { "and we", actionDelegate });
            return behaviour;
        }     
    }
}
