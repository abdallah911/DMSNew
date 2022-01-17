using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DMS_Synchronization.Controllers
{
    public class SyncController : ApiController
    {
        // POST api/Sync/{Push-Pull-Both}
        //public string Post(string operation)
        //{
        //    return operation;

        //}

        [HttpPost]
        [Route("Sync")]
        public IHttpActionResult Post(string operation)
        {
            
            try
            {
                var result = SyncManager.Sync(operation);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message = message + " Error: " + ex.InnerException.Message;
                }
                return BadRequest(message);
            }
        }


    }
}
