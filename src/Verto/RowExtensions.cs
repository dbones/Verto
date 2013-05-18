namespace Verto
{
    using System.Collections;
    using Rhino.Etl.Core;

    public static class RowExtensions
    {
        /// <summary>
        /// appends all the columns of a row to the current row.
        /// </summary>
        public static void Append(this Row current, Row appendFrom)
        {
            var enumerator = (IDictionaryEnumerator)appendFrom.GetEnumerator();
            while (enumerator.MoveNext())
            {
                current[enumerator.Key] = enumerator.Value;
            }
        }
    }
}