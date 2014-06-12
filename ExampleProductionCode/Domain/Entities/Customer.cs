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

        public void Register()
        {
            Debug.WriteLine("Production persists User email");
        }

        public void Confirm_Registration()
        {
            Debug.WriteLine("Production send confirmation email to customers email address");
        }

        public void Login()
        {
            Debug.WriteLine("Production authenticate user email address and password");
        }
    }
}