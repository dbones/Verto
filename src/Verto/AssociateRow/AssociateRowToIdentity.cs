namespace Verto.AssociateRow
{
    using System.Data;
    using System.Data.SqlClient;
    using Rhino.Etl.Core;

    /// <summary>
    /// this will set a parameter to be an output parameter, returning the last Identity, will not work with batch commands
    /// </summary>
    public class AssociateRowToIdentity : IAssociateRowTo
    {
        private readonly string _paramName;

        public AssociateRowToIdentity(string paramName)
        {
            _paramName = paramName.StartsWith("@") ? paramName : string.Format("@{0}", paramName);
        }

        /// <summary>
        /// use this to prepare the SQL command
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="cmd">sql command</param>
        public void PrepareCommand(Row row, SqlCommand cmd)
        {
            cmd.CommandText = CreateCommand();
            AddParameters(row, cmd);
        }

        public void AddParameters(Row row, SqlCommand cmd)
        {
            var sqlParameter = cmd.CreateParameter();
            sqlParameter.Direction = ParameterDirection.Output;
            sqlParameter.ParameterName = _paramName;
            sqlParameter.DbType = DbType.Int32;
            cmd.Parameters.Add(sqlParameter);
        }

        public string CreateCommand()
        {
            return string.Format("SET {0}=SCOPE_IDENTITY();", _paramName);
        }
    }
}