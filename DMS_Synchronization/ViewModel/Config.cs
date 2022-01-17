
namespace DMS_Synchronization
{
    public class Config
    {
        public Config()
        {
            SyncDirection = SyncDirection.Sync;
            DateFormat = "dd/MM/yyyy";
        }

        public SyncDirection SyncDirection { get; set; }
        public string DateFormat { get; set; }
    }
}
