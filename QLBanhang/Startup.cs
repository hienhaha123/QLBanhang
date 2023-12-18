using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using QLBanhang.Models;

[assembly: OwinStartupAttribute(typeof(QLBanhang.Startup))]
namespace QLBanhang
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            InitUserRole();
        }
        private void InitUserRole()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //tạo role Admin
            if(!roleManager.RoleExists("Admin"))// chưa có mới tạo
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
                //tạo user
                var user = new ApplicationUser();
                user.UserName = "admin@qlbh.com";
                user.Email = "admin@qlbh.com";
                string pass = "123456";
                var chkUser = userManager.Create(user, pass);
                context.SaveChanges();
                // ĐƯA USER QLBH VÀO role Admin
                if (chkUser.Succeeded)
                    userManager.AddToRole(user.Id, "Admin");
            }

        }
    }
}
