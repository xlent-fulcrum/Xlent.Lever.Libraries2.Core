using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.MoveTo.Core.ServerTranslators
{
    /// <summary>
    /// Decorate values from server and translate concept values to server.
    /// </summary>
    public abstract class ServerTranslatorBase
    {
        /// <summary>
        /// The concept name for the id. Is used for translations of id parameters and id results.
        /// </summary>
        protected string IdConceptName { get; }

        /// <summary>
        /// The service that should carry out the actual translation of values decorated into concept values.
        /// </summary>
        protected ITranslatorService TranslatorService { get; }

        // TODO: Read client from context
        /// <summary>
        /// The name of the server. Is used for decorating values from the server and for translating to the server.
        /// </summary>
        protected System.Func<string> GetServerNameMethod { get; }

        /// <summary>
        /// Set up a new client translator.
        /// </summary>
        /// <param name="idConceptName">The <see cref="IdConceptName"/>.</param>
        /// <param name="getServerNameFunction">The <see cref="GetServerNameMethod"/>.</param>
        /// <param name="translatorService">The <see cref="TranslatorService"/>.</param>
        protected ServerTranslatorBase(string idConceptName, System.Func<string> getServerNameFunction, ITranslatorService translatorService)
        {
            IdConceptName = idConceptName;
            TranslatorService = translatorService;
        }

        /// <summary>
        /// Returns a new translator for a server. 
        /// </summary>
        protected Translator CreateTranslator()
        {
            return new Translator(GetServerNameMethod(), TranslatorService);
        }
    }
}