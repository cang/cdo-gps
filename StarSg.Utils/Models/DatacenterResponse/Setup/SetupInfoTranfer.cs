using System;

namespace StarSg.Utils.Models.DatacenterResponse.Setup
{
    public class SetupInfoTranfer
    {
        public virtual long Id { get; set; }
        public virtual long Serial { get; set; }
        public virtual DateTime Request { set; get; }
        public virtual DateTime Response { set; get; }
        public virtual byte[] Data { set; get; }
        public virtual string UserName { set; get; }
        public virtual bool Complete { set; get; }
        public virtual string Note { set; get; }
    }
}