using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class UsersAdminController : Controller
    {

        public UsersAdminController()
        {
            context = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        public UsersAdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public RoleManager<IdentityRole> RoleManager { get; private set; }
        public ApplicationDbContext context { get; private set; }

        //
        // GET: /Users/
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

            IEnumerable<ApplicationUser> entityList = await UserManager.Users.ToListAsync();
            if (entityList.Any())
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    entityList = entityList.Where(s => s.UserName.Contains(searchString)
                                                   || s.Email.Contains(searchString)
                                                   || s.PhoneNumber.Contains(searchString));
                    entityList = entityList.Where(s => (s.UserName != null && s.UserName.Contains(searchString))
                                                                      || (s.Email != null && s.Email.Contains(searchString))
                                                                      || (s.PhoneNumber != null && s.PhoneNumber.Contains(searchString)));

                }
                switch (sortOrder)
                {
                    case "name_desc":
                        entityList = entityList.OrderByDescending(s => s.UserName);
                        break;
                    default: // Name ascending 
                        entityList = entityList.OrderBy(s => s.UserName);
                        break;
                }
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(entityList.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            ViewBag.RoleNameList = (from roleId in user.Roles
                                    select RoleManager.Roles.FirstOrDefault(t => t.Id == roleId.RoleId)
                                        into role
                                        where role != null
                                        select role.Name).ToList();
            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Id", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, string RoleId)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                user.UserName = userViewModel.UserName;
                //user.HomeTown = userViewModel.HomeTown;
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User Admin to Role Admin
                if (adminresult.Succeeded)
                {
                    if (!String.IsNullOrEmpty(RoleId))
                    {
                        //Find Role Admin
                        var role = await RoleManager.FindByIdAsync(RoleId);
                        var result = await UserManager.AddToRoleAsync(user.Id, role.Name);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First().ToString());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Id", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First().ToString());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                return View();
            }
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");


            ViewBag.UserRoleList = GetUserRole(user);
            return View(user);
        }

        private List<UserRoleViewModel> GetUserRole(ApplicationUser user)
        {
            var userRoleList = new List<UserRoleViewModel> { };
            foreach (var role in user.Roles)
            {
                var entity = RoleManager.Roles.FirstOrDefault(t => t.Id == role.RoleId);
                if (entity != null)
                    userRoleList.Add(new UserRoleViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        RoleId = role.RoleId,
                        RoleName = entity.Name
                    });

            }
            return userRoleList;
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserName,Id")] ApplicationUser formuser, string id,
            string RoleId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
            var user = await UserManager.FindByIdAsync(id);
            user.UserName = formuser.UserName;
            //user.HomeTown = formuser.HomeTown;
            if (ModelState.IsValid)
            {
                //Update the user details
                await UserManager.UpdateAsync(user);
                if (!String.IsNullOrEmpty(RoleId))
                {
                    //Find Role
                    var role = await RoleManager.FindByIdAsync(RoleId);
                    //Add user to new role
                    var result = await UserManager.AddToRoleAsync(id, role.Name);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First().ToString());
                        ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                        return View();
                    }
                }
                return RedirectToAction("Edit", new { id = id });
            }
            else
            {
                ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                return View();
            }
        }

        [ActionName("DeleteRole")]
        public async Task<ActionResult> DeleteRole(string roleName, string userId)
        {
            await UserManager.RemoveFromRoleAsync(userId, roleName);
            return RedirectToAction("Edit", new { id = userId });
        }


        ////
        //// GET: /Users/Delete/5
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var user = await context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        ////
        //// POST: /Users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(string id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        var user = await context.Users.FindAsync(id);
        //        var logins = user.Logins;
        //        foreach (var login in logins)
        //        {
        //            context.UserLogins.Remove(login);
        //        }
        //        var rolesForUser = await IdentityManager.Roles.GetRolesForUserAsync(id, CancellationToken.None);
        //        if (rolesForUser.Count() > 0)
        //        {
        //            foreach (var item in rolesForUser)
        //            {
        //                var result = await IdentityManager.Roles.RemoveUserFromRoleAsync(user.Id, item.Id, CancellationToken.None);
        //            }
        //        }
        //        context.Users.Remove(user);
        //        await context.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}
    }
}