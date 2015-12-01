using System;
using System.Diagnostics;
using MyApplication.Domain.Entities;
using KernowCode.KTest.Logging;

namespace TestProject.Personas.Presentation
{
    public partial class WebUser : Personas.WebUser, ITestCustomer
    {
        public void Registration()
        {
            if (Verb.Value == "completed") 
                Log.WriteLine(string.Format("Presentation layer regsiter step would register {0} customer email.", TestCustomerEntity.Email));
            else if (Verb.Value == "confirmed")
                Log.WriteLine("Presentation layer confirm registration step would check email send/recieved using fake smtp server");
        }

        public void Login()
        {
            Log.WriteLine("Presentation layer login step would enter email and password into inputs and submit");
        }
    }
}