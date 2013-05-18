namespace Verto.IdGeneration
{
    /// <summary>
    /// generates ids for a given entity (the entity should have the id on it directly, ie not a subclass)
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// generate an id
        /// </summary>
        /// <param name="entity">the entity to generate an id for</param>
        /// <returns>a id for the entity</returns>
        object GenerateKey(string entity);
    }
}