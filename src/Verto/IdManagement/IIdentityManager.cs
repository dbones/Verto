namespace Verto.IdManagement
{
    using System.Collections.Generic;

    /// <summary>
    /// this is responsible for all the ids, creating new ids for the destination system and mapping to the old system
    /// </summary>
    /// <remarks>
    /// you could use the default IdentityManager, which uses an in-memory container,
    /// or you can create your own for example, one which uses a MDM store/service to handle the identities</remarks>
    public interface IIdentityManager
    {
        /// <summary>
        /// add the ids of both the source and destination system (this should be used with Identity )
        /// </summary>
        /// <param name="entity">the entity which the id's belong too.</param>
        /// <param name="oldId">the source system id</param>
        /// <param name="newId">the destination system id</param>
        void Add(string entity, object oldId, object newId);

        /// <summary>
        /// returns all the mapped ids for a given entity
        /// </summary>
        /// <param name="entity">the entity of which you want all the mapped ids for</param>
        /// <returns>all the known mapped Ids for the entity</returns>
        IEnumerable<Identity> GetAllIds(string entity);

        /// <summary>
        /// from the entity and the old id (source id), this will return the new id (destination id)
        /// </summary>
        /// <param name="entity">the entity of which the id belongs to</param>
        /// <param name="oldId">the source id</param>
        /// <returns>the destination id</returns>
        object GetNewId(string entity, object oldId);

        /// <summary>
        /// create an identity for the destination system
        /// </summary>
        /// <param name="entity">the entity to generate an id for (this is the entity with the Id property on it directly not a subclass)</param>
        /// <param name="oldId">the source id, this will be used to map the source id to the destination id</param>
        /// <returns>the new destination id</returns>
        object CreateId(string entity, object oldId);
    }
}