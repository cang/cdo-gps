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
    /// <summary>
    ///     xử lý các thông tin chi nhánh
    /// </summary>
    [ValidRequestFilter]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BranchController : ValidController
    {
        /// <summary>
        ///     lấy chi nhánh theo id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(BaseResponse))]
        [Route("Branch")]
        public IHttpActionResult GetBranch(string Id)
        {
            var ret = AccountManager.GetBranch(Id);
            if(ret!=null)
                return Ok(new BranchResponse { Status = 1, Description = "OK", Data  = new BranchTranfer() {
                    Id = ret.Id,
                    Logo = ret.Logo,
                    Name= ret.Name,
                    ReportPhoneNumber= ret.ReportPhoneNumber,
                    SupportPhoneNumber = ret.SupportPhoneNumber,
                    WebSite = ret.WebSite,
                    Reserve= ret.Reserve,

                    Address = ret.Address,
                    Footer = ret.Footer,
                    Header = ret.Header,
                    LongName = ret.LongName
                } });


            Log.Warning("BranchController", $"Không tồn tai branch {Id}");
            return Ok(new BaseResponse { Status = 0 });
        }

        private Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        private void UpdateLogo(BranchTranfer branch)
        {
            try
            {
                string base64String = branch.Logo;

                //data:image/png;
                int idx = base64String.IndexOf(";");
                if (idx < 0) return;

                string sformat = base64String.Substring(0, idx);
                string fileex = sformat.Split('/')[1];

                //data:image/png;base64,
                idx = base64String.IndexOf("base64,");
                if (idx < 0) return;
                base64String = base64String.Substring(idx + 7);

                using (Image img = Base64ToImage(base64String))
                {
                    string path = HostingEnvironment.MapPath("~/public/images");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);


                    string filename = System.Text.RegularExpressions.Regex.Replace(branch.Id, @"[^a-zA-Z0-9]+", String.Empty);
                    path = Path.Combine(path, filename + "." + fileex);

                    if (File.Exists(path))
                    {
                        File.SetAttributes(path, FileAttributes.Normal);
                        File.Delete(path);
                    }

                    string virpath = AuthConfig.RouteDomainUrl + "/public/images/" + filename + "." + fileex;
                    img.Save(path);

                    branch.Reserve = virpath;
                    branch.Logo = virpath;
                }
            }
            catch (Exception e)
            {
                Log.Exception("BranchController",e,"UpdateLogo");
            }
        }

        /// <summary>
        ///     tạo chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(BaseResponse))]
        [Route("Branch")]
        public IHttpActionResult AddBranch(BranchTranfer branch)
        {
            if (Account.Level <= AccountLevel.Administrator)
            {
                if (branch == null)
                {
                    Log.Warning("BranchController", "branch truyền vào null");
                    return Ok(new BaseResponse { Status = 0 });
                }

                UpdateLogo(branch);

                var obj = new BranchData {
                     Id = branch.Id,
                     Logo = branch.Logo,
                     Name = branch.Name,
                     ReportPhoneNumber = branch.ReportPhoneNumber,
                     SupportPhoneNumber = branch.SupportPhoneNumber,
                     WebSite = branch.WebSite,
                     Reserve = branch.Reserve,

                    Address = branch.Address,
                    Footer = branch.Footer,
                    Header = branch.Header,
                    LongName = branch.LongName
                };

                if (AccountManager.AddBranch(obj))
                {
                    Log.Warning("BranchController", $"thêm branch {branch.Id} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }

            Log.Warning("BranchController", $"thêm branch {branch.Id} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }


        /// <summary>
        ///     cập nhật chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(BaseResponse))]
        [Route("Branch")]
        public IHttpActionResult UpdateBranch(BranchTranfer branch)
        {
            if (Account.Level <= AccountLevel.Administrator)
            {
                if (branch == null)
                {
                    Log.Warning("BranchController", "branch truyền vào null");
                    return Ok(new BaseResponse { Status = 0 });
                }

                UpdateLogo(branch);

                var obj = new BranchData
                {
                    Id = branch.Id,
                    Logo = branch.Logo,
                    Name = branch.Name,
                    ReportPhoneNumber = branch.ReportPhoneNumber,
                    SupportPhoneNumber = branch.SupportPhoneNumber,
                    WebSite = branch.WebSite,
                    Reserve = branch.Reserve,

                    Address = branch.Address,
                    Footer = branch.Footer,
                    Header = branch.Header,
                    LongName = branch.LongName
                };

                if (AccountManager.UpdateBranch(obj))
                {
                    Log.Warning("BranchController", $"cập nhật branch {branch.Id} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }

            Log.Warning("BranchController", $"cập nhật branch {branch.Id} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        /// <summary>
        ///     xóa chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ResponseType(typeof(BaseResponse))]
        [Route("Branch")]
        public IHttpActionResult DeleteBranch(string Id)
        {
            if (Account.Level <= AccountLevel.Administrator)
            {
                if (AccountManager.DeleteBranch(Id))
                {
                    Log.Warning("BranchController", $"xóa branch {Id} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }

            Log.Warning("BranchController", $"xóa branch {Id} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        /// <summary>
        ///     lấy tất cả chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(BaseResponse))]
        [Route("Branch/All")]
        public IHttpActionResult GetAllBranch()
        {
            var ret = AccountManager.AllBranch();
            var rets = new BranchResponses { Status = 1, Description = "OK" };
            foreach (var item in ret)
            {
                rets.Data.Add( new BranchTranfer()
                {
                    Id = item.Id,
                    Logo = item.Logo,
                    Name = item.Name,
                    ReportPhoneNumber = item.ReportPhoneNumber,
                    SupportPhoneNumber = item.SupportPhoneNumber,
                    WebSite = item.WebSite,
                    Reserve = item.Reserve,

                    Address = item.Address,
                    Footer = item.Footer,
                    Header = item.Header,
                    LongName = item.LongName
                }
                );
            }
            return Ok(rets);
        }


    }


}