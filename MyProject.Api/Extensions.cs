using MyProject.Api.Authentication;
using MyProject.Core;
using MyProject.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Api
{
    public static class Extensions
    {
        public static IActionResult ToActionResult<TReason, TResultData, TController>(
            this Result<TReason, TResultData> result, TController controller)
            where TReason : struct
            where TResultData : class
            where TController : ControllerBase
        {
            if (result.Success)
                return controller.Ok(result);
            else
                return controller.Conflict(result);
        }

        public static IActionResult ToActionResult<TReason, TController>(
            this Result<TReason> result, TController controller)
            where TReason : struct
            where TController : ControllerBase
        {
            if (result.Success)
                return controller.Ok(result);
            else
                return controller.Conflict(result);
        }

        public static IActionResult ToActionResult<TResultData, TController>(
            this DataResult<TResultData> result, TController controller)
            where TResultData : class
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
