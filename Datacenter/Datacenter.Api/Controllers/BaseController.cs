#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : BaseController.cs
// Time Create : 1:38 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.Description;
using Core.Utils;
using DaoDatabase;
using Datacenter.Api.Core;
using Datacenter.Model.Log.ZipLog;
using Datacenter.QueryRoute;
using DataCenter.Core;
using Log;
using StarSg.Core;
using StarSg.Utils.Utils;
using Datacenter.Model.Log;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     thôn tin controller cở sở chứa các thông tin cần thiết cho 1 controller hoạt động
    /// </summary>
    public class BaseController : ApiController, IControllerInstall
    {
        /// <summary>
        ///     Ghi log
        /// </summary>
        [Import]
        protected ILog Log { get; set; }

        /// <summary>
        ///     dữ liệu lưu trên cache
        /// </summary>
        [Import]
        protected IDataStore Cache { get; set; }

        /// <summary>
        ///     truy vấn dữ liệu
        /// </summary>
        protected IQueryRoute DataContext { get; private set; }

        /// <summary>
        ///     id của máy chủ trạm chứa toàn bộ dữ liệu
        /// </summary>
        protected int MotherSqlId { get; set; }

        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual bool ValidAccess(IDependencyResolver dependency, HttpRequestHeaders header)
        {
            //todo: kiểm tra tính hợp lệ của các request ở đây ( chỉ cho phép các route truy cập)
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependency"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool InstallOption(IDependencyResolver dependency, HttpRequestHeaders header)
        {
            // cài dặt các thông số mặc định cần thiết cho 1 controller chạy
            try
            {
                //Log = (ILog)dependency.GetService(typeof(ILog));
                var factory = (ReponsitoryFactory) dependency.GetService(typeof (ReponsitoryFactory));
                DataContext = factory.CreateQuery();
                Cache = (IDataStore) dependency.GetService(typeof (IDataStore));
                // todo : thêm thông tin cấu hình sql ở đây
                var loader = (Loader) dependency.GetService(typeof (Loader));
                //Config = loader.Config;
                MotherSqlId = loader.Config.MotherSql.Id;
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected override void Dispose(bool disposing)
        {
            try
            {
                DataContext?.Dispose();
            }
            catch (Exception ex)
            {
                Log?.Exception("Controller", ex, "Disponse controller lỗi");
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TZip"></typeparam>
        /// <param name="dbId"></param>
        /// <param name="where"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IList<T> GetZipLog<T, TZip>(int dbId,Expression<Func<TZip, bool>> where, DateTime begin, DateTime end) where TZip:class,IZip where T:class,IEntity
        {
            if(begin.Date >= DateTime.Now.Date) return new List<T>(0);

            dbId +=1000;
            //get all day
            DateTime begintime = new DateTime(begin.Year, begin.Month, begin.Day, 0, 0, 0);
            DateTime endtime = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59,999);

            var tmp =
                 DataContext.CreateQuery<TZip>(dbId);
            if (where != null)
                tmp = tmp.Where(where);
            var kq = tmp.Where(m => m.TimeUpdate >= begintime && m.TimeUpdate <= endtime)
                .Execute();

            if (typeof(T) == typeof(DeviceLog))
            {
                var result = new List<DeviceLog>();
                foreach (IDeviceZip zip in kq)
                {
                    //result.AddRange(zip.Data.UnZip().ByteArrayToObject<IList<T>>());
                    DeviceLogCollection devlog = new DeviceLogCollection();
                    devlog.Deserializer(zip.Data.UnZip());

                    //gán lại Serial nếu như khác nhau (thiết bị đổi serial mà trong zip chưa đổi)
                    foreach (var obj in devlog.listout)
                        if (obj.Serial != zip.Serial) obj.Serial = zip.Serial;

                    result.AddRange(devlog.listout);
                }

                //filter by real time
                result = result.Where(m => m.DeviceStatus.ClientSend >= begin && m.DeviceStatus.ClientSend <= end).ToList();

                return (IList<T>)result;
            }
            else if (typeof(T) == typeof(DeviceTraceLog))
            {
                var result = new List<DeviceTraceLog>();
                foreach (IDeviceZip zip in kq)
                {
                    DeviceTraceLogCollection devlog = new DeviceTraceLogCollection();
                    devlog.Deserializer(zip.Data.UnZip());

                    //gán lại Serial nếu như khác nhau (thiết bị đổi serial mà trong zip chưa đổi)
                    foreach (var obj in devlog.listout)
                        if(obj.Serial != zip.Serial) obj.Serial = zip.Serial;

                    result.AddRange(devlog.listout);
                }

                //filter by real time
                result = result.Where(m => m.BeginTime >= begin && m.BeginTime <= end).ToList();

                return (IList<T>)result;
            }
            else
            {
                var result = new List<T>();
                foreach (var zip in kq)
                {
                    result.AddRange(zip.Data.UnZip().ByteArrayToObject<IList<T>>());
                }
                return result;
            }
            
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TZip"></typeparam>
        /// <param name="dbId">định tuyến của database</param>
        /// <param name="whereor"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IList<T> GetZipLogWhereOr<T, TZip>(int dbId,Expression<Func<TZip, bool>>[] whereor, DateTime begin,
            DateTime end) where TZip : class, IZip where T : class, IEntity
        {
            if (begin.Date >= DateTime.Now.Date) return new List<T>(0);

            dbId += 1000;

            //get all day
            DateTime begintime = new DateTime(begin.Year, begin.Month, begin.Day, 0, 0, 0);
            DateTime endtime = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);

            var tmp =
                 DataContext.CreateQuery<TZip>(dbId);
            if (whereor != null && whereor.Length > 0)
                tmp = tmp.WhereOr(whereor);
            var kq = tmp.Where(m => m.TimeUpdate >= begintime && m.TimeUpdate <= endtime)
                .Execute();

            if (typeof(T) == typeof(DeviceLog))
            {
                var result = new List<DeviceLog>();
                foreach (IDeviceZip zip in kq)
                {
                    //result.AddRange(zip.Data.UnZip().ByteArrayToObject<IList<T>>());
                    DeviceLogCollection devlog = new DeviceLogCollection();
                    devlog.Deserializer(zip.Data.UnZip());

                    //gán lại Serial nếu như khác nhau (thiết bị đổi serial mà trong zip chưa đổi)
                    foreach (var obj in devlog.listout)
                        if (obj.Serial != zip.Serial) obj.Serial = zip.Serial;

                    result.AddRange(devlog.listout);
                }

                //filter by real time
                result = result.Where(m => m.DeviceStatus.ClientSend >= begin && m.DeviceStatus.ClientSend <= end).ToList();

                return (IList<T>)result;
            }
            else if (typeof(T) == typeof(DeviceTraceLog))
            {
                var result = new List<DeviceTraceLog>();
                foreach (IDeviceZip zip in kq)
                {
                    DeviceTraceLogCollection devlog = new DeviceTraceLogCollection();
                    devlog.Deserializer(zip.Data.UnZip());

                    //gán lại Serial nếu như khác nhau (thiết bị đổi serial mà trong zip chưa đổi)
                    foreach (var obj in devlog.listout)
                        if (obj.Serial != zip.Serial) obj.Serial = zip.Serial;

                    result.AddRange(devlog.listout);
                }

                //filter by real time
                result = result.Where(m => m.BeginTime >= begin && m.BeginTime <= end).ToList();

                return (IList<T>)result;
            }
            else
            {
                var result = new List<T>();
                foreach (var zip in kq)
                {
                    result.AddRange(zip.Data.UnZip().ByteArrayToObject<IList<T>>());
                }
                return result;
            }

        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected Expression<Func<T, bool>> BuildWhereOr<T>(params Expression<Func<T, bool>>[] expression)
        {
            if (expression.Length < 1) throw new Exception("BaseController : Danh sach dieu kien rong");
            if (expression.Length <= 1) return expression[0];
            BinaryExpression tmp = null;
            for (var i = 1; i < expression.Length; i++)
            {
                tmp = tmp == null
                    ? Expression.OrElse(expression[i - 1].Body, expression[i].Body)
                    : Expression.OrElse(tmp, expression[i].Body);
            }
            if (tmp == null) throw new Exception("BaseController WhereOr : Lỗi");
            ;
            var condition = Expression.Lambda<Func<T, bool>>(tmp,
                expression.First().Parameters);
            return condition;
        }


    }
}