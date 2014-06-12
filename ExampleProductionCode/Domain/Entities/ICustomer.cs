namespace MyApplication.Domain.Entities
{
    public interface ICustomer
    {
        void Register();
        void Confirm_Registration();
        void Login();
    }
}