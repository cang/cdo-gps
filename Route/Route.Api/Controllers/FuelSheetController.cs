using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Http;
using Core.Models.Tranfer;
using Route.Api.Core;
using StarSg.Core;
using System.Linq;
using StarSg.Utils.Models.DatacenterResponse.DeviceModel;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin các loại xe
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FuelSheetController : BaseController
    {
        /// <summary>
        ///     thêm thông tin Bình chứa nhiên liệu
        /// </summary>
        /// <param name="model">thông tin Bình chứa nhiên liệu</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse Add(FuelSheetTranfer model)
        {
            if (UserPermision.GetLevel()>= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền thêm Bình chứa nhiên liệu" };

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
                    var myret  = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/FuelSheet/Add", model);
                    if (ret.Status == 0) ret = myret;
                }
                catch (System.Exception)
                {
                }
            }

            return ret;
        }

        /// <summary>
        ///     cập nhật Bình chứa nhiên liệu
        /// </summary>
        /// <param name="name">tên Bình chứa nhiên liệu</param>
        /// <param name="model">thông tin Bình chứa nhiên liệu</param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(string name, FuelSheetTranfer model)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền sửa Bình chứa nhiên liệu" };

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
                    var myret = api.Put<BaseResponse>($"{center.Ip}:{center.Port}/api/FuelSheet/Update?name={name}", model);
                    if (ret.Status == 0) ret = myret;
                }
                catch (System.Exception)
                {
                }
            }

            return ret;
        }

        /// <summary>
        ///     xóa thông tin Bình chứa nhiên liệu
        /// </summary>
        /// <param name="name">tên Bình chứa nhiên liệu</param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(string name)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new BaseResponse { Description = "Không có quyền xóa Bình chứa nhiên liệu" };

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
                    var myret = api.Del<BaseResponse>($"{center.Ip}:{center.Port}/api/FuelSheet/Del?name={name}");
                    if (ret.Status == 0) ret = myret;
                }
                catch (System.Exception)
                {
                }
            }

            return ret;
        }

        /// <summary>
        ///     lấy thông tin Bình chứa nhiên liệu theo tên
        /// </summary>
        /// <param name="name">tên Bình chứa nhiên liệu</param>
        /// <returns></returns>
        [HttpGet]
        public FuelSheetGetSingle GetByName(string name)
        {
            var allCenter = DataCenterStore.GetAll();
            if (allCenter == null)
                return new FuelSheetGetSingle { Description = "Không tìm thấy thông tin máy chủ xử lý" };

            var api = new ForwardApi();
            foreach (var center in allCenter)
            {
                if (center == null) continue;
                try
                {
                    var ret = api.Get<FuelSheetGetSingle>($"{center.Ip}:{center.Port}/api/FuelSheet/GetByName?name={name}");
                    if (ret.Status > 0) return ret;
                }
                catch (System.Exception)
                {
                }
            }

            return new FuelSheetGetSingle { Description = "Không tìm thấy Bình chứa nhiên liệu này" };
        }

        /// <summary>
        ///     Lấy toàn bộ danh sách Bình chứa nhiên liệu
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public FuelSheetGetMulti GetAll()
        {
            var allCenter = DataCenterStore.GetAll();

            var result = new FuelSheetGetMulti();
            result.Status = 1;
            result.Description = "OK";
            result.FuelSheets = new List<FuelSheetTranfer>();
            var api = new ForwardApi();
            foreach (var dataCenterInfo in allCenter)
            {
                var tmp =
                    api.Get<FuelSheetGetMulti>($"{dataCenterInfo.Ip}:{dataCenterInfo.Port}/api/FuelSheet/GetAll");
                if (tmp != null && tmp.Status == 1)
                    ((List<FuelSheetTranfer>) result.FuelSheets).AddRange(tmp.FuelSheets);
            }

            //distinct by Name
            result.FuelSheets = result.FuelSheets.GroupBy(m => m.Name).Select(g => g.First()).ToList();

            return result;
        }


    }
}