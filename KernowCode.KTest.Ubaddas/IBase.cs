using System;

namespace KernowCode.KTest.Ubaddas
{
    public interface IBase : IAs
    {
        IGiven Given(Action domainEntityCommand);
        IGiven Given(Action<ISet> actionDelegate);
    }
}