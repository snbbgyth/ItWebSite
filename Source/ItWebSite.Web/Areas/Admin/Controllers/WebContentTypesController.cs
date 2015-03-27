using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.DAL;
using ItWebSite.Web.DAL.MySql;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "Admin,Edit")]
    public class WebContentTypesController : BaseController
    {

        private IWebContentTypeDal _webContentTypeDal;

        public WebContentTypesController()
        {
            _webContentTypeDal = DependencyResolver.Current.GetService<IWebContentTypeDal>();
        }

        // GET: Admin/WebContentTypes
        public async Task<ActionResult> Index()
        {
            return View(await _webContentTypeDal.QueryAllAsync());
        }

        // GET: Admin/WebContentTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebContentType webContentType = await _webContentTypeDal.QueryByIdAsync(id);
            if (webContentType == null)
            {
                return HttpNotFound();
            }
            return View(webContentType);
        }

        // GET: Admin/WebContentTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/WebContentTypes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public async Task<ActionResult> Create(WebContentType webContentType)
        {
            if (!string.IsNullOrEmpty(webContentType.Name))
            {
                InitInsert(webContentType);
                await _webContentTypeDal.InsertAsync(webContentType);
                return RedirectToAction("Index");
            }
            return View(webContentType);
        }

        // GET: Admin/WebContentTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebContentType webContentType = await _webContentTypeDal.QueryByIdAsync(id);
            if (webContentType == null)
            {
                return HttpNotFound();
            }
            return View(webContentType);
        }

        // POST: Admin/WebContentTypes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] WebContentType webContentType)
        {
            if (ModelState.IsValid)
            {
                InitModify(webContentType);
                await _webContentTypeDal.ModifyAsync(webContentType);
                return RedirectToAction("Index");
            }
            return View(webContentType);
        }

        // GET: Admin/WebContentTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebContentType webContentType = await _webContentTypeDal.QueryByIdAsync(id);
            if (webContentType == null)
            {
                return HttpNotFound();
            }
            return View(webContentType);
        }

        // POST: Admin/WebContentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            await _webContentTypeDal.DeleteByIdAsync(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
            base.Dispose(disposing);
        }
    }
}
