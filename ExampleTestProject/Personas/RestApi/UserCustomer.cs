using System;
using System.Diagnostics;
using MyApplication.Domain.Entities;

namespace TestProject.Personas.RestApi
{
    public partial class WebUser : ICustomer
    {
        public Customer Customer { get; set; }

        public void Register()
        {
            Debug.WriteLine(string.Format("Rest Api layer regsiter step would register {0} customer email.", Customer.Email));
        }

        public void Confirm_Registration()
        {
            Debug.WriteLine("Rest Api layer confirm registration step would check email send/recieved using fake smtp server");
        }

        public void Login()
        {
            Debug.WriteLine("Rest Api layer login step would enter email and password into inputs and submit");
        }
    }
}