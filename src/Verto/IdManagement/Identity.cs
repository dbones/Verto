namespace Verto.IdManagement
{
    /// <summary>
    /// a simple mapping of the old id in the source system and new id in the destination system
    /// </summary>
    public class Identity
    {
        private readonly int _hash;
        public Identity(object oldId, object newId)
        {
            OldId = oldId;
            NewId = newId;
            _hash = string.Format("old:{0},new:{1}", oldId, newId).GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        /// <summary>
        /// source Id
        /// </summary>
        public object OldId { get; private set; }

        /// <summary>
        /// destination Id
        /// </summary>
        public object NewId { get; private set; }
    }
}