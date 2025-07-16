using Microsoft.AspNetCore.Mvc.RazorPages;
using KumAccess.Models;
using KumAccess.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KumAccess.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly UserGet _userService;
        private readonly ApplicationGet _applicationService;
        private readonly RoleGet _roleService;
        private readonly GroupGet _groupService;
        private readonly UserAppRoleSet _userAppRoleSet;
        private readonly UserGroupSet _userGroupSet;
        private readonly UserAppRoleGet _userAppRoleGet;
        private readonly UserGroupGet _userGroupGet;
        public List<User> Users { get; set; }
        public List<App> Applications { get; set; }
        public List<Role> Roles { get; set; }
        public List<Group> Groups { get; set; }


        public IndexModel(UserGet userService, ApplicationGet applicationService, RoleGet roleService, GroupGet groupService, UserAppRoleSet userAppRoleSet, UserGroupSet userGroupSet ,UserAppRoleGet userAppRoleGet, UserGroupGet userGroupGet)
        {
            _userService = userService;
            _applicationService = applicationService;
            _roleService = roleService;
            _groupService = groupService;
            _userAppRoleSet = userAppRoleSet;
            _userGroupSet = userGroupSet;
            _userAppRoleGet = userAppRoleGet;
            _userGroupGet = userGroupGet;


        }

        public void OnGet()
        {
            Users = _userService.GetAllUsers();
            Applications = _applicationService.GetAllApplications();
            Roles = _roleService.GetAllRoles();
            Groups = _groupService.GetAllGroups();


        }
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostSaveUserAppRoleAsync()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var payload = JsonSerializer.Deserialize<SaveUserAppRoleRequest>(body, options);

                if (payload == null || payload.UserId <= 0 || payload.ApplicationId <= 0 || payload.RoleId <= 0)
                {
                    return BadRequest(new { success = false, message = "Eksik veya hatalý veri." });
                }

                var existing = _userAppRoleGet.GetAllUserAppRoles()
                    .Any(x => x.UserId == payload.UserId &&
                              x.ApplicationId == payload.ApplicationId &&
                              x.RoleId == payload.RoleId);

                if (existing)
                {
                    Console.WriteLine("Bu kullanýcýya bu uygulama ve rol zaten atanmýþ.");
                    return new JsonResult(new { success = false, message = "Bu kullanýcýya bu uygulama ve rol zaten atanmýþ." });
                }

                await _userAppRoleSet.AddUserAppRoleAsync(payload.UserId, payload.ApplicationId, payload.RoleId);

                return new JsonResult(new { success = true, message = "Kayýt baþarýyla eklendi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA: " + ex.Message);
                return StatusCode(500, new { success = false, message = "Sunucu hatasý: " + ex.Message });
            }
        }



        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostSaveUserGroupAsync()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var payload = JsonSerializer.Deserialize<SaveUserGroupRequest>(body, options);

                if (payload == null || payload.UserId <= 0 || payload.GroupId <= 0)
                {
                    return BadRequest("Eksik ya da hatalý veri.");
                }

                var exists = _userGroupGet.GetAllUserGroups()
                    .Any(x => x.UserId == payload.UserId && x.GroupId == payload.GroupId);

                if (exists)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bu kullanýcý zaten bu gruba atanmýþ."
                    });
                }

                await _userGroupSet.AddUserGroupAsync(payload.UserId, payload.GroupId);

                return new JsonResult(new
                {
                    success = true,
                    message = "Kullanýcý gruba baþarýyla eklendi."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA: " + ex.Message);
                return StatusCode(500, "Sunucu hatasý: " + ex.Message);
            }
        }


        public class SaveUserAppRoleRequest
        {
            public int UserId { get; set; }
            public int ApplicationId { get; set; }
            public int RoleId { get; set; }
        }

        public class SaveUserGroupRequest
        {
            public int UserId { get; set; }
            public int GroupId { get; set; }
        }
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostGetUserPermissionsAsync()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var payload = JsonSerializer.Deserialize<GetUserRequest>(body, options);

                if (payload?.UserId > 0)
                {
                    var allUserAppRoles = _userAppRoleGet.GetAllUserAppRoles()
                        .Where(x => x.UserId == payload.UserId)
                        .ToList();

                    var applications = _applicationService.GetAllApplications();
                    var roles = _roleService.GetAllRoles();

                    var result = allUserAppRoles.Select(x => new
                    {
                        applicationId = x.ApplicationId,
                        applicationName = applications.FirstOrDefault(a => a.ApplicationId == x.ApplicationId)?.ApplicationName ?? "Unknown",
                        roleName = roles.FirstOrDefault(r => r.RoleId == x.RoleId)?.RoleName ?? "Unknown"
                    }).ToList();

                    return new JsonResult(result);
                }

                return BadRequest("UserId geçersiz.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA: " + ex.Message);
                return StatusCode(500, new { error = ex.Message }); 
            }
        }


        public class GetUserRequest
        {
            public int UserId { get; set; }
        }


    }


}

