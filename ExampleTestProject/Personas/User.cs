using KernowCode.KTest.Logging;
using KernowCode.KTest.Ubaddas;
using MyApplication.Domain.Entities;

namespace TestProject.Personas
{
    public class WebUser : IPersona, IUseVerbs
    {
        public Verb Verb { get; set; }
        public ILogger Log { get; set; }
        public TestCustomer TestCustomerEntity { get; set; }                
    }
}