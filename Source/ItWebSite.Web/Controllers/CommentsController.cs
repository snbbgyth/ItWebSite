using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;

namespace ItWebSite.Web.Controllers
{
    public class CommentsController : Controller
    {
        private ICommentDal _commentDal;

        public CommentsController()
        {
            _commentDal =  DependencyResolver.Current.GetService<ICommentDal>();
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Phone,UserName,Content,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                InitAuthor(comment);
                await _commentDal.InsertAsync(comment);
                return RedirectToAction("Successful");
            }
            return View(comment);
        }

        public ActionResult Successful()
        {
            ViewBag.Message = "提交成功,谢谢你的留言。";
            return View();
        }

        private void InitAuthor(Comment comment)
        {
            comment.CreateDate = DateTime.Now;
            comment.LastModifyDate = DateTime.Now;
            if (User.Identity.IsAuthenticated)
            {
                comment.Creater = User.Identity.Name;
                comment.LastModifier = User.Identity.Name;
            }
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
