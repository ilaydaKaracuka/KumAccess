using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KumAccess.Services;
using KumAccess.Models; 
using System.Threading.Tasks;
using System.Text.Json;

namespace KumAccess.Pages.Roles
{
    public class IndexModel : PageModel
    {
        private readonly RoleSet _roleSet;
        private readonly RoleGet _roleService;
        private readonly UserAppRoleGet _userAppRoleGet;
        private readonly UserGet _userGet;

        public List<Role> Roles { get; set; }
        public IndexModel(RoleSet roleSet, RoleGet roleService, UserAppRoleGet userAppRoleGet, UserGet userGet)
        {
            _roleSet = roleSet;
            _roleService = roleService;
            _userAppRoleGet = userAppRoleGet;

            _userGet = userGet;

        }

        public async Task<IActionResult> OnPostAddRoleAsync([FromBody] Role request)
        {
            if (!string.IsNullOrWhiteSpace(request.RoleName))
            {
                await _roleSet.AddRoleAsync(request.RoleName);
                return new JsonResult(new { success = true, message = "Role saved successfully!" });
            }

            return new JsonResult(new { success = false, message = "Role name cannot be empty." });
        }
       

      public void OnGet()
        {
            Roles = _roleService.GetAllRoles();
        }
        public class GetRoleRequest
        {
            public int RoleId { get; set; }
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostGetRoleUsersAsync()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var payload = JsonSerializer.Deserialize<GetRoleRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (payload?.RoleId > 0)
                {
                    var allUserAppRoles = _userAppRoleGet.GetAllUserAppRoles()
                        .Where(x => x.RoleId == payload.RoleId)
                        .ToList();

                    var allUsers = _userGet.GetAllUsers();

                    var result = allUserAppRoles.Select(x => new
                    {
                        userId = x.UserId,
                        userName = allUsers.FirstOrDefault(u => u.UserId == x.UserId)?.FirstName + " " + allUsers.FirstOrDefault(u => u.UserId == x.UserId)?.LastName
                    }).ToList();

                    return new JsonResult(result);
                }

                return BadRequest("Geçersiz RoleId.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA: " + ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
