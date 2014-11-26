using System;
using System.Diagnostics;
using MyApplication.Domain.Entities;
using KernowCode.KTest.Logging;

namespace TestProject.Personas.RestApi
{
    public partial class WebUser : ICustomer
    {
        public ILogger Log { get; set; }
        public Customer Customer { get; set; }

        public void Register()
        {
            Log.WriteLine(string.Format("Rest Api layer regsiter step would register {0} customer email.", Customer.Email));
        }

        public void Confirm_Registration()
        {
            Log.WriteLine("Rest Api layer confirm registration step would check email send/recieved using fake smtp server");
        }

        public void Login()
        {
            Log.WriteLine("Rest Api layer login step would enter email and password into inputs and submit");
        }
    }
}