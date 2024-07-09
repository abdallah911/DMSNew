using DMS_Authontication1.Models;
using DMS_TEST;
using DMS_TEST.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class ClaimPhotoesViewModel
    {
        public int Id { get; set; }
        public string CardId { get; set; }
        public long ClaimNumber { get; set; }
        public string Url { get; set; }
        public bool? IsDispense { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }

    }
}