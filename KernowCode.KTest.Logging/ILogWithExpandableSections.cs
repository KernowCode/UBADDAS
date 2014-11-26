using System.Drawing;

namespace KernowCode.KTest.Logging
{
    public interface ILogWithExpandableSections
    {
        void SetStartTextsToHaveSectionOpen(params string[] texts);
    }
}