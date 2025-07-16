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

        public List<User> Users { get; set; }
        public List<App> Applications { get; set; }
        public List<Role> Roles { get; set; }
        public List<Group> Groups { get; set; }


        public IndexModel(UserGet userService, ApplicationGet applicationService, RoleGet roleService, GroupGet groupService, UserAppRoleSet userAppRoleSet, UserGroupSet userGroupSet ,UserAppRoleGet userAppRoleGet)
        {
            _userService = userService;
            _applicationService = applicationService;
            _roleService = roleService;
            _groupService = groupService;
            _userAppRoleSet = userAppRoleSet;
            _userGroupSet = userGroupSet;
            _userAppRoleGet = userAppRoleGet;


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

                Console.WriteLine(" Gelen JSON:");
                Console.WriteLine(body);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var payload = JsonSerializer.Deserialize<SaveUserAppRoleRequest>(body, options);

                if (payload != null && payload.UserId > 0 && payload.ApplicationId > 0 && payload.RoleId > 0)
                {
                    Console.WriteLine($" Insert denemesi: UserId={payload.UserId}, AppId={payload.ApplicationId}, RoleId={payload.RoleId}");

                    await _userAppRoleSet.AddUserAppRoleAsync(payload.UserId, payload.ApplicationId, payload.RoleId);

                    Console.WriteLine(" Veritabanýna baþarýyla eklendi!");
                    return new JsonResult(new { success = true });
                }

                Console.WriteLine(" Geçersiz veri!");
                return BadRequest("Eksik veya hatalý veri.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(" HATA: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace); // Hata detaylarýný daha ayrýntýlý görmek için
                return StatusCode(500, "Sunucu hatasý: " + ex.Message);
            }
        }


        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostSaveUserGroupAsync()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var payload = JsonSerializer.Deserialize<SaveUserGroupRequest>(body, options);

            if (payload != null && payload.UserId > 0 && payload.GroupId > 0)
            {
                await _userGroupSet.AddUserGroupAsync(payload.UserId, payload.GroupId);
                return new JsonResult(new { success = true });
            }
            Console.WriteLine("Gelen payload:");
            Console.WriteLine($"UserId: {payload?.UserId}, GroupId: {payload?.GroupId}");

            return BadRequest("Eksik ya da hatalý veri.");
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
                return StatusCode(500, new { error = ex.Message }); // Düz JSON dön
            }
        }


        public class GetUserRequest
        {
            public int UserId { get; set; }
        }


    }


}

