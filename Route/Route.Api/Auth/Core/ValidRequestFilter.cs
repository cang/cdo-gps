#region header
// /*********************************************************************************************/
// Project :Authentication
// FileName : ValidRequestFilter.cs
// Time Create : 9:09 AM 22/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Log;


namespace Route.Api.Auth.Core
{
    /// <summary>
    /// check trước các request trước khi đưa vào cho hệ thống xử lý
    /// </summary>
    public class ValidRequestFilter:Attribute, IAuthenticationFilter
    {
        #region Implementation of IFilter

        /// <summary>
        /// Gets or sets a value indicating whether more than one instance of the indicated attribute can be specified for a single program element.
        /// </summary>
        /// <returns>
        /// true if more than one instance is allowed to be specified; otherwise, false. The default is false.
        /// </returns>
        public bool AllowMultiple { get; }

        #endregion

        #region Implementation of IAuthenticationFilter

        /// <summary>
        /// Authenticates the request.
        /// </summary>
        /// <returns>
        /// A Task that will perform authentication.
        /// </returns>
        /// <param name="context">The authentication context.</param><param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var log =
                (ILog)
                    GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ILog));
            //todo: ghi log quản lý lưu lượng truy cập ở đây
            try
            {
                var tmp = HttpUtility.ParseQueryString(context.ActionContext.Request.RequestUri.Query);
                var apiKey = tmp.Get("api_key");
                if (!string.IsNullOrEmpty(apiKey) && !context.ActionContext.Request.Headers.Contains("token"))
                {
                    context.ActionContext.Request.Headers.Add("token", apiKey);

                }
                // ReSharper disable once SuspiciousTypeConversion.Global
                var controller = context.ActionContext.ControllerContext.Controller as IControllerInstall;
                if (controller == null)
                {
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                    //context.ActionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    //{
                    //    Content =
                    //        new ObjectContent<bool>(false, new JsonMediaTypeFormatter())
                    //};
                    return Task.FromResult(0);
                }
                if (!controller.Install(GlobalConfiguration.Configuration.DependencyResolver,
                    context.ActionContext.Request.Headers))
                {
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                    return Task.FromResult(0);
                }
                
            }
            catch (Exception ex)
            {
                log.Exception("API Authenticate", ex, "Xử lý thông tin truy cập api lỗi");
            }

            return Task.FromResult(0);
        }

        #endregion

        #region Implementation of IAuthenticationFilter

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result);
            return Task.FromResult(0);
        }

        #endregion
    }
    /// <summary>
    /// </summary>
    public class ResultWithChallenge : IHttpActionResult
    {
        /// <summary>
        ///     The authentication scheme.
        /// </summary>
        private readonly string authenticationScheme = "sgst";

        /// <summary>
        ///     The next.
        /// </summary>
        private readonly IHttpActionResult next;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResultWithChallenge" /> class.
        /// </summary>
        /// <param name="next">
        ///     The next.
        /// </param>
        public ResultWithChallenge(IHttpActionResult next)
        {
            this.next = next;
        }

        /// <summary>
        ///     The execute async.
        /// </summary>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await next.ExecuteAsync(cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(authenticationScheme));
            }

            return response;
        }
    }
}