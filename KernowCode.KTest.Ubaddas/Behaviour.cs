using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// Main UBADDAS behaviour logic, providing fluent interface
    /// </summary>
    public class Behaviour : IBase, IGiven, IWhen, IThen, IState, ISet
    {
        internal const int LeftSectionPadding = 9;

        private static string _targetApplicationLayer;

        private Behaviour()
        {
            Narrate = true;
        }

        private static string HelpCreateLine(string term, string content)
        {
            var padding = new string(' ', LeftSectionPadding - term.Length);
            return padding + (term + content).ExpandToReadable();
        }

        private void As(IPersona persona) //this method invoked via reflection from BehaviourExtensions method
        {
            CurrentPersonaType = GetPersonaLayerType(persona);
            if (Narrate)
                Console.WriteLine(HelpCreateLine("as", Char.ToUpperInvariant(persona.Name()[0]) + persona.Name().Substring(1))); 
        }

        private Type GetPersonaLayerType(IPersona persona)
        {
            if (_targetApplicationLayer == null)
                return persona.GetType();
            string typeNamespace = null;
            var personaType = persona.GetType();
            typeNamespace = personaType.FullName.TrimEnd(personaType.Name.ToCharArray()) +
                            _targetApplicationLayer;
            typeNamespace = typeNamespace + "." + personaType.Name + ", " + personaType.Assembly;
            var type = Type.GetType(typeNamespace);
            if (type == null)
                throw new NotImplementedException(
                    Environment.NewLine
                    + "Could not create instance of Persona with namespace " + typeNamespace
                    + Environment.NewLine);
            return type;
        }

        private void DoBehaviourSet(string behaviour, Action<ISet> actionDelegate)
        {
            Narrate = false;
            var rememberedPersona = CurrentPersonaType;                   
            Console.Write(behaviour.PadLeft(LeftSectionPadding).ExpandToReadable() + " ");
            actionDelegate(this);
            CurrentPersonaType = rememberedPersona;
            Narrate = true;
        }    

        private void DoBehaviour(string behaviour, Action domainEntityCommand)
        {
            if (Narrate)
            {
                var line = "";
                if (domainEntityCommand.Method.Name.Contains("_"))
                    line = domainEntityCommand.Method.Name.Replace("_", " " + domainEntityCommand.Target.Name()).ExpandToReadable();
                else
                    line = string.Format("{0} {1}", domainEntityCommand.Method.Name, domainEntityCommand.Target.Name()).ExpandToReadable();
                Console.WriteLine(HelpCreateLine(behaviour, line));
            }
            var implementedDomain = CreatePersonaImplementation();
            SetDomainOnPersonaImplementation(domainEntityCommand, implementedDomain);
            var method = GetPersonaImplementedMethod(domainEntityCommand);
            try
            {
                method.Invoke(implementedDomain, null);
            }
            catch (Exception exception)
            {
                if (exception.InnerException is NotImplementedException)
                {
                    throw new NotImplementedException(
                        string.Format(
                            Environment.NewLine
                            + "Pending implementation I{0}.{1} in the {2} class.",
                            domainEntityCommand.Target.GetType().Name,
                            domainEntityCommand.Method.Name, implementedDomain.GetType().FullName) +
                        Environment.NewLine, exception);
                }
                throw;
            }
        }        

        private object CreatePersonaImplementation()
        {
            object asPersona;
            try
            {
                asPersona = Activator.CreateInstance(CurrentPersonaType);
            }
            catch (Exception exception)
            {
                try
                {
                    throw new Exception(
                        Environment.NewLine +
                        string.Format("Make sure the '{0}' class has a parameterless constructor.",
                                      CurrentPersonaType.Name)
                        + Environment.NewLine, exception);
                }catch(Exception)
                {
                    throw new Exception(
                        Environment.NewLine +
                        "No Persona has been specified.  Use the 'As' statement to specify the Persona."
                        + Environment.NewLine, exception);
                }
            }            
            return asPersona;
        }

        private void SetDomainOnPersonaImplementation(Action domainEntityCommand, object persona)
        {
            try
            {
                var entityProperty = persona.GetType().GetProperty(domainEntityCommand.Target.GetType().Name);
                entityProperty.SetValue(persona, domainEntityCommand.Target);
            }
            catch (Exception exception)
            {
                throw new Exception(
                    Environment.NewLine
                    + string.Format("Could not set the '{0}' entity for the '{1}' persona implementation.",
                                    domainEntityCommand.Target.GetType().Name, persona.GetType().Name)
                    + Environment.NewLine
                    + string.Format(
                        "Check you have a public property 'public {0} {0} {{ get; set; }}' in the '{1}' class.",
                        domainEntityCommand.Target.GetType().Name, persona.GetType().Name)
                    + Environment.NewLine, exception);
            }
        }

        private MethodInfo GetPersonaImplementedMethod(Action domainEntityCommand)
        {
            try
            {
                var method = CurrentPersonaType.GetMethod(domainEntityCommand.Method.Name);
                if (method == null)
                    method =
                        CurrentPersonaType.GetMethods(BindingFlags.Public | BindingFlags.Instance |
                                                      BindingFlags.NonPublic)
                            .FirstOrDefault(
                                x =>
                                x.Name.EndsWith(domainEntityCommand.Target.GetType().Name + "." + domainEntityCommand.Method.Name));
                if (method == null)
                    throw new NotImplementedException();
                return method;
            }
            catch(Exception exception)
            {
                throw new Exception(
                    Environment.NewLine
                    + string.Format(
                        "There was a problem calling '{0}' in the '{1}' persona implementation of the '{2}' entity.",
                        domainEntityCommand.Method.Name, CurrentPersonaType.Name, domainEntityCommand.Target.GetType().Name)
                    + Environment.NewLine
                    + string.Format(
                        "Make sure the '{0}' persona implementation implements the '{1}' entity interface including the '{2}' method.",
                        CurrentPersonaType.Name, domainEntityCommand.Target.GetType().Name, domainEntityCommand.Method.Name)
                    + Environment.NewLine, exception);
            }
        }
        
        /// <summary>
        /// <para>Specifies the start of the 'Given' section of BDD</para>
        /// <para>This can be followed by 'And' and 'When'</para>
        /// <para>It must be preceeded by 'As'</para>
        /// <para>Example</para>
        /// <para> .Given(customer.Login)</para>
        /// </summary>
        /// <param name="domainEntityCommand">The entitiy interface command method (without executing parenthesis)</param>
        /// <returns>Interface providing fluent methods 'And' and 'When'</returns>
        public IGiven Given(Action domainEntityCommand)
        {
            DoBehaviour("given", domainEntityCommand);
            return this;
        }

        /// <summary>
        /// <para>Specifies the start of the 'When' section of BDD</para>
        /// <para>This can be followed by 'And' and 'Then'</para>
        /// <para>Example</para>
        /// <para> .When(customer.Login)</para>
        /// </summary>
        /// <param name="domainEntityCommand">The entitiy interface command method (without executing parenthesis)</param>
        /// <returns>Interface providing fluent methods 'And' and 'Then'</returns>
        public IWhen When(Action domainEntityCommand)
        {
            DoBehaviour("when", domainEntityCommand);
            return this;
        }

        /// <summary>
        /// <para>Specifies the start of the 'Then' section of BDD</para>
        /// <para>This can be followed by 'And'</para>
        /// <para>Example</para>
        /// <para> .Then(customer.Logout)</para>
        /// </summary>
        /// <param name="domainEntityCommand">The entitiy interface command method (without executing parenthesis)</param>
        /// <returns>Interface providing fluent method 'And'</returns>
        public IThen Then(Action domainEntityCommand)
        {
            DoBehaviour("then", domainEntityCommand);
            return this;
        }

        /// <summary>
        /// <para>Specifies the start of the 'Given' section of BDD</para>
        /// <para>This can be followed by 'And' and 'When'</para>
        /// <para>It must be preceeded by 'As'</para>
        /// <para>Example</para>
        /// <para> .Given(x => RegisterCustomer(x, customer))</para>
        /// </summary>
        /// <param name="actionDelegate">A delegate containing a call to a method that will execute another set of behaviours. Specify 'ISet behaviour' as the first parameter of your method and use behaviour to perform another Given,When,Then</param>
        /// <returns>Interface providing fluent methods 'And' and 'When'</returns>
        public IGiven Given(Action<ISet> actionDelegate)
        {
            DoBehaviourSet("given", actionDelegate);
            return this;
        }

        /// <summary>
        /// <para>Specifies the start of the 'When' section of BDD</para>
        /// <para>This can be followed by 'And' and 'Then'</para>
        /// <para>Example</para>
        /// <para> .When(x => CompleteCustomerRegistration(x, customer))</para>
        /// </summary>
        /// <param name="actionDelegate">A delegate containing a call to a method that will execute another set of behaviours. Specify 'ISet behaviour' as the first parameter of your method and use behaviour to perform another Given,When,Then</param>
        /// <returns>Interface providing fluent methods 'And' and 'Then'</returns>
        public IWhen When(Action<ISet> actionDelegate)
        {
            DoBehaviourSet("when", actionDelegate);
            return this;
        }

        /// <summary>
        /// <para>Specifies the start of the 'Then' section of BDD</para>
        /// <para>This can be followed by 'And'</para>
        /// <para>Example</para>
        /// <para> .Then(x => CheckAllOrders(x, customer))</para>
        /// </summary>
        /// <param name="actionDelegate">A delegate containing a call to a method that will execute another set of behaviours. Specify 'ISet behaviour' as the first parameter of your method and use behaviour to perform another Given,When,Then</param>
        /// <returns>Interface providing fluent method 'And'</returns>
        public IThen Then(Action<ISet> actionDelegate)
        {
            DoBehaviourSet("then", actionDelegate);
            return this;
        }

        internal static Behaviour SoThat(string businessValue, string targetApplicationLayer)
        {
            _targetApplicationLayer = targetApplicationLayer;
            var testName = GetTestMethodName();
            testName = AddPrefix(testName);            
            Console.WriteLine("{0}", testName.ExpandToReadable());
            Console.WriteLine(HelpCreateLine("so that", businessValue));
            return new Behaviour();
        }

        private static string GetTestMethodName()
        {
            var frames = new StackTrace().GetFrames();
            foreach (var stackFrame in frames)
            {
                foreach (var attribute in stackFrame.GetMethod().CustomAttributes)
                {
                    if (new[] { "TestAttribute", "TestMethodAttribute" }.Any(x => x == attribute.AttributeType.Name))
                        return stackFrame.GetMethod().Name;
                }
            }
            throw new Exception("", new Exception("When trying to render test behaviour to console, could not find test method (by custom attribute named 'Test' or 'TestMethod')"));
        }

        private static string AddPrefix(string reason)
        {
            var disallowedPrefixes = new [] {"Should"};
            foreach (var disallowedPrefix in disallowedPrefixes)
                reason = reason.TrimStart(disallowedPrefix.ToCharArray());
            var reasonPrefixes = new[] { "IWantTo", "IWant", "InOrderTo", "InOrder" };
            if (reasonPrefixes.Any(x => reason.ToLower().StartsWith(x.ToLower())))
                return reason;
            return HelpCreateLine("IWantTo", reason);
        }

        /// <summary>
        /// Stop or start output of BDD statements
        /// </summary>
        public bool Narrate { get; set; }

        /// <summary>
        /// Stores the IPersona type specified previously in the BDD statement via the 'As' statement
        /// </summary>
        public Type CurrentPersonaType { get; set; }

        /// <summary>
        /// <para>Used when you create a method to perform an inner set of behaviours</para>
        /// <para>Example</para>
        /// <para> behaviour.Perform()</para>
        /// <para>  .Given(...</para>
        /// </summary>
        /// <returns></returns>
        public IBase Perform()
        {
            var methodName = new StackTrace().GetFrame(1).GetMethod().Name.ExpandToReadable();
            methodName = Char.ToLowerInvariant(methodName[0]) + methodName.Substring(1);
            Console.WriteLine(methodName);
            return this;
        }
    }
}