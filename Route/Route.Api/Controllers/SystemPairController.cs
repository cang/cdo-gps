using Route.Api.Auth;
using Route.Api.Auth.Core;
using Route.Api.Auth.Core.ConfigFile;
using Route.Api.Auth.Models.Entity;
using Route.Api.Auth.Models.Req;
using Route.Api.Auth.Models.Response;
using StarSg.Core;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;

namespace Route.Api.Controllers
{
    [ValidRequestFilter]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SystemPairController : ValidController
    {
  
        [HttpGet]
        [ResponseType(typeof(BaseResponse))]
        [Route("SystemPair")]
        public IHttpActionResult GetSystemNameValue(string Id)
        {
            var ret = AccountManager.GetSystemPair(Id);
            if (ret != null)
                return Ok(new SystemPairResponse
                {
                    Status = 1,
                    Description = "OK"
                    ,
                    Data = new SystemPairTransfer()
                    { Id = ret.Id, Val = ret.Val, Note = ret.Note }
                });

            Log.Warning("SystemPairController", $"Không tồn tai obj {Id}");
            return Ok(new BaseResponse { Status = 0 });
        }

        [HttpPost]
        [ResponseType(typeof(BaseResponse))]
        [Route("SystemPair")]
        public IHttpActionResult AddSystemNameValue(SystemPairTransfer obj)
        {
            if (Account.Level <= AccountLevel.Administrator)
            {
                if (obj == null)
                {
                    Log.Warning("SystemPairController", "obj truyền vào null");
                    return Ok(new BaseResponse { Status = 0 });
                }

                var newobj = new SystemPair {
                     Id = obj.Id,
                      Val = obj.Val,
                      Note = obj.Note
                };

                if (AccountManager.AddSystemPair(newobj))
                {
                    Log.Warning("SystemPairController", $"thêm obj {obj.Id} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }

            Log.Warning("SystemPairController", $"thêm obj {obj.Id} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        [HttpPut]
        [ResponseType(typeof(BaseResponse))]
        [Route("SystemPair")]
        public IHttpActionResult UpdateSystemNameValue(SystemPairTransfer obj)
        {
            if (Account.Level <= AccountLevel.Administrator)
            {
                if (obj == null)
                {
                    Log.Warning("SystemPairController", "obj truyền vào null");
                    return Ok(new BaseResponse { Status = 0 });
                }
                var newobj = new SystemPair
                {
                    Id = obj.Id,
                    Val = obj.Val,
                    Note = obj.Note
                };

                if (AccountManager.UpdateSystemPair(newobj))
                {
                    Log.Warning("SystemPairController", $"cập nhật obj {obj.Id} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }

            Log.Warning("SystemPairController", $"cập nhật obj {obj.Id} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        [HttpDelete]
        [ResponseType(typeof(BaseResponse))]
        [Route("SystemPair")]
        public IHttpActionResult DeleteSystemNameValue(string Id)
        {
            if (Account.Level <= AccountLevel.Administrator)
            {
                if (AccountManager.DeleteSystemPair(Id))
                {
                    Log.Warning("SystemPairController", $"xóa obj {Id} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }

            Log.Warning("SystemPairController", $"xóa obj {Id} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        [HttpGet]
        [ResponseType(typeof(BaseResponse))]
        [Route("SystemPair/All")]
        public IHttpActionResult GetAllSystemNameValue()
        {
            var ret = AccountManager.AllSystemPair();
            var rets = new SystemPairResponses { Status = 1, Description = "OK" };
            foreach (var item in ret)
            {
                rets.Data.Add(new SystemPairTransfer()
                { Id = item.Id, Val = item.Val, Note = item.Note }
                );
            }
            return Ok(rets);
        }


    }


}