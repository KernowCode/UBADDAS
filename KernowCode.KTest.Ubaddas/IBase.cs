using System;

namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// BDD initiator
    /// </summary>
    public interface IBase : IAs
    {
        /// <summary>
        /// <para>Specifies the start of the 'Given' section of BDD</para>
        /// <para>This can be followed by 'And' and 'When'</para>
        /// <para>Must be preceeded with the 'As' statement</para>
        /// <para>Example</para>
        /// <para> .Given(customer.Login)</para>
        /// </summary>
        /// <param name="domainEntityCommand">The entitiy interface command method (without executing parenthesis)</param>
        /// <returns>Interface providing fluent methods 'And' and 'When'</returns>
        IGiven Given(Action domainEntityCommand);
       
        /// <summary>
        /// <para>Specifies the start of the 'Given' section of BDD</para>
        /// <para>This can be followed by 'And' and 'When'</para>
        /// /// <para>Must be preceeded with the 'As' statement</para>
        /// <para>Example</para>
        /// <para> .Given(x => RegisterCustomer(x, customer))</para>
        /// </summary>
        /// <param name="actionDelegate">A delegate containing a call to a method that will execute another set of behaviours. Specify 'ISet behaviour' as the first parameter of your method and use behaviour to perform another Given,When,Then</param>
        /// <returns>Interface providing fluent methods 'And' and 'When'</returns>
        IGiven Given(Action<ISet> actionDelegate);
    }
}