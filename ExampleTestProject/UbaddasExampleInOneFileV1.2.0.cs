using System.Diagnostics;
using KernowCode.KTest.Logging;
using KernowCode.KTest.Ubaddas.Example.Personas;
using KernowCode.KTest.Ubaddas.Example.Production.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

// This example shows all elements in on file.  Elements of this file
// for instance the namespaces and classes should exist in their own
// project or files

namespace KernowCode.KTest.Ubaddas.Example
{    
    public enum MyValues
    {
        WeIncreaseCustomerBase
    }

    public class ExampleTestBase : StoryFeatureBase<MyValues>
    {
        protected override string ApplicationLayer()
        {
            base.ApplicationLayer(); //hover over to get intelisense documentation (it is not necessary to call the base method)

            //changing the return value will result in a different target application layer to test against.
            return "Presentation"; //will point to the Personas/Presentation folder and associated namespace.
            //return "RestApi"; //will point to the Personas/RestApi folder and associated namespace.

            //Notice the Personas folder structure in this project and how it
            //relates to the possible values return from this method.

            //Decide on your own application layers and implement accordingly.
            //You could get the layer name from an App.Config if you prefer.
            //Your automated continuous integration could run the test with 
            //a command line argument or build with different App.Config files.
        }   

        [TestInitialize]
        public void TestSetup()
        {
            base.LoggerFactory = MyCustomLoggerFactory;
        }

        private List<ILogger> MyCustomLoggerFactory()
        {
            var loggers = new List<ILogger>();            
            loggers.Add(new ConsoleLogger(new ConsoleStringLoggerEntry(), Behaviour.LeftSectionPadding));
            loggers.Add(new HtmlLogger(new HtmlStringLoggerEntry(), GetTestOutputFilePath("html")));                        
            return loggers;
        }

    }

    [TestClass]
    public class ExampleTest : ExampleTestBase
    {                 
        [TestMethod]
        public void TestRegisterCustomer_StandardBehaviour()
        {
            var customer = new TestCustomer();

            SoThat(MyValues.WeIncreaseCustomerBase)
                .As(new WebUser())
                .Given(customer.Named("initial customer").Has("completed").Registration)
                .When(customer.Has("confirmed").Registration)
                .Then(customer.Named("as returning customer").Login);                                
        }

        [TestMethod]
        public void TestRegisterCustomer_NestedReusableBehaviours()
        {
            var customer1 = new TestCustomer();
            var customer2 = new TestCustomer();

            SoThat(MyValues.WeIncreaseCustomerBase)
                .As(new WebUser())
                .GivenWe(x => RegisterAndConfirmCustomerRegistration(x, customer1.Named("customer 1")))                
                .AndWe(x => RegisterAndConfirmCustomerRegistration(x, customer2.Named("customer 2")));                
        }

        private void RegisterAndConfirmCustomerRegistration(ISet behaviour, ITestCustomer customer)
        {
            behaviour.Perform()
                .Given(customer.Has("completed").Registration)
                .When(customer.Has("confirmed").Registration)
                .Then(customer.Named("returning " + customer.Name()).Login);
        }
        
    }

    namespace Personas
    {
        public interface ITestCustomer : ICustomer, IUseVerbs
        {
        }

        public class TestCustomer: ITestCustomer
        {
            public Verb Verb { get; set; }

            public void Registration()
            {                
            }

            public void Login()
            {
            }                        
        }

        public class WebUser : IPersona, IUseVerbs
        {
            public Verb Verb { get; set; }
            public ILogger Log { get; set; }
            public TestCustomer TestCustomerEntity { get; set; }
        }

        namespace Presentation 
        {
            //You could use Selenium WebDriver for instance to command the presentation layer
            public class WebUser : Personas.WebUser, ITestCustomer
            {                
                public void Registration()
                {
                    if (Verb.Value == "completed")
                        Log.WriteLine("Presentation layer test implementation - register user " + TestCustomerEntity.Name());
                    else if (Verb.Value == "confirmed")                        
                        Log.WriteLine("Presentation layer test implementation - user confirms registration email");
                }

                public void Login()
                {
                    Log.WriteLine("Presentation layer test implementation - user logs in");
                }
            }
        }

        namespace RestApi //another layer implementation
        {
            //You could make Rest call for instance to command the Api layer
            public class WebUser : Personas.WebUser, ITestCustomer
            {                                
                public void Registration()
                {
                    if (Verb.Value == "completed")
                        Log.WriteLine("RestApi layer test implementation - register user");
                    else if (Verb.Value == "confirmed")
                        Log.WriteLine("RestApi layer test implementation - user confirms registration email");
                }

                public void Login()
                {
                    Log.WriteLine("RestApi layer test implementation - user logs in");
                }
            }
        }
    }
}

namespace KernowCode.KTest.Ubaddas.Example.Production.Code
{

    public class Customer : ICustomer
    {
        public void Register()
        {
            //your production domain logic here, what your app does
        }

        public void Registration()
        {
            //your production domain logic here, what your app does
        }

        public void Login()
        {
            //your production domain logic here, what your app does
        }
    }

    public interface ICustomer
    {        
        void Registration();
        void Login();
    }
}
