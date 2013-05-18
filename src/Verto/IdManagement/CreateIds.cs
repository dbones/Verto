namespace Verto.IdManagement
{
    using System.Collections.Generic;
    using Rhino.Etl.Core;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// this will create a new id (to be used in the destination) for every row
    /// </summary>
    public class CreateIds : AbstractOperation
    {
        private readonly IdentityManager _identityManager;
        private readonly string _entity;
        private readonly string _oldIdColumn;
        private readonly string _newIdColumn;

        /// <summary>
        /// creates a new Id to be used in the destination system
        /// </summary>
        /// <param name="identityManager">the identity manager</param>
        /// <param name="entity">entity which the id belongs to</param>
        /// <param name="oldIdColumn">the old id column name</param>
        /// <param name="newIdColumn">the new id column name, this will be appended onto each row</param>
        public CreateIds(IdentityManager identityManager, string entity, string oldIdColumn, string newIdColumn)
        {
            _identityManager = identityManager;
            _entity = entity;
            _oldIdColumn = oldIdColumn;
            _newIdColumn = newIdColumn;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows)
            {
                var id = _identityManager.CreateId(_entity, row[_oldIdColumn]);
                row[_newIdColumn] = id;
                yield return row;
            }
        }
    }
}
