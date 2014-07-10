using System;

namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// BDD When part
    /// </summary>
    public interface IWhen : ITense
    {
        /// <summary>
        /// <para>Specifies the start of the 'Then' section of BDD</para>
        /// <para>This can be followed by 'And'</para>
        /// <para>Example</para>
        /// <para> .Then(customer.Logout)</para>
        /// </summary>
        /// <param name="domainEntityCommand">The entitiy interface command method (without executing parenthesis)</param>
        /// <returns>Interface providing fluent method 'And'</returns>
        IThen Then(Action domainEntityCommand);

        /// <summary>
        /// <para>Specifies the start of the 'Then' section of BDD</para>
        /// <para>This can be followed by 'And'</para>
        /// <para>Example</para>
        /// <para> .ThenWe(x => CheckAllOrders(x, customer))</para>
        /// </summary>
        /// <param name="actionDelegate">A delegate containing a call to a method that will execute another set of behaviours. Specify 'ISet behaviour' as the first parameter of your method and use behaviour to perform another Given,When,Then</param>
        /// <returns>Interface providing fluent method 'And'</returns>
        IThen ThenWe(Action<ISet> actionDelegate);
    }
}