using Xlent.Lever.Libraries2.MoveTo.Core.Translation;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.ClientTranslators
{
    /// <summary>
    /// Decorate values from client and translate concept values to client.
    /// </summary>
    public abstract class ClientTranslatorBase
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
        /// The name of the client. Is used for decorating values and for translating back to client.
        /// </summary>
        protected System.Func<string> GetClientNameMethod { get; }

        /// <summary>
        /// Set up a new client translator.
        /// </summary>
        /// <param name="idConceptName">The <see cref="IdConceptName"/>.</param>
        /// <param name="getClientNameMethod">The <see cref="GetClientNameMethod"/>.</param>
        /// <param name="translatorService">The <see cref="TranslatorService"/>.</param>
        protected ClientTranslatorBase(string idConceptName, System.Func<string> getClientNameMethod, ITranslatorService translatorService)
        {
            IdConceptName = idConceptName;
            GetClientNameMethod = getClientNameMethod;
            TranslatorService = translatorService;
        }

        /// <summary>
        /// Returns a new translator for a client. 
        /// </summary>
        protected Translator CreateTranslator()
        {
            return new Translator(GetClientNameMethod(), TranslatorService);
        }
    }
}