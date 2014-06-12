using System;

namespace KernowCode.KTest.Ubaddas
{
    public interface IWhen : ITense
    {
        IThen Then(Action domainEntityCommand);
        IThen Then(Action<ISet> actionDelegate);
    }
}