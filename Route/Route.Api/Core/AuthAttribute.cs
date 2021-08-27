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
using StarSg.Core;

namespace Route.Api.Core
{
    /// <summary>
    /// </summary>
    public class AuthAttribute : Attribute, IAuthenticationFilter
    {
        #region Implementation of IFilter

        /// <summary>
        ///     Gets or sets a value indicating whether more than one instance of the indicated attribute can be specified for a
        ///     single program element.
        /// </summary>
        /// <returns>
        ///     true if more than one instance is allowed to be specified; otherwise, false. The default is false.
        /// </returns>
        public bool AllowMultiple => false;

        #endregion

        #region Implementation of IAuthenticationFilter

        /// <summary>
        ///     Authenticates the request.
        /// </summary>
        /// <returns>
        ///     A Task that will perform authentication.
        /// </returns>
        /// <param name="context">The authentication context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var log =
                (ILog)
                    GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof (ILog));
            //todo: ghi log quản lý lưu lượng truy cập ở đây
            try
            {
                var tmp = HttpUtility.ParseQueryString(context.ActionContext.Request.RequestUri.Query);
                var apiKey = tmp.Get("api_key");
                if (!string.IsNullOrEmpty(apiKey) && !context.ActionContext.Request.Headers.Contains("token"))
                {
                    context.ActionContext.Request.Headers.Add("token", apiKey);

                }
                var controller = context.ActionContext.ControllerContext.Controller as IControllerInstall;
                if (controller == null)
                {
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                    return Task.FromResult(0);
                }
                if (!controller.ValidAccess(GlobalConfiguration.Configuration.DependencyResolver,
                    context.ActionContext.Request.Headers))
                {
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                    return Task.FromResult(0);
                }

                if (!controller.InstallOption(GlobalConfiguration.Configuration.DependencyResolver,
                    context.ActionContext.Request.Headers))
                {
                    context.ErrorResult = new InternalServerErrorResult(context.Request);
                    return Task.FromResult(0);
                }
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                log.Exception("API Authenticate", ex, "Xử lý thông tin truy cập api lỗi");
            }

            context.ErrorResult = new InternalServerErrorResult(context.Request);
            return Task.FromResult(0);
        }

        #endregion

        #region Implementation of IAuthenticationFilter

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result);
            return Task.FromResult(0);
        }

        #endregion

        private static Task Error(HttpAuthenticationContext context, string content)
        {
            context.ErrorResult = new BadRequestErrorMessageResult(content,
                (ApiController) context.ActionContext.ControllerContext.Controller);
            //{
            //    Content = new StringContent(content)
            //};
            return Task.FromResult(0);
        }
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