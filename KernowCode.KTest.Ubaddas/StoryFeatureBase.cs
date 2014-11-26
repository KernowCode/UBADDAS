using KernowCode.KTest.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// <para>Enables a test class to use the UBADDAS test syntax</para>
    /// <para>An Enum representing the User Stories business values must be supplied</para>
    /// <para></para>
    /// </summary>
    /// <typeparam name="TBusinessValueEnum"></typeparam>
    public class StoryFeatureBase<TBusinessValueEnum>
    {
        /// <summary>
        /// <para>Gets the name of the target application layer to test against.</para>
        /// <para>Override this in your test or base test feature class</para>
        /// <para>Returning null will result in using the root Persona class in the Personas namespace</para>
        /// </summary>
        /// <returns>Persona namespace indicating the application layer. i.e. 'Presentation' which will translate to the namespace yourProject.Personas.Presentation.thePersona class</returns>
        protected virtual string ApplicationLayer()
        {
            return null;
        }
        
        /// <summary>
        /// <para>Initial method to start new User Story and BDD syntax.</para>
        /// <para>Uses test method name as the 'I Want' part of the User Story syntax.</para>
        /// <para>Follow this 'SoThat' with the 'As' to specify the 'As A' part of the User Story syntax.</para>
        /// </summary>
        /// <param name="businessValue">Supply an enumerator that describes the business values for the User Stories</param>
        /// <returns>Fluent interface providing BDD Given, When, Then syntax</returns>
        protected IBase SoThat(TBusinessValueEnum businessValue)
        {
            var loggerFactory = LoggerFactory ?? ConfigureDefaultLoggersFactory;
            var loggers = new Loggers(loggerFactory());
            loggers.SetStartTextsToEmphasise("I want", "So that", "As", "Given", "When", "Then", "And");
            return Behaviour.SoThat(businessValue.ToString(), ApplicationLayer(), loggers);
        }

        private List<ILogger> ConfigureDefaultLoggersFactory()
        {
            var loggers = new List<ILogger>();
            loggers.Add(new ConsoleLogger(Behaviour.LeftSectionPadding));            
            return loggers;
        }

        /// <summary>
        /// <para>Create a new test output file path based on test method name located at executing assembly TestOutput folder</para>
        /// <para>i.e. .../bin/Debug/kTestOutput/MyTest.html</para>
        /// </summary>
        /// <param name="extension">file extension to append to filename</param>
        /// <returns>filepath</returns>
        public static string GetTestOutputFilePath(string extension)
        {
            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");
            return Behaviour.GetTestFilename(@"{1}\kTestOutput\{0}.{2}.{3}", GetSourceDirectory(), timeStamp, extension);
        }

        private static string GetSourceDirectory()
        {
            var uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Allows you to specify the test output loggers
        /// </summary>
        protected Func<List<ILogger>> LoggerFactory { get; set; }
    }
}
