namespace KernowCode.KTest.Ubaddas
{
    /// <summary>
    /// Allows a subset of behaviour to be performed
    /// </summary>
    public interface ISet
    {
        /// <summary>
        /// Starts the User Story BDD behaviour for a subset of behaviour
        /// </summary>
        /// <returns>User Story initiator to provide 'As' statement</returns>
        IBase Perform();
    }
}