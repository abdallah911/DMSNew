
namespace DMS_Synchronization
{
    public enum SyncDirection
    {
        /// <summary>
        /// Pull and then Push
        /// </summary>
        Sync,
        /// <summary>
        /// Pull from back office database to SE server
        /// </summary>
        Pull,
        /// <summary>
        /// Push from SE database to back office database
        /// </summary>
        Push
    }
}
