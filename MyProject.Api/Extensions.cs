using MyProject.Api.Authentication;
using MyProject.Core;
using MyProject.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Api
{
    public static class Extensions
    {
        public static IActionResult ToActionResult<TResult, TController>(
            this TResult result, TController controller)
            where TResult : IResultBase
            where TController : ControllerBase
        {
            if (result.Success)
                return controller.Ok(result);
            else
                return controller.Conflict(result);
        }

        public static void GetUserIdentity<TController>(
            this ICoreRequestBase request, TController controller)
            where TController : ControllerBase
        {
            request.Identity =
                (UserIdentity)controller.HttpContext.Items[JwtAuthenticationHandler.IdentityKey];
        }
    }
}
