using OnlineMarketApi.Models.RequestsResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OnlineMarketApi.Handlers
{
    public class CustomAuthorize : AuthorizationFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        // Override
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {

                string controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
                if (controllerName != "PublicKey")
                {
                    if (actionContext.Request.Headers.Authorization == null)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Forbidden);
                    }
                    else
                    {
                        var authToken = actionContext.Request.Headers
                          .Authorization.Parameter;
                        var decodeauthToken = System.Text.Encoding.UTF8.GetString(
                            Convert.FromBase64String(authToken));
                        var arrUserNameandPassword = decodeauthToken.Split(':');
                        if (Helper.IsAuthorizedUser(arrUserNameandPassword[0], arrUserNameandPassword[1]))
                        {
                            Thread.CurrentPrincipal = new GenericPrincipal(
                            new GenericIdentity(arrUserNameandPassword[0]), null);
                        }
                        else
                        {
                            actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
