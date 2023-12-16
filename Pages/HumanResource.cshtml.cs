using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASPCoreWebSecurity.Pages;

[Authorize(Policy = "MustBelongToHRDepartment")]
public class HumanResource : PageModel
{
    public void OnGet()
    {
        
    }
}