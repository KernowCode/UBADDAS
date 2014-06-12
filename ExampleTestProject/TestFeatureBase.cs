using KernowCode.KTest.Ubaddas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    public enum MyBusinessValue
    {
        WeIncreaseTheCustomerBase
    }

    public class TestFeatureBase : StoryFeatureBase<MyBusinessValue>
    {
        protected override string ApplicationLayer()
        {                        
            //changing the return value will result in a different target application layer to test against.
            
            return "Presentation"; //will point to the Personas/Presentation folder and associated namespace.
            //return "RestApi"; //will point to the Personas/RestApi folder and associated namespace.

            //Notice the Personas folder structure in this project and how it
            //relates to the possible values return from this method.

            //Decide on your own application layers and implement accordingly.
            //You could get the layer name from an App.Config if you prefer.
            //Your automated continuous integration could run the test with 
            //a command line argument or build with different App.Config files.
        }
    }
}
