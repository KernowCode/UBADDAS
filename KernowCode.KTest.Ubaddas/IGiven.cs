using System;

namespace KernowCode.KTest.Ubaddas
{
    public interface IGiven : ITense
    {
        IWhen When(Action domainEntityCommand);
        IWhen When(Action<ISet> actionDelegate);
    }
}