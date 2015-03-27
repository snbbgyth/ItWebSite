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
    public class WebContentsController : BaseController
    {
        private IWebContentDal _webContentDal;
        private IWebContentTypeDal _webContentTypeDal;

        public WebContentsController()
        {
            _webContentDal = DependencyResolver.Current.GetService<IWebContentDal>();
            _webContentTypeDal = DependencyResolver.Current.GetService<IWebContentTypeDal>();
        }

        // GET: Admin/WebContents
        public async Task<ActionResult> Index()
        {
            var entityList = await _webContentDal.QueryAllAsync();
            foreach (var entity in entityList)
            {
                entity.WebContentType = await _webContentTypeDal.QueryByIdAsync(entity.WebContentTypeId);
            }

            return View(entityList);
        }

        // GET: Admin/WebContents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebContent webContent = await _webContentDal.QueryByIdAsync(id);
            if (webContent == null)
            {
                return HttpNotFound();
            }
            webContent.WebContentType = await _webContentTypeDal.QueryByIdAsync(webContent.WebContentTypeId);
            return View(webContent);
        }

        // GET: Admin/WebContents/Create
        public async Task<ActionResult> Create()
        {
            var webContent = new WebContent();
            webContent.WebContentTypeList = await _webContentTypeDal.QueryAllAsync();
            return View(webContent);
        }

        // POST: Admin/WebContents/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(WebContent webContent)
        {
            InitInsert(webContent);
            await _webContentDal.InsertAsync(webContent);
            return RedirectToAction("Index");
        }

        // GET: Admin/WebContents/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebContent webContent = await _webContentDal.QueryByIdAsync(id);
            if (webContent == null)
            {
                return HttpNotFound();
            }
            webContent.WebContentType = await _webContentTypeDal.QueryByIdAsync(webContent.WebContentTypeId);
            webContent.WebContentTypeList = await _webContentTypeDal.QueryAllAsync();
            return View(webContent);
        }

        // POST: Admin/WebContents/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WebContent webContent)
        {
            InitModify(webContent);
            await _webContentDal.ModifyAsync(webContent);
            return RedirectToAction("Index");
        }

        // GET: Admin/WebContents/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebContent webContent = await _webContentDal.QueryByIdAsync(id);
            if (webContent == null)
            {
                return HttpNotFound();
            }
            webContent.WebContentType = await _webContentTypeDal.QueryByIdAsync(webContent.WebContentTypeId);
            return View(webContent);
        }

        // POST: Admin/WebContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _webContentDal.DeleteByIdAsync(id);
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
