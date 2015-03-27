using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Web.Areas.Admin.Models;
using ItWebSite.Web.DAL.MySql;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "Admin")]
    public class RolesAdminController : Controller
    {
        public RolesAdminController()
        {
            context = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        public RolesAdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public RoleManager<IdentityRole> RoleManager { get; private set; }
        public ApplicationDbContext context { get; private set; }
        //
        // GET: /Roles/
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            IEnumerable<IdentityRole> entityList =   RoleManager.Roles.ToList();
            if (entityList.Any())
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    entityList = entityList.Where(s => s.Name.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        entityList = entityList.OrderByDescending(s => s.Name);
                        break;

                    default: // Name ascending 
                        entityList = entityList.OrderBy(s => s.Name);
                        break;
                }
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(entityList.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Roles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            ViewBag.UserNameList = (from userId in role.Users select UserManager.Users.FirstOrDefault(t => t.Id == userId.UserId) into user where user != null select user.UserName).ToList();
            return View(role);
        }

        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First().ToString());
                    return View();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        // GET: /Roles/Edit/Admin
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id")] IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var result = await RoleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                var result = await RoleManager.DeleteAsync(role);
                RemoveRole(role);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        private async void RemoveRole(IdentityRole role)
        {
            var userIdList = new List<string>();
       
            foreach (var user in UserManager.Users.ToList())
            {
                foreach (var userRole in user.Roles)
                {
                    if (userRole.RoleId == role.Id)
                        userIdList.Add(user.Id);
                }
            }
            foreach (var userId in userIdList)
            {
                try
                {
                    await UserManager.RemoveFromRoleAsync(userId, role.Name);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}