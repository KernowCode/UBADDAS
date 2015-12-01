using System.Diagnostics;

namespace MyApplication.Domain.Entities
{
    public class Customer : ICustomer
    {
        public string Email { get; set; }

        public Customer(string email)
        {
            Email = email;
        }

        public void Registration()
        {
            Debug.WriteLine("Production persists User email");
        }

        public void Login()
        {
            Debug.WriteLine("Production authenticate user email address and password");
        }
    }
}