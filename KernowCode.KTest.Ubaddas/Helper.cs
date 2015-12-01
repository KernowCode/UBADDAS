using System;

namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// <para>Handle complex type property chains where a value in the chain if null will throw an error</para>    
    /// </summary>
    public static class Default
    {
        /// <para>Handle complex type property chains where a value in the chain if null will throw an error</para>
        /// <para>e.g. var x = a.b.c would fail if b was null</para>
        /// <para>use. var x = Default.Get(() => a.b.c);</para>
        public static T Get<T>(Func<T> func, T defaultValue = default(T))
        {
            return func.DefaultGet(defaultValue: defaultValue);
        }

        /// <summary>
        /// Handles nulls in a complex type chain
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="func">Function returning complex type property</param>
        /// <param name="defaultValue">If cannot navigate complex type due to null return this value</param>
        /// <returns>Property or default</returns>
        public static T DefaultGet<T>(this Func<T> func, T defaultValue = default(T))
        {
            T value = defaultValue;
            try
            {
                value = func();
            }
            catch (Exception)
            {
                //complex type contained nulls
            }
            return value;
        }
    }
}
