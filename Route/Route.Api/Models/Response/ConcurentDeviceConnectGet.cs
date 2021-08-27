using StarSg.Core;

namespace Route.Api.Models.Response
{
    public class ConcurentDeviceConnect
    {
        public int Concurent { get; set; }
        public int MaxConnect { get; set; }
    }
    public class ConcurentDeviceConnectGet:BaseResponse
    {
         public ConcurentDeviceConnect Data { get; set; }
    }
}