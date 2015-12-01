using KernowCode.KTest.Logging;

namespace KernowCode.KTest.Ubaddas
{
    public interface IUseVerbs
    {
        Verb Verb { get; set; }
    }

    public class Verb
    {
        public object Value { get; set; }
        public string Sentence { get; set; }
    }

    public static class VerbExtension
    {
        private const string HasSentenceFormat = "[subject]Has{0}[object]";

        public static T Has<T,T2>(this T subject, T2 verb) where T:IUseVerbs
        {
            subject.Verb = HelpSetCurrentTestVerb(verb, HasSentenceFormat);            
            return subject;
        }

        public static T Has<T>(this T subject) where T : IUseVerbs
        {
            subject.Verb = HelpSetCurrentTestVerb("", HasSentenceFormat);            
            return subject;
        }

        private static Verb HelpSetCurrentTestVerb<T>(T verb, string verbSentenceFormat)
        {
            return new Verb()
            {
                Value = verb,
                Sentence =
                    string.Format(verbSentenceFormat.Replace(" ", ""), verb.ToString().CapitaliseInitial())
            };
        }
    }
}
