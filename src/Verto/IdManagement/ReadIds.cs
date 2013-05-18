namespace Verto.IdManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using Rhino.Etl.Core;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// this reads all the old and new ids into rows
    /// </summary>
    public class ReadIds : AbstractOperation
    {
        private readonly IIdentityManager _identityManager;
        private readonly string _entity;

        /// <summary>
        /// read all the ids for a given entity, placing them into their own row
        /// </summary>
        /// <param name="identityManager">the identity manager to use</param>
        /// <param name="entity">entity to read ids from</param>
        public ReadIds(IIdentityManager identityManager, string entity)
        {
            _identityManager = identityManager;
            _entity = entity;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            return _identityManager.GetAllIds(_entity).Select(Row.FromObject);
        }
    }
}