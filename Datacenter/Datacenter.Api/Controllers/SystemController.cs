#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : SystemController.cs
// Time Create : 10:56 AM 13/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System.ComponentModel.Composition;
using System.Web.Mvc;
using Datacenter.Api.Core;
using StarSg.Utils.Models.DatacenterResponse.System;
using System;
using System.Collections.Generic;
using StarSg.Core;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     lấy thông tin log của hệ thống
    /// </summary>
    [Auth, Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class SystemController : BaseController
    {
        [Import]
        private CacheLogManager _cacheLog;

        /// <summary>
        /// Lấy thông tin log của Data Center
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpGet]
        public SysLogResponse GetDatacenterLog(string tag, long index)
        {
            try
            {
                var result = MefConfig.RealLog.GetLog(index);
                return new SysLogResponse
                {
                    Status = 1,
                    Description = "OK",
                    Datas = result.Item2,
                    Index = result.Item1
                };
            }
            catch (Exception ex)
            {
                return new SysLogResponse
                {
                    Status = 1,
                    Description = "OK",
                    Datas =new List<String>() { $"[EXCEPTION] {ex.Message}" , $"[EXCEPTION] {ex.StackTrace}" },
                    Index = index
                };
            }
        }

        /// <summary>
        /// Thực hiệt tất cả các công việc lưu trừ cho cache
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetSaveCache(bool savelog=true)
        {
            try
            {
                //Chờ cho load xong mới xử lý trả về
                DataCenter.Core.DataStore.LoadReadyEvent.WaitOne();

                if (savelog)
                {
                    if (_cacheLog == null) Log.Fatal("SystemController", "_cacheLog==null");
                    else _cacheLog.ProcessImportData();
                }

                Cache.SaveCacheData();

                return new BaseResponse
                {
                    Status = 1,
                    Description = "OK"
                };
            }
            catch (Exception ex)
            {
                return new SysLogResponse
                {
                    Status = 0,
                    Description = $"[EXCEPTION] {ex.Message} TRACE {ex.StackTrace}"
                };
            }
        }

        /// <summary>
        /// Load memory từ cache
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetLoadCache()
        {
            try
            {
                Cache.LoadRawLogSerials();

                return new BaseResponse
                {
                    Status = 1,
                    Description = "OK"
                };
            }
            catch (Exception ex)
            {
                return new SysLogResponse
                {
                    Status = 0,
                    Description = $"[EXCEPTION] {ex.Message} TRACE {ex.StackTrace}"
                };
            }
        }


        /// <summary>
        /// GCCollect
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetGCCollect()
        {
            try
            {
                long before = GC.GetTotalMemory(false);
                GC.Collect();
                long after = GC.GetTotalMemory(true);
                return new BaseResponse
                {
                    Status = 1,
                    Description = $"Before={before} After={after}"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Status = 0,
                    Description = $"[EXCEPTION] {ex.Message} TRACE {ex.StackTrace}"
                };
            }
        }

        /// <summary>
        /// GetSaveDeviceStatus
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetSaveDeviceStatusCache()
        {
            try
            {
                Cache.SaveDeviceStatusCache();
                return new BaseResponse
                {
                    Status = 1,
                    Description = "OK"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Status = 0,
                    Description = $"[EXCEPTION] {ex.Message} TRACE {ex.StackTrace}"
                };
            }
        }


    }
}