using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KumAccess.Services;
using KumAccess.Models; 
using System.Threading.Tasks;
using System.Text.Json;

namespace KumAccess.Pages.Applications
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationService _applicationService;
        private readonly ApplicationGet _appService;
        private readonly UserAppRoleGet _userAppRoleGet;
        private readonly UserGet _userGet;
        private readonly RoleGet _roleGet;
        private readonly GroupAppRoleGet _groupAppRoleGet;
        private readonly GroupGet _groupGet;






        public IndexModel(ApplicationService applicationService, ApplicationGet appService, UserAppRoleGet userAppRoleGet, UserGet userGet, RoleGet roleGet, GroupGet groupGet, GroupAppRoleGet groupAppRoleGet)
        {
            _applicationService = applicationService;
            _appService = appService;
            _userAppRoleGet = userAppRoleGet;
            _userGet = userGet;
         

            _roleGet = roleGet;
            _groupGet = groupGet;
            _groupAppRoleGet = groupAppRoleGet;


        }

        public async Task<IActionResult> OnPostAddApplicationAsync([FromBody] App request)
        {
            if (!string.IsNullOrWhiteSpace(request.ApplicationName))
            {
                await _applicationService.AddApplicationAsync(request.ApplicationName);
                return new JsonResult(new { success = true, message = "Application saved successfully!" });
            }

            return new JsonResult(new { success = false, message = "Application name cannot be empty." });
        }

        public List<App> Applications { get; set; }

        public void OnGet()
        {
            Applications = _appService.GetAllApplications();
        }
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostGetAppUsersAsync()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var payload = JsonSerializer.Deserialize<GetAppRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (payload?.ApplicationId > 0)
                {
                    var userAppRoles = _userAppRoleGet.GetAllUserAppRoles()
                        .Where(x => x.ApplicationId == payload.ApplicationId)
                        .ToList();

                    var users = _userGet.GetAllUsers();
                    var roles = _roleGet.GetAllRoles();

                   
                    var result = userAppRoles.Select(x => new
                    {
                        userId = x.UserId,
                        firstName = users.FirstOrDefault(u => u.UserId == x.UserId)?.FirstName ?? "N/A",  // Varsayýlan deðer ekledim
                        lastName = users.FirstOrDefault(u => u.UserId == x.UserId)?.LastName ?? "N/A",      // Varsayýlan deðer ekledim
                        roleName = roles.FirstOrDefault(r => r.RoleId == x.RoleId)?.RoleName ?? "Unknown"  // Varsayýlan deðer ekledim
                    }).ToList();

                    return new JsonResult(result);
                }

                return BadRequest("Geçersiz ApplicationId.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA: " + ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public class GetAppRequest
        {
            public int ApplicationId { get; set; }
        }
        [IgnoreAntiforgeryToken]

        public async Task<IActionResult> OnPostGetAppGroupsAsync()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var payload = JsonSerializer.Deserialize<GetAppRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (payload?.ApplicationId > 0)
                {
                    // GroupAppRole verilerini al
                    var groupAppRoles = _groupAppRoleGet.GetAllGroupAppRoles()
                        .Where(x => x.ApplicationId == payload.ApplicationId)
                        .ToList();

                    var groups = _groupGet.GetAllGroups(); // Grup bilgilerini al
                    var roles = _roleGet.GetAllRoles(); // Rol bilgilerini al

                    var result = groupAppRoles.Select(x => new
                    {
                        groupId = x.GroupId,
                        groupName = groups.FirstOrDefault(g => g.GroupId == x.GroupId)?.GroupName ?? "N/A",
                        roleName = roles.FirstOrDefault(r => r.RoleId == x.RoleId)?.RoleName ?? "Unknown"
                    }).ToList();

                    return new JsonResult(result);
                }

                return BadRequest("Geçersiz ApplicationId.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA: " + ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }
}
