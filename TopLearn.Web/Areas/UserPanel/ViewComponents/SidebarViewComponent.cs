using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public SidebarViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("SidebarViewComponent",
                await _userService.GetSidebarDataByEmailAsync(User.Identity?.Name)));
        }
    }
}
