using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using KernowCode.KTest.Logging;

namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// Main UBADDAS behaviour logic, providing fluent interface
    /// </summary>
    public class Behaviour : IBase, IGiven, IWhen, IThen, IState, ISet
    {
        /// <summary>
        /// Specifies the amount of indentation for console output to align BDD statements
        /// </summary>
        public const int LeftSectionPadding = 9;

        private readonly Loggers _loggers;
        private readonly string _targetApplicationLayer;

        private Behaviour(string targetApplicationLayer, Loggers loggers, string testName, string businessValue)
        {
            _loggers = loggers;
            _targetApplicationLayer = targetApplicationLayer;
            Log(() => _loggers.WriteLine("{0}", new[] {testName}));
            Log(() => _loggers.WriteLine("so that" + Loggers.MidAlignSeparator + businessValue.ExpandToReadable().DecapitaliseInitial()));
            _loggers.StepsStart();

        }

        #region IBase Members

        /// <summary>
        /// <para>Specifies the start of the 'Given' section of BDD</para>
        /// <para>This can be followed by 'And' and 'When'</para>
        /// <para>It must be preceeded by 'As'</para>
        /// <para>Example</para>
        /// <para> .Given(customer.Login)</para>
        /// <para> .Given() //nothing</para>
        /// </summary>
        /// <param name="domainEntityCommand">The entitiy interface command method (without executing parenthesis)</param>
        /// <returns>Interface providing fluent methods 'And' and 'When'</returns>
        public IGiven Given(Action domainEntityCommand = null)
        {
            DoBehaviour("given", domainEntityCommand);
            return this;
        }

        /// <summary>
        /// <para>Specifies the start of the 'Given' section of BDD</para>
        /// <para>This can be followed by 'And' and 'When'</para>
        /// <para>It must be preceeded by 'As'</para>
        /// <para>Example</para>
        /// <para> .GivenWe(x => RegisterCustomer(x, customer))</para>
        /// </summary>
        /// <param name="actionDelegate">A delegate containing a call to a method that will execute another set of behaviours. Specify 'ISet behaviour' as the first parameter of your method and use behaviour to perform another Given,When,Then</param>
        /// <returns>Interface providing fluent methods 'And' and 'When'</returns>
        public IGiven GivenWe(Action<ISet> actionDelegate)
        {
            DoBehaviourSet("given we", actionDelegate);
            return this;
        }

        #endregion

        #region IGiven Members

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
        /// <para>Specifies the start of the 'When' section of BDD</para>
        /// <para>This can be followed by 'And' and 'Then'</para>
        /// <para>Example</para>
        /// <para> .WhenWe(x => CompleteCustomerRegistration(x, customer))</para>
        /// </summary>
        /// <param name="actionDelegate">A delegate containing a call to a method that will execute another set of behaviours. Specify 'ISet behaviour' as the first parameter of your method and use behaviour to perform another Given,When,Then</param>
        /// <returns>Interface providing fluent methods 'And' and 'Then'</returns>
        public IWhen WhenWe(Action<ISet> actionDelegate)
        {
            DoBehaviourSet("when we", actionDelegate);
            return this;
        }

        #endregion

        #region ISet Members

        /// <summary>
        /// <para>Used when you create a method to perform an inner set of behaviours</para>
        /// <para>Example</para>
        /// <para> behaviour.Perform()</para>
        /// <para>  .Given(...</para>
        /// </summary>
        /// <returns></returns>
        public IBase Perform()
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name.ExpandToReadable();
            methodName = methodName.DecapitaliseInitial();
            _loggers.WriteLine(methodName);
            return this;
        }

        #endregion

        #region IState Members

        /// <summary>
        /// Stores the IPersona type specified previously in the BDD statement via the 'As' statement
        /// </summary>
        public Type CurrentPersonaType { get; set; }

        #endregion

        #region IWhen Members

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
        /// <para>Specifies the start of the 'Then' section of BDD</para>
        /// <para>This can be followed by 'And'</para>
        /// <para>Example</para>
        /// <para> .ThenWe(x => CheckAllOrders(x, customer))</para>
        /// </summary>
        /// <param name="actionDelegate">A delegate containing a call to a method that will execute another set of behaviours. Specify 'ISet behaviour' as the first parameter of your method and use behaviour to perform another Given,When,Then</param>
        /// <returns>Interface providing fluent method 'And'</returns>
        public IThen ThenWe(Action<ISet> actionDelegate)
        {
            DoBehaviourSet("then we", actionDelegate);
            return this;
        }

        #endregion

        private void Log(Action action)
        {
            _loggers.SubSectionStart();
            try
            {
                action.Invoke();
            }
            finally
            {
                _loggers.SubSectionEnd();
            }
        }

        private void As(IPersona persona) //this method invoked via reflection from BehaviourExtensions method
        {
            _loggers.StepsStop();
            _loggers.SubSectionEnd();
            _loggers.SubSectionStart();
            _loggers.StepsStart();
            CurrentPersonaType = GetPersonaLayerType(persona);
            _loggers.WriteLine("as" + Loggers.MidAlignSeparator + persona.Name().DecapitaliseInitial());
        }

        private Type GetPersonaLayerType(IPersona persona)
        {
            if (_targetApplicationLayer == null)
                return persona.GetType();
            string typeNamespace = null;
            Type personaType = persona.GetType();
            typeNamespace = personaType.FullName.TrimEnd(personaType.Name.ToCharArray()) +
                            _targetApplicationLayer;
            typeNamespace = typeNamespace + "." + personaType.Name + ", " + personaType.Assembly;
            Type type = Type.GetType(typeNamespace);
            if (type == null)
                throw new NotImplementedException(
                    Environment.NewLine
                    + "Could not create instance of Persona with namespace " + typeNamespace
                    + Environment.NewLine);
            return type;
        }

        private void DoBehaviourSet(string behaviour, Action<ISet> actionDelegate)
        {
            Log(() =>
                {
                    _loggers.Write(behaviour + Loggers.MidAlignSeparator);
                    Type rememberedPersona = CurrentPersonaType;
                    actionDelegate(this);
                    CurrentPersonaType = rememberedPersona;
                });
        }

        private void DoBehaviour(string behaviour, Action domainEntityCommand)
        {
            Log(() =>
                {
                    if (domainEntityCommand != null)
                    {
                        string line = "";
                        if (domainEntityCommand.Method.Name.Contains("_"))
                            line =
                                domainEntityCommand.Method.Name.Replace("_", " " + domainEntityCommand.Target.Name()).
                                    ExpandToReadable();
                        else
                            line =
                                string.Format("{0} {1}", domainEntityCommand.Method.Name,
                                              domainEntityCommand.Target.Name())
                                    .ExpandToReadable();
                        line = line.DecapitaliseInitial(); 

                        _loggers.WriteLine(behaviour + Loggers.MidAlignSeparator + line);
                        object implementedDomain = CreatePersonaImplementation();
                        SetLoggerOnPersonaImplementation(implementedDomain);
                        SetDomainOnPersonaImplementation(domainEntityCommand, implementedDomain);
                        MethodInfo method = GetPersonaImplementedMethod(domainEntityCommand);
                        try
                        {
                            _loggers.StepsStart();
                            method.Invoke(implementedDomain, null);
                            _loggers.StepsStop();
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
                    else
                    {
                        _loggers.StepsStart();
                        _loggers.WriteLine("(no action taken)");
                        _loggers.StepsStop();
                    }
                });
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
                }
                catch (Exception)
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
                PropertyInfo entityProperty = persona.GetType().GetProperty(domainEntityCommand.Target.GetType().Name);
                if (entityProperty == null)
                    throw new Exception(string.Format("Could not get property named '{0}'",
                                                      domainEntityCommand.Target.GetType().Name));
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

        private void SetLoggerOnPersonaImplementation(object persona)
        {
            try
            {
                PropertyInfo entityProperty =
                    persona.GetType().GetProperties().FirstOrDefault(x => x.PropertyType == typeof (ILogger));
                entityProperty.SetValue(persona, _loggers);
            }
            catch
            {
                //ignore
            }
        }

        private MethodInfo GetPersonaImplementedMethod(Action domainEntityCommand)
        {
            try
            {
                MethodInfo method = CurrentPersonaType.GetMethod(domainEntityCommand.Method.Name);
                if (method == null)
                    method =
                        CurrentPersonaType.GetMethods(BindingFlags.Public | BindingFlags.Instance |
                                                      BindingFlags.NonPublic)
                            .FirstOrDefault(
                                x =>
                                x.Name.EndsWith(domainEntityCommand.Target.GetType().Name + "." +
                                                domainEntityCommand.Method.Name));
                if (method == null)
                    throw new NotImplementedException();
                return method;
            }
            catch (Exception exception)
            {
                throw new Exception(
                    Environment.NewLine
                    + string.Format(
                        "There was a problem calling '{0}' in the '{1}' persona implementation of the '{2}' entity.",
                        domainEntityCommand.Method.Name, CurrentPersonaType.Name,
                        domainEntityCommand.Target.GetType().Name)
                    + Environment.NewLine
                    + string.Format(
                        "Make sure the '{0}' persona implementation implements the '{1}' entity interface including the '{2}' method.",
                        CurrentPersonaType.Name, domainEntityCommand.Target.GetType().Name,
                        domainEntityCommand.Method.Name)
                    + Environment.NewLine, exception);
            }
        }

        /// <summary>
        /// New behaviour factory method
        /// </summary>
        /// <param name="businessValue">text describing the business value</param>
        /// <param name="targetApplicationLayer">project folder name containing target test class implementations</param>
        /// <param name="loggers">instance of loggers</param>
        /// <returns>new behaviour instance</returns>
        public static Behaviour SoThat(string businessValue, string targetApplicationLayer, Loggers loggers)
        {
            string testName = GetTestName();
            loggers.SetStartTextsToEmphasise("I want", "So that", "As", "Given", "When", "Then", "And");
            loggers.SetStartTextsToHaveSectionOpen("As");
            return new Behaviour(targetApplicationLayer, loggers, testName, businessValue);
        }

        private static string GetTestName()
        {
            string testName = GetTestMethod().Name;
            testName = AddPrefix(testName);
            return testName;
        }

        /// <summary>
        /// Creates a filename based on test method name
        /// </summary>
        /// <param name="format">Specify a string format if required to enrich filename</param>
        /// <param name="args">Specify format arguments (starting at 1 becaue 0 is used for the filename</param>
        /// <returns></returns>
        public static string GetTestFilename(string format = "{0}", params object[] args)
        {
            MethodBase method = GetTestMethod();
            string path = CleanFileName(method.DeclaringType.FullName);
            string filename = CleanFileName(method.Name);
            var arguments = new List<object>();
            arguments.Add(path + "\\" + filename);
            arguments.AddRange(args);
            filename = string.Format(format, arguments.ToArray());
            return filename;
        }

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName,
                                                            (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        private static MethodBase GetTestMethod()
        {
            StackFrame[] frames = new StackTrace().GetFrames();
            foreach (StackFrame stackFrame in frames)
            {
                foreach (CustomAttributeData attribute in stackFrame.GetMethod().CustomAttributes)
                {
                    if (new[] {"TestAttribute", "TestMethodAttribute"}.Any(x => x == attribute.AttributeType.Name))
                        return stackFrame.GetMethod();
                }
            }
            throw new Exception("",
                                new Exception(
                                    "When trying to render test behaviour to console, could not find test method (by custom attribute named 'Test' or 'TestMethod')"));
        }

        private static string AddPrefix(string reason)
        {
            var disallowedPrefixes = new[] {"Should", "Test"};
            foreach (string disallowedPrefix in disallowedPrefixes)
                reason = reason.TrimStart(disallowedPrefix.ToCharArray());
            var reasonPrefixes = new[] {"IWantTo", "IWant", "InOrderTo", "InOrder"};
            string prefix = reasonPrefixes.FirstOrDefault(x => reason.ToLower().StartsWith(x.ToLower()));
            if (!string.IsNullOrEmpty(prefix)) reason = reason.Substring(prefix.Length);
            else prefix = "IWantTo";
            reason = prefix + Loggers.MidAlignSeparator + reason.DecapitaliseInitial();
            return reason.ExpandToReadable();
        }
    }
}