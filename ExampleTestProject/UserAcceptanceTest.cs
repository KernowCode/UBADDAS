using System.Diagnostics;
using KernowCode.KTest.Ubaddas.Example;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KernowCode.KTest.Ubaddas;
using MyApplication.Domain.Entities;
using TestProject.Personas;

namespace TestProject
{
    [TestClass]
    public class Test1 : ExampleTestFeatureBase
    {
        [TestMethod]
        public void IWantToRegisterANewUserA()
        {
            //Demonstrates basic usage                        
            ICustomer customer = new Customer("myTestEmail@mail.com");

            SoThat(MyBusinessValue.WeIncreaseTheCustomerBase)
                .As(new WebUser())
                .Given(customer.Register)
                .When(customer.Confirm_Registration)
                .Then(customer.Login);
        }

        [TestMethod]
        public void IWantToRegisterANewUserB()
        {
            //Demonstrates more descriptive naming of personas and entities            
            var user = new WebUser().Named("a web user"); //naming by hand
            ICustomer newCustomer = new Customer("myTestEmail@mail.com");
            newCustomer.Named(() => newCustomer); //naming from variable name

            SoThat(MyBusinessValue.WeIncreaseTheCustomerBase)
                .As(user)
                .Given(newCustomer.Register)
                .When(newCustomer.Confirm_Registration)
                .Then(newCustomer.Login);
        }

        [TestMethod]
        public void IWantToRegisterANewUserC()
        {
            //Demonstrates inline naming and renaming through test
            var user = new WebUser();
            ICustomer newCustomer = new Customer("myTestEmail@mail.com");            

            SoThat(MyBusinessValue.WeIncreaseTheCustomerBase)
                .As(user.Named("The user"))
                .Given(newCustomer.Named("customer for the first time").Register)
                .When(newCustomer.Named("via email customer").Confirm_Registration)
                .Then(newCustomer.Named("returning customer").Login);
        }

        [TestMethod]
        public void ShouldLoginAsANewUserD() //'Should' gets renamed to 'I Want To'
        {            
            ICustomer customer = new Customer("myTestEmail@mail.com");            

            SoThat(MyBusinessValue.WeIncreaseTheCustomerBase)
                .As(new WebUser())
                .GivenWe(x => FullyRegister(x, customer)) //delegate call to do a set of actions                
                .When(customer.Confirm_Registration)                
                .Then(customer.Login);
        }

        private void FullyRegister(ISet behaviour, ICustomer customer)
        {
            Debug.WriteLine("Perform full customer registration");            
            behaviour.Perform()
                .Given(customer.Register);
                //.And(customer...)
        }
    }
}
