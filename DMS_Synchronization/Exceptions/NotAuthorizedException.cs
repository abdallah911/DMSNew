namespace DMS_Synchronization.Exceptions
{
    using System;

    public partial class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(Exception ex) : base("Invlid token.", ex)
        {

        }
    }
}
