using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Helper
{
    public class DocumentSetting
    {
        public static string UploadFile(HttpPostedFileBase File, string FolderPath)
        {
            if(File != null)
            {

                //2- Get a unique name for the CV file
                string FileName = Guid.NewGuid() + Path.GetFileName(File.FileName);
                //3- the whole file path
                string FilePath = Path.Combine(FolderPath, FileName);
                //4- save the file as stream ,, File is an unmanaged resourse so we use using
                using (var fs = new FileStream(FilePath, FileMode.Create)) //try{var fs = new FileStream(FilePath, FileMode.Create)} finally{fs.dispose();}
                {
                    using (var inputStream = File.InputStream)
                    {
                        inputStream.CopyTo(fs);
                    }
                }
                return FileName;
            }
            return "None";
        }
        //public HttpPostedFileBase ReadFileToHttpPostedFileBase(string filePath)
        //{
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        //    string fileName = System.IO.Path.GetFileName(filePath);

        //    MemoryStream ms = new MemoryStream(fileBytes);
        //    HttpPostedFileBase httpPostedFile = new HttpPostedFileWrapper(
        //        new System.Web.HttpPostedFileBase
        //        {
        //            InputStream = ms,
        //            FileName = fileName,
        //            ContentLength = fileBytes.Length,
        //            ContentType = MimeMapping.GetMimeMapping(fileName)
        //        });

        //    return httpPostedFile;
        //}
        public static void DeleteFile(string FolderName, string FileName)
        {
            string FullPath = Path.Combine(Directory.GetCurrentDirectory(), FolderName, FileName);
            if (File.Exists(FullPath))
            {
                File.Delete(FullPath);

            }
        }
    }
}