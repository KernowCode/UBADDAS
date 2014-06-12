UBADDAS
=======

**User Behavior and Domain Driven Acceptance Stories**

*This test framework concisely and coherently brings together the concepts of User Stories, Behaviour Driven Development, and Domain Driven Design. It simply provides a means to author test steps as Personas for each Entity Command. It executes these methods targeting the application layer of choice, resulting in succinct output of test results to the console.  The result is abstract and reusable behaviours that do not need to be rewritten to support other application layers or event platform implementations.*

A C# Test Framework combining
* As A, I Want, So That (User Stories)
* Given, When, Then (BDD)
* Entities and Personas (DDD)

Benefits
* Concise - combines test method naming with User Story and BDD syntaxes
* Guiding - consume first testing with strict structure helps discover the domain entities and commands
* Readable - single appropriately named methods result in simple to read tests and output
* Reusable - target application layers or even platform implementations, plus includes calls to delegate methods

Example console output
```C#
I want to register a new user
  So that Increase customer base
       As user
    Given Register customer
     When Confirm customer registration
     Then Login customer
```
Usage - Test Code
```C#
[Test]
public void IWantToRegisterANewUser() {
  var user = new User();
  ICustomer customer = new Customer();
  SoThat(MyBusinessValue.IncreaseCustomerBase)
      .As(user)
      .Given(customer.Register)
      .When(customer.Confirm_Registration)
      .Then(customer.Login);
}

public enum MyBusinessValue {
  IncreaseCustomerBase
}

public partial class User : IPersona {
}

public partial class User : ICustomer {
  public Customer Customer { get; set; } //injected by framework
  public void Register() {
    // your test code here. i.e. selenium or rest api calls
    var email = Customer.Email; //i.e. For registering with app
  }
  // other interface implementations
}
```
Usage - Production Code
```C#
public interface ICustomer {
  void Register()
  void Confirm_Registration();
  void Login();
}

public class Customer : ICustomer {
  public void Register() {
    // your application domain logic
  }
  // other interface implementations
}


