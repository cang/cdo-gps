using System;
using System.Collections;
using System.Collections.Generic;
using Route.Core;
using StarSg.Core;

namespace Route.Api.Models.Response
{
    public class DatacenterRespose
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public IList<int> ReportServerId { get; set; } 
    }
    public class DatacenterGet:BaseResponse
    {
         public IList<DatacenterRespose> Data { get; set; }
    }
}