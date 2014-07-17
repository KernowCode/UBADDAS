using System.Diagnostics;
using KernowCode.KTest.Ubaddas.Example.Personas;
using KernowCode.KTest.Ubaddas.Example.Production.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            base.ApplicationLayer(); //it is not necessary to call the base method, over over to get intelisense documentation

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
    }

    [TestClass]
    public class ExampleTest : ExampleTestBase
    {        
        [TestMethod]
        public void TestMethod1()
        {
            ICustomer customer = new Customer();

            SoThat(MyValues.WeIncreaseCustomerBase)
                .As(new User())
                .Given(customer.Named("initial customer").Register)
                .When(customer.Confirm_Registration)
                .Then(customer.Named("as returning customer").Login);
        }
    }

    namespace Personas
    {
        public class User : IPersona
        {
        }

        namespace Presentation 
        {
            //You could use Selenium WebDriver for instance to command the presentation layer
            public class User : ICustomer
            {
                public Customer Customer { get; set; }

                public void Register()
                {
                    Debug.WriteLine("Presentation layer test implementation - register user");
                }

                public void Confirm_Registration()
                {
                    Debug.WriteLine("Presentation layer test implementation - user confirms registration email");
                }

                public void Login()
                {
                    Debug.WriteLine("Presentation layer test implementation - user logs in");
                }
            }
        }

        namespace RestApi //another layer implementation
        {
            //You could make Rest call for instance to command the Api layer
            public class User : ICustomer
            {
                public Customer Customer { get; set; }

                public void Register()
                {
                    Debug.WriteLine("RestApi layer test implementation - register user");
                }

                public void Confirm_Registration()
                {
                    Debug.WriteLine("RestApi layer test implementation - user confirms registration email");
                }

                public void Login()
                {
                    Debug.WriteLine("RestApi layer test implementation - user logs in");
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
            //you production domain logic here, what your app does
        }

        public void Confirm_Registration()
        {
            //you production domain logic here, what your app does
        }

        public void Login()
        {
            //you production domain logic here, what your app does
        }
    }

    public interface ICustomer
    {
        void Register();
        void Confirm_Registration();
        void Login();
    }
}
