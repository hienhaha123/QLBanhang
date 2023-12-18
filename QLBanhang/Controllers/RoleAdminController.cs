using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using QLBanhang.Models;
using System.Net;
using System.Threading.Tasks;

namespace QLBanhang.Controllers
{
    [Authorize(Roles ="Admin")] // chỉ cho các user thuộc role admin mới truy cập contrller này
    public class RoleAdminController : Controller
    {
        // GET: RoleAdmin
        public ActionResult Index()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            return View(roleManager.Roles);
        }

        // GET: RoleAdmin/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}
        public async Task<ActionResult>Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var roleMager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var role = await roleMager.FindByIdAsync(id);
            var context = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var users = new List<ApplicationUser>();
            foreach(var user in userManager.Users.ToList())
            {
                if (await userManager.IsInRoleAsync(user.Id, role.Name))
                    users.Add(user);
            }
            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }
        // GET: RoleAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoleAdmin/Create
        [HttpPost]
        public async Task<ActionResult>Create(RoleViewModel roleViewModel)
        {
            var context = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var roleResult = await roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("", roleResult.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: RoleAdmin/Edit/5
        public async Task< ActionResult> Edit(string id)
        {
            var context = new ApplicationDbContext();
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
                return HttpNotFound();
            RoleViewModel roleModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };
            return View(roleModel);

        }

        // POST: RoleAdmin/Edit/5
        [HttpPost]
        public async Task< ActionResult> Edit(RoleViewModel roleModel)
        {
            var context = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = await roleManager.FindByIdAsync(roleModel.Id);
                if (role != null)
                {
                    role.Name = roleModel.Name;
                    await roleManager.UpdateAsync(role);
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: RoleAdmin/Delete/5
        public async Task< ActionResult> Delete(string id)
        {
            var context = new ApplicationDbContext();
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
                return HttpNotFound();
            return View(role);
        }

        // POST: RoleAdmin/Delete/5
        [HttpPost]
        public async Task< ActionResult> Delete(string id, string tmp ="")
        {
            if (ModelState.IsValid)
            {
                var context = new ApplicationDbContext();
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = await roleManager.FindByIdAsync(id);
                if (role == null)
                    return HttpNotFound();
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
