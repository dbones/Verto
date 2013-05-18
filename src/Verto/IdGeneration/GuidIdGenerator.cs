namespace Verto.IdGeneration
{
    using System;

    /// <summary>
    /// generate Id's using Guids
    /// </summary>
    public class GuidIdGenerator : IIdGenerator
    {
        public object GenerateKey(string entity)
        {
            return Guid.NewGuid();
        }
    }
}