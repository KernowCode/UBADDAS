using KernowCode.KTest.Ubaddas;

namespace TestProject.Personas
{
    public class TestCustomer : ITestCustomer
    {
        public Verb Verb { get; set; }

        public string Email { get; set; }

        public TestCustomer(string email)
        {
            Email = email;
        }        

        public void Registration()
        {
        }

        public void Login()
        {
        }
    }
}
