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
            var customer = new TestCustomer("myTestEmail@mail.com");

            SoThat(MyBusinessValue.WeIncreaseTheCustomerBase)
                .As(new WebUser())
                .Given(customer.Has("completed").Registration)
                .When(customer.Has("confirmed").Registration)
                .Then(customer.Login);
        }

        [TestMethod]
        public void IWantToRegisterANewUserB()
        {
            //Demonstrates more descriptive naming of personas and entities            
            var user = new WebUser().Named("a web user"); //naming by hand
            var newCustomer = new TestCustomer("myTestEmail@mail.com");
            newCustomer.Named(() => newCustomer); //naming from variable name

            SoThat(MyBusinessValue.WeIncreaseTheCustomerBase)
                .As(user)
                .Given(newCustomer.Has("completed").Registration)
                .When(newCustomer.Has("confirmed").Registration)
                .Then(newCustomer.Login);
        }

        [TestMethod]
        public void IWantToRegisterANewUserC()
        {
            //Demonstrates inline naming and renaming through test
            var user = new WebUser();
            var newCustomer = new TestCustomer("myTestEmail@mail.com");            

            SoThat(MyBusinessValue.WeIncreaseTheCustomerBase)
                .As(user.Named("The user"))
                .Given(newCustomer.Named("customer for the first time").Has("completed").Registration)
                .When(newCustomer.Named("via email customer").Has("confirmed").Registration)
                .Then(newCustomer.Named("returning customer").Login);
        }

        [TestMethod]
        public void ShouldLoginAsANewUserD() //'Should' gets renamed to 'I Want To'
        {            
            var customer = new TestCustomer("myTestEmail@mail.com");            

            SoThat(MyBusinessValue.WeIncreaseTheCustomerBase)
                .As(new WebUser())
                .GivenWe(x => FullyRegister(x, customer)) //delegate call to do a set of actions                
                .When(customer.Has("confirmed").Registration)                
                .Then(customer.Login);
        }

        private void FullyRegister(ISet behaviour, ITestCustomer customer)
        {
            Debug.WriteLine("Perform full customer registration");            
            behaviour.Perform()
                .Given(customer.Has("fully completed").Registration);
                //.And(customer...)
        }
    }
}
