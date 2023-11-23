using BookLoansPorject.Models;
using BookLoansPorject.Repositories;
using BookLoansPorject.Repositories.Interfaces;
using BookLoansPorject.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace BookLoansPorject.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly BookDbContext db = new BookDbContext();
        IGenericBookRepo<Book> bookrepo;
        public BooksController()
        {
            this.bookrepo = new GenericBookRepo<Book>(db);
        }
        // GET: Books
        [AllowAnonymous]
        public ActionResult Index(int pg = 1)
        {
            var data = this.bookrepo.GetAll("BookLoans").ToPagedList(pg, 5);
            return View(data);
        }
        public ActionResult Create()
        {

            BookViewModel b = new BookViewModel();
            b.BookLoans.Add(new BookLoan { });
            return View(b);
        }
        [HttpPost]
        public ActionResult Create(BookViewModel model, string act = "")
        {
            if (act == "add")
            {
                model.BookLoans.Add(new BookLoan { });

                foreach (var er in ModelState.Values)
                {
                    er.Errors.Clear();
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.BookLoans.RemoveAt(index);
                foreach (var er in ModelState.Values)
                {
                    er.Errors.Clear();
                }
            }
            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    var b = new Book
                    {
                        Title = model.Title,
                        Author = model.Author,
                        Published= model.Published,
                        Description= model.Description,
                        Genre = model.Genre,
                        IsAvailable = model.IsAvailable,
                    };
                    string ext = Path.GetExtension(model.BookCover.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string savePath = Server.MapPath("~/Pictures/") + fileName;
                    model.BookCover.SaveAs(savePath);
                    b.BookCover = fileName;
                    foreach (var l in model.BookLoans)
                    {

                        b.BookLoans.Add(l);
                    }
                    this.bookrepo.Insert(b);
                }
            }
            ViewBag.Act = act;
            return PartialView("_CreatePartial", model);
        }
        public ActionResult Edit(int id)
        {
            var x = this.bookrepo.Get(id, "BookLoans");
            var b = new BookEditModel
            {
                Id = x.Id,
                Title = x.Title,
                Author = x.Author,
                Published = x.Published,
                Description = x.Description,
                Genre = x.Genre,
                IsAvailable = x.IsAvailable,
                BookLoans = x.BookLoans.ToList()

            };
            ViewData["CurrentPic"] = x.BookCover;
            return View(b);

        }
        [HttpPost]
        public ActionResult Edit(BookEditModel model, string act = "")
        {
            if (act == "add")
            {
                model.BookLoans.Add(new BookLoan { });
                foreach (var er in ModelState.Values)
                {
                    er.Errors.Clear();
                }
            }

            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.BookLoans.RemoveAt(index);
                foreach (var er in ModelState.Values)
                {
                    er.Errors.Clear();
                }
            }

            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    //var b = db.Books.First(x => x.Id == model.Id);

                    var c = this.bookrepo.Get(model.Id, "BookLoans");
                    var cu = new Book
                    {
                        Title = model.Title,
                        Author = model.Author,
                        Published = model.Published,
                        Description = model.Description,
                        Genre = model.Genre,
                        IsAvailable = model.IsAvailable,
                        BookCover = c.BookCover
                    };
                   

                    if (model.BookCover != null)
                    {
                        string ext = Path.GetExtension(model.BookCover.FileName);
                        string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = Server.MapPath("~/Pictures/") + fileName;
                        model.BookCover.SaveAs(savePath);
                        cu.BookCover = fileName;
                    }
                    this.bookrepo.ExecuteCommand($"DELETE FROM Books WHERE Id={c.Id}");
                    foreach (var l in model.BookLoans)
                    {
                        cu.BookLoans.Add(new BookLoan
                        {
                            BorrowerName = l.BorrowerName,
                            Address = l.Address,
                            Phone = l.Phone,
                            LoanDate = l.LoanDate,
                            ReturnDate = l.ReturnDate,

                        });
                    }

                    this.bookrepo.Insert(cu);
                    return RedirectToAction("Index");
                }

            }
            ViewData["CurrentPic"] = db.Books.First(x => x.Id == model.Id).BookCover;
            return View(model);

        }
        public ActionResult Delete(int id)
        {
            this.bookrepo.ExecuteCommand($"dbo.DeleteBook {id}");
            return Json(new { success = true, deleted = id });
        }
    }
}