namespace Xlent.Lever.Libraries2.Core.Translation
{
    /// <summary>
    /// Interface that is required for a model to be translatable.
    /// </summary>
    public interface ITranslatable
    {
        /// <summary>
        /// Call the decorate method for the <paramref name="translator"/> for each value that should be decorated in the model.
        /// </summary>
        void DecorateForTranslation(Translator translator);
    }
}