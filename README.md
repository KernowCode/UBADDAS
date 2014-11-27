UBADDAS
=======

**User Behavior and Domain Driven Acceptance Stories**

*This test framework concisely and coherently brings together the concepts of User Stories, Behaviour Driven Development, and Domain Driven Design. It simply provides a means to author test steps as Personas for each Entity Command. It executes these methods targeting the application layer of choice, resulting in succinct output of test results to the console.  The results are abstract and reusable behaviours that do not need to be rewritten to support other application layers and even different platform implementations.*

Read the [Whole Story](http://kernowcode.wordpress.com/2014/06/19/abc-2/)

A C# Test Framework combining
* As A, I Want, So That (User Stories)
* Given, When, Then (BDD)
* Entities and Personas (DDD)

Benefits
* Concise - combines test method naming with User Story and BDD syntaxes 
* Guiding - following test driven development with strict structure helps discover the domain entities and commands
* Coupled - but in a good way that restricts over specification
* Readable - single appropriately named methods result in simple to read tests and output
* Reusable - target testing against different application layers or even platform implementations

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
namespace TestProject
{
  [TestClass]
  public class MyTest : StoryFeatureBase<MyBusinessValue>
  {
    public override string ApplicationLayer()
    {
      return "PresentationLayer"; //could be other i.e. WebApi, possibly provided by App.Config
    }
    
    [Test]
    public void IWantToRegisterANewUser()
    {
      var user = new User();
      ICustomer customer = new Customer();
      SoThat(MyBusinessValue.IncreaseCustomerBase)
        .As(user)
        .Given(customer.Register)
        .When(customer.Confirm_Registration)
        .Then(customer.Login);
    }
  
    public enum MyBusinessValue
    {
      IncreaseCustomerBase
    }
  }
}

namespace TestProject.Personas
{
  public partial class User : IPersona
  {
  }
}

namespace TestProject.Personas.PresentationLayer
{
  public partial class User : ICustomer
  {
    public Customer Customer { get; set; } //injected by framework
    public void Register()
    {
      // your test code here. i.e. selenium
      var email = Customer.Email; //i.e. For registering with app
    }
    // other interface implementations
  }
}

namespace TestProject.Personas.WebApi
{
  public partial class User : ICustomer
  {
    public Customer Customer { get; set; } //injected by framework
    public void Register()
    {
      // your test code here. i.e. rest api calls
    }
    // other interface implementations
  }
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
```

**V1.1.1 - Improved logging indentation and added HTML logging output**

*You can implement your own loggers based on ILogger and configure them for tests.  You can now write log events from within your code blocks*

Test Fixture Setup
```C#
[TestInitialize]
public void TestSetup()
{
  base.LoggerFactory = MyCustomLoggerFactory;
}

private List<ILogger> MyCustomLoggerFactory()
{
  var loggers = new List<ILogger>();            
  loggers.Add(new ConsoleLogger(Behaviour.LeftSectionPadding));
  loggers.Add(new HtmlLogger(GetTestOutputFilePath("html")));                        
  return loggers;
}
```

Using Logging
```C#
public class User : ICustomer
{
  public ILogger Log { get; set; } //Add this line to use Log later in you test implementation
  public Customer Customer { get; set; }

  public void Register()
  {
    Log.WriteLine("Presentation layer test implementation - register user");
  }
}
```

The output will detail the story {I want, so that, as a}, behaviour {given, when, then}, and the detail from your Log uses
