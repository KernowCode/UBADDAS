using System;

namespace KernowCode.KTest.Ubaddas
{
    public interface IState
    {
        bool Narrate { get; set; }
        Type CurrentPersonaType { get; set; }
    }
}