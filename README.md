UBADDAS
=======

**User Behavior and Domain Driven Acceptance Stories**

*This test framework concisely and coherently brings together the concepts of User Stories, Behaviour Driven Development, and Domain Driven Design.  It simply provides a means to author test steps as Personas for each Entity Command and executes these methods throught the test with succinct output of test results to the console.*

A C# Test Framework combining
* UAT - As A, I Want, So That
* BDD - Given, When, Then
* DDD - Entities and Personas

Benefits
* Concise - combines test method naming with User Story and BDD syntaxes
* Guiding - consume first testing with strict structure helps discover the domain entities and commands
* Readable - single appropriately named methods result in simple to read tests and output
* Reusable - delegate calls to methods aid reusability
* Agnostic - specify architecture layer to target test i.e. Presentation, Web API.

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


