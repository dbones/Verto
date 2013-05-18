namespace Verto.IdGeneration
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;

    /// <summary>
    /// generate Id's using Hilo, should be compatible with NHibernate
    /// </summary>
    public class HiloIdGenerator : IIdGenerator
    {
        private readonly string _connectionName;
        /// <summary>
        /// default Table name (which stores the Hilo)
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// default Hi Column
        /// </summary>
        public string HiColumn { get; set; }

        /// <summary>
        /// default max
        /// </summary>
        public int MaxLow { get; set; }

        /// <summary>
        /// default where
        /// </summary>
        public string Where { get; set; }

        private readonly IDictionary<string, HiloEntry> _hilos;

        public HiloIdGenerator(string connectionName)
        {
            _connectionName = connectionName;
            TableName = "Hilo";
            HiColumn = "NextHiValue";
            MaxLow = 1000;
            Where = "where Entity = '{0}'";
            _hilos = new Dictionary<string, HiloEntry>();

        }

        /// <summary>
        /// set up a Hilo entry directly
        /// </summary>
        /// <param name="hiloEntry"></param>
        public void AddTableHilo(HiloEntry hiloEntry)
        {
            _hilos.Add(hiloEntry.Name, hiloEntry);
        }

        public object GenerateKey(string entity)
        {
            if (!_hilos.ContainsKey(entity))
            {
                var hilo = new HiloEntry
                               {
                                   Name = entity,
                                   HiColumn = HiColumn,
                                   MaxLow = MaxLow,
                                   TableName = TableName,
                                   Where = Where
                               };
                _hilos.Add(entity, hilo);
            }

            var tableHilo = _hilos[entity];

            if (!tableHilo.CurrentHi.HasValue || tableHilo.NextId > MaxLow)
            {
                tableHilo.CurrentHi = GetHi(tableHilo);
                tableHilo.NextId = 1;
            }

            int id = tableHilo.NextId + (tableHilo.CurrentHi.Value * MaxLow);
            tableHilo.NextId++;

            return id;

        }

        /// <summary>
        /// gets the next hi
        /// </summary>
        /// <param name="hiloEntry">the Hilo entry to get the hi for</param>
        protected virtual int GetHi(HiloEntry hiloEntry)
        {
            using (var conn = CreateConnection(_connectionName))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = SqlCmdText(hiloEntry);
                var idParameter = cmd.CreateParameter();
                idParameter.ParameterName = IdParamName();
                idParameter.Direction = ParameterDirection.Output;
                idParameter.DbType = DbType.Int32;
                cmd.Parameters.Add(idParameter);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return (int)idParameter.Value;
            }
        }

        /// <summary>
        /// command to read and update the Hilo entry
        /// </summary>
        /// <param name="hiloEntry">the hilo entry to create the command for</param>
        private string SqlCmdText(HiloEntry hiloEntry)
        {
            string where = string.IsNullOrEmpty(Where)
                               ? ""
                               : string.Format(hiloEntry.Where, hiloEntry.Name);

            StringBuilder cmd = new StringBuilder();
            cmd.AppendFormat("SELECT {3} = {0} FROM {1} {2};", hiloEntry.HiColumn, hiloEntry.TableName, where, IdParamName());
            cmd.AppendFormat("\r\nUPDATE {0} SET {1} = {3} + 1 {2};", hiloEntry.TableName, hiloEntry.HiColumn, where, IdParamName());
            return cmd.ToString();
        }



        /// <summary>
        /// name of the Id parameter (@Id), override if your db used a different prefix
        /// </summary>
        /// <returns></returns>
        protected virtual string IdParamName()
        {
            //a little over the top, but thought if someone was using MySql or another DB provider they should override this.
            return "@Id";
        }

        /// <summary>
        /// provides the connection to use, not opened, default is MSSQL. Override to provide any other db connection 
        /// </summary>
        /// <param name="connectionName">creates a new connection (do not open)</param>
        protected virtual IDbConnection CreateConnection(string connectionName)
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString);
        }



    }
}