namespace Verto.AssociateRow
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Text;
    using Rhino.Etl.Core;

    /// <summary>
    /// Associates a single row to many IAssociateRowTo's, i.e. many table inserts (mainly to handle inheritance hierarchies)
    /// For example pass in several AssociateRowTo&gt;T&lt; and this will combine them all into a single command
    /// This will work with batch statements
    /// </summary>
    public class AssociateRowToMany : IAssociateRowTo
    {
        private readonly IEnumerable<IAssociateRowTo> _associateRowTos;
        private string _command;

        public AssociateRowToMany(IEnumerable<IAssociateRowTo> associateRowTos)
        {
            _associateRowTos = associateRowTos;
        }

        public AssociateRowToMany(params IAssociateRowTo[] associateRowTos)
        {
            _associateRowTos = associateRowTos;
        }

        /// <summary>
        /// use this to prepare the SQL command
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="cmd">sql command</param>
        public void PrepareCommand(Row row, SqlCommand cmd)
        {
            if (string.IsNullOrEmpty(_command))
            {
                _command = CreateCommand();
            }

            cmd.CommandText = _command;
            AddParameters(row, cmd);
        }

        public void AddParameters(Row row, SqlCommand cmd)
        {
            foreach (IAssociateRowTo associateRowTo in _associateRowTos)
            {
                associateRowTo.AddParameters(row, cmd);
            }
        }

        public string CreateCommand()
        {
            StringBuilder command = new StringBuilder();
            foreach (IAssociateRowTo associateRowTo in _associateRowTos)
            {
                command.AppendLine(associateRowTo.CreateCommand());
            }
            return command.ToString();
        }
    }
}