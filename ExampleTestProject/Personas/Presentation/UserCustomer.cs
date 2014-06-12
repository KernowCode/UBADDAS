using System;
using System.Diagnostics;
using MyApplication.Domain.Entities;

namespace TestProject.Personas.Presentation
{
    public partial class User : ICustomer
    {
        public Customer Customer { get; set; }

        public void Register()
        {
            Debug.WriteLine(string.Format("Presentation layer regsiter step would register {0} customer email.", Customer.Email));
        }

        public void Confirm_Registration()
        {
            Debug.WriteLine("Presentation layer confirm registration step would check email send/recieved using fake smtp server");
        }

        public void Login()
        {
            Debug.WriteLine("Presentation layer login step would enter email and password into inputs and submit");
        }
    }
}