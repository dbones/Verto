namespace Verto.AssociateRow
{
    using System.Data.SqlClient;
    using Rhino.Etl.Core;

    /// <summary>
    /// provide a way to set up the association between a row and something else (object(s))
    /// </summary>
    public interface IAssociateRowTo
    {
        /// <summary>
        /// use this to prepare the SQL command (apply the command and params to the command)
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="cmd">sql command</param>
        void PrepareCommand(Row row, SqlCommand cmd);

        /// <summary>
        /// adds the parameters to the command, nothing else
        /// </summary>
        void AddParameters(Row row, SqlCommand cmd);

        /// <summary>
        /// create the command text to be applied to the command
        /// </summary>
        string CreateCommand();
    }
}