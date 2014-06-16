using System;

namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// <para>Enables a test class to use the UBADDAS test syntax</para>
    /// <para>An Enum representing the User Stories business values must be supplied</para>
    /// <para></para>
    /// </summary>
    /// <typeparam name="TBusinessValueEnum"></typeparam>
    public class StoryFeatureBase<TBusinessValueEnum>
    {
        /// <summary>
        /// <para>Gets the name of the target application layer to test against.</para>
        /// <para>Override this in your test or base test feature class</para>
        /// <para>Returning null will result in using the root Persona class in the Personas namespace</para>
        /// </summary>
        /// <returns>Persona namespace indicating the application layer. i.e. 'Presentation' which will translate to the namespace yourProject.Personas.Presentation.thePersona class</returns>
        protected virtual string ApplicationLayer()
        {
            return null;
        }
        
        /// <summary>
        /// <para>Initial method to start new User Story and BDD syntax.</para>
        /// <para>Uses test method name as the 'I Want' part of the User Story syntax.</para>
        /// <para>Follow this 'SoThat' with the 'As' to specify the 'As A' part of the User Story syntax.</para>
        /// </summary>
        /// <param name="businessValue">Supply an enumerator that describes the business values for the User Stories</param>
        /// <returns>Fluent interface providing BDD Given, When, Then syntax</returns>
        protected IBase SoThat(TBusinessValueEnum businessValue)
        {
            return Behaviour.SoThat(businessValue.ToString(), ApplicationLayer());
        }
    }
}
