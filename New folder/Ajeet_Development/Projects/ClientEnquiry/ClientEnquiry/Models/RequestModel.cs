using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientEnquiry.Models
{
    public class RequestModel
    {
        public int ReqId { get; set; }
        public string supportType { get; set; }
        public string priority { get; set; }
        public string department { get; set; }
        public string personName { get; set; }
        public string personMobile { get; set; }
        public string personEmail { get; set; }
        public string details { get; set; }
        public string remark { get; set; }
        public string status { get; set; }
        public string assignedTo { get; set; }
    }
}