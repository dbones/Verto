namespace Verto.IdGeneration
{
    /// <summary>
    /// this represents the Hilo entry (where to look for the Hilo and the current Hi and lo for an entity)
    /// </summary>
    public class HiloEntry
    {
        /// <summary>
        /// the name of the entity this Hilo is for
        /// </summary>
        public string Name { get; set; }
        internal int NextId { get; set; }
        public int? CurrentHi { get; set; }

        public HiloEntry()
        {
            Where = "where Entity = '{0}'";
        }

        /// <summary>
        /// table which store the Hilo
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// the column name for the Hi
        /// </summary>
        public string HiColumn { get; set; }
        /// <summary>
        /// the max low
        /// </summary>
        public int MaxLow { get; set; }
        /// <summary>
        /// the where clause
        /// </summary>
        public string Where { get; set; }

    }
}