using System;
using System.Reflection;

namespace KernowCode.KTest.Ubaddas
{
    public static class BehaviourExtensions
    {
        public static T As<T>(this T behaviour, IPersona persona) where T : IAs
        {
            behaviour.GetType().GetMethod("As", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                behaviour, new object[] { persona });
            return behaviour;
        }

        public static T And<T>(this T behaviour, Action domainEntityCommand) where T : ITense
        {
            behaviour.GetType().GetMethod("DoBehaviour", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                behaviour, new object[] { "And", domainEntityCommand });
            return behaviour;
        }

        public static T And<T>(this T behaviour, Action<ISet> actionSet) where T : ITense
        {
            behaviour.GetType().GetMethod("DoBehaviourSet", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                behaviour, new object[] { "And", actionSet });
            return behaviour;
        }     
    }
}