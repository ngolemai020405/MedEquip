using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace WebAdmin.Controllers;
public class BaseAdminController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            context.Result = RedirectToAction("Login", "Account");
            return;
        }
        ViewBag.HoTen = HttpContext.Session.GetString("HoTen");
        ViewBag.RoleName = HttpContext.Session.GetString("RoleName");
        base.OnActionExecuting(context);
    }
}