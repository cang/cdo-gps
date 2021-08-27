using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Http;
using Core.Models.Tranfer;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.DeviceModel;
using System.Linq;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin các loại xe
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceModelController : BaseController
    {
        /// <summary>
        ///     thêm thông tin model
        /// </summary>
        /// <param name="model">thông tin model</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse Add(DeviceModelTranfer model)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền thêm Loại Xe" };

            var allCenter = DataCenterStore.GetAll();
            if (allCenter == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            BaseResponse ret = new BaseResponse { Description = "Chưa xử lý" };
            foreach (var center in allCenter)
            {
                if (center == null) continue;
                try
                {
                    var myret  = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceModel/Add", model);
                    if (ret.Status == 0) ret = myret;
                }
                catch (System.Exception)
                {
                }
            }

            return ret;
        }

        /// <summary>
        ///     cập nhật model xe
        /// </summary>
        /// <param name="name">tên model</param>
        /// <param name="model">thông tin model</param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(string name, DeviceModelTranfer model)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền sửa Loại Xe" };

            var allCenter = DataCenterStore.GetAll();
            if (allCenter == null)
                return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý" };

            var api = new ForwardApi();
            BaseResponse ret = new BaseResponse { Description = "Chưa xử lý" };
            foreach (var center in allCenter)
            {
                if (center == null) continue;
                try
                {
                    var myret = api.Put<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceModel/Update?name={name}", model);
                    if (ret.Status == 0) ret = myret;
                }
                catch (System.Exception)
                {
                }
            }

            return ret;
        }

        /// <summary>
        ///     xóa thông tin model
        /// </summary>
        /// <param name="name">tên model</param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(string name)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new BaseResponse { Description = "Không có quyền xóa Loại Xe" };

            var allCenter = DataCenterStore.GetAll();
            if (allCenter == null)
                return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý" };

            var api = new ForwardApi();
            BaseResponse ret = new BaseResponse { Description = "Chưa xử lý" };
            foreach (var center in allCenter)
            {
                if (center == null) continue;
                try
                {
                    var myret = api.Del<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceModel/Del?name={name}");
                    if (ret.Status == 0) ret = myret;
                }
                catch (System.Exception)
                {
                }
            }

            return ret;
        }

        /// <summary>
        ///     lấy thông tin model theo tên
        /// </summary>
        /// <param name="name">tên model</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceModelGetSingle GetByName(string name)
        {
            var allCenter = DataCenterStore.GetAll();
            if (allCenter == null)
                return new DeviceModelGetSingle { Description = "Không tìm thấy thông tin máy chủ xử lý" };

            var api = new ForwardApi();
            foreach (var center in allCenter)
            {
                if (center == null) continue;
                try
                {
                    var ret = api.Get<DeviceModelGetSingle>($"{center.Ip}:{center.Port}/api/DeviceModel/GetByName?name={name}");
                    if (ret.Status > 0) return ret;
                }
                catch (System.Exception)
                {
                }
            }

            return new DeviceModelGetSingle { Description = "Không tìm thấy loại xe này" };
        }

        /// <summary>
        ///     Lấy toàn bộ danh sách model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public DeviceModelGetMulti GetAll()
        {
            var allCenter = DataCenterStore.GetAll();

            var result = new DeviceModelGetMulti();
            result.Status = 1;
            result.Description = "OK";
            result.Models = new List<DeviceModelTranfer>();
            var api = new ForwardApi();
            foreach (var dataCenterInfo in allCenter)
            {
                var tmp =
                    api.Get<DeviceModelGetMulti>($"{dataCenterInfo.Ip}:{dataCenterInfo.Port}/api/DeviceModel/GetAll");
                if (tmp != null && tmp.Status == 1)
                    ((List<DeviceModelTranfer>) result.Models).AddRange(tmp.Models);
            }

            //distinct by Name
            result.Models = result.Models.GroupBy(m => m.Name).Select(g => g.First()).ToList();

            return result;
        }


    }
}