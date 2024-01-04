using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Helper
{
    public class FileUploadWrapper : HttpPostedFileBase
    {
        private readonly string filePath;

        public FileUploadWrapper(string filePath)
        {
            this.filePath = filePath;
        }

        public override int ContentLength => (int)new FileInfo(filePath).Length;

        public override string FileName => Path.GetFileName(filePath);

        public override Stream InputStream => new FileStream(filePath, FileMode.Open, FileAccess.Read);

        public override string ContentType => MimeMapping.GetMimeMapping(filePath);
    }

}