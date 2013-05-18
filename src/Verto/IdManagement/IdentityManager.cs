namespace Verto.IdManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using IdGeneration;

    /// <summary>
    /// this is responsible for all the ids, creating new ids for the destination system and mapping to the old system (this uses an in memory container to store all the ids)
    /// </summary>
    public class IdentityManager : IIdentityManager
    {
        private readonly IIdGenerator _idGenerator;
        private readonly IDictionary<string, IDictionary<object, Identity>> _identities;

        /// <summary>
        /// create an Identity manager
        /// </summary>
        /// <param name="idGenerator">the id generator the destination system uses to create new Ids</param>
        public IdentityManager(IIdGenerator idGenerator)
        {
            _idGenerator = idGenerator;
            _identities = new Dictionary<string, IDictionary<object, Identity>>(8);
        }

        /// <summary>
        /// create an identity for the destination system
        /// </summary>
        /// <param name="entity">the entity to generate an id for (this is the entity with the Id property on it directly not a subclass)</param>
        /// <param name="oldId">the source id, this will be used to map the source id to the destination id</param>
        /// <returns>the new destination id</returns>
        public object CreateId(string entity, object oldId)
        {
            var id = _idGenerator.GenerateKey(entity);
            Add(entity, oldId, id);
            return id;
        }


        /// <summary>
        /// add the ids of both the source and destination system (this should be used with Identity )
        /// </summary>
        /// <param name="entity">the entity which the id's belong too.</param>
        /// <param name="oldId">the source system id</param>
        /// <param name="newId">the destination system id</param>
        public void Add(string entity, object oldId, object newId)
        {
            if (!_identities.ContainsKey(entity))
            {
                _identities.Add(entity, new Dictionary<object, Identity>(128));
            }
            _identities[entity].Add(oldId, new Identity(oldId, newId));
        }

        /// <summary>
        /// returns all the mapped ids for a given entity
        /// </summary>
        /// <param name="entity">the entity of which you want all the mapped ids for</param>
        /// <returns>all the known mapped Ids for the entity</returns>
        public IEnumerable<Identity> GetAllIds(string entity)
        {
            return _identities[entity].Select(identity => identity.Value);
        }


        /// <summary>
        /// from the entity and the old id (source id), this will return the new id (destination id)
        /// </summary>
        /// <param name="entity">the entity of which the id belongs to</param>
        /// <param name="oldId">the source id</param>
        /// <returns>the destination id</returns>
        public object GetNewId(string entity, object oldId)
        {
            var dictionary = _identities[entity];
            if (dictionary == null)
            {
                return null;
            }

            return dictionary[oldId].NewId;
        }
    }
}