using System;

namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// BDD Given part
    /// </summary>
    public interface IGiven : ITense
    {
        /// <summary>
        /// <para>Specifies the start of the 'When' section of BDD</para>
        /// <para>This can be followed by 'And' and 'Then'</para>
        /// <para>Example</para>
        /// <para> .When(customer.Login)</para>
        /// </summary>
        /// <param name="domainEntityCommand">The entitiy interface command method (without executing parenthesis)</param>
        /// <returns>Interface providing fluent methods 'And' and 'Then'</returns>
        IWhen When(Action domainEntityCommand);

        /// <summary>
        /// <para>Specifies the start of the 'When' section of BDD</para>
        /// <para>This can be followed by 'And' and 'Then'</para>
        /// <para>Example</para>
        /// <para> .When(x => CompleteCustomerRegistration(x, customer))</para>
        /// <para> </para>
        /// </summary>
        /// <param name="actionDelegate">A delegate containing a call to a method that will execute another set of behaviours. Specify 'ISet behaviour' as the first parameter of your method and use behaviour to perform another Given,When,Then</param>
        /// <returns>Interface providing fluent methods 'And' and 'Then'</returns>
        IWhen When(Action<ISet> actionDelegate);
    }
}