namespace Verto
{
    using System;

    public static class ObjectExtensions
    {
        /// <summary>
        /// deals with the row value being null, this cannot be passed to a db as null, so it will turn a null into DBNull 
        /// </summary>
        public static object ReturnDbValue(this object instance)
        {
            return instance ?? DBNull.Value;
        }
    }
}