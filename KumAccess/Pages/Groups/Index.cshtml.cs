using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KumAccess.Services;
using KumAccess.Models;
using System.Threading.Tasks;
using static KumAccess.Pages.Users.IndexModel;
using System.Text.Json;

namespace KumAccess.Pages.Groups
{
    public class IndexModel : PageModel
    {
        private readonly GroupSet _groupSet;
        private readonly GroupGet _groupService;
        private readonly ApplicationGet _applicationService;
        private readonly RoleGet _roleService;
        private readonly GroupAppRoleSet _groupAppRoleSet;
        private readonly UserGroupGet _userGroupGet;
        private readonly UserGet _userGet;
        private readonly GroupAppRoleGet _groupAppRoleGet;

        public List<App> Applications { get; set; }
        public List<Role> Roles { get; set; }
        public List<Group> Groups { get; set; }

        public IndexModel(GroupSet groupSet, GroupGet groupService, ApplicationGet applicationService, RoleGet roleService, GroupAppRoleGet groupAppRoleGet, GroupAppRoleSet groupAppRoleSet, UserGroupGet userGroupGet, UserGet userGet)
        {
            _groupSet = groupSet;
            _applicationService = applicationService;
            _roleService = roleService;
            _groupService = groupService;
            _groupAppRoleSet = groupAppRoleSet;
            _userGroupGet = userGroupGet;
            _userGet = userGet;

            _groupAppRoleGet = groupAppRoleGet;


        }

        public async Task<IActionResult> OnPostAddGroupAsync([FromBody] Group request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.GroupName))
                return new JsonResult(new { success = false, message = "Group name cannot be empty." });

            try
            {
                await _groupSet.AddGroupAsync(request.GroupName);
                return new JsonResult(new { success = true, message = "Group saved successfully!" });
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = "Error: " + ex.Message });
            }
        }

        public void OnGet()
        {
            Groups = _groupService.GetAllGroups();
            Applications = _applicationService.GetAllApplications();
            Roles = _roleService.GetAllRoles();
        }
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostSaveGroupAppRoleAsync()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var payload = JsonSerializer.Deserialize<SaveGroupAppRoleRequest>(body, options);

                if (payload == null || payload.GroupId <= 0 || payload.ApplicationId <= 0 || payload.RoleId <= 0)
                {
                    return BadRequest(new { success = false, message = "Eksik veya hatalý veri." });
                }

                // Zaten atanmýþ mý kontrol et
                var existing = _groupAppRoleGet.GetAllGroupAppRoles()
                    .Any(x => x.GroupId == payload.GroupId &&
                              x.ApplicationId == payload.ApplicationId &&
                              x.RoleId == payload.RoleId);

                if (existing)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bu uygulama ve rol zaten bu gruba atanmýþ."
                    });
                }

                await _groupAppRoleSet.AddGroupAppRoleAsync(payload.GroupId, payload.ApplicationId, payload.RoleId);

                return new JsonResult(new { success = true, message = "Atama baþarýyla kaydedildi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA: " + ex.Message);
                return StatusCode(500, new { success = false, message = "Sunucu hatasý: " + ex.Message });
            }
        }

        public class SaveGroupAppRoleRequest
        {
            public int GroupId { get; set; }
            public int ApplicationId { get; set; }
            public int RoleId { get; set; }
        }


        public class GetGroupRequest
        {
            public int GroupId { get; set; }
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostGetGroupUsersAsync()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var payload = JsonSerializer.Deserialize<GetGroupRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (payload?.GroupId > 0)
                {
                    var allUserGroups = _userGroupGet.GetAllUserGroups()
                        .Where(x => x.GroupId == payload.GroupId)
                        .ToList();

                    var allUsers = _userGet.GetAllUsers();

                    var result = allUserGroups.Select(x => new
                    {
                        userId = x.UserId,
                        userName = allUsers.FirstOrDefault(u => u.UserId == x.UserId)?.FirstName + " " + allUsers.FirstOrDefault(u => u.UserId == x.UserId)?.LastName
                    }).ToList();

                    return new JsonResult(result);
                }

                return BadRequest("Geçersiz GroupId.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA: " + ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
