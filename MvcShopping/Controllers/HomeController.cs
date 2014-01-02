using MvcShopping.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace MvcShopping.Controllers
{
    public class HomeController : BaseController
    {
        // 首頁
        public ActionResult Index()
        {
            var data = db.ProductCategories.ToList();
#if DEBUG
            // 插入範例資料 (測試用)
            if (data.Count == 0)
            {
                db.ProductCategories.Add(new ProductCategory() { Id = 1, Name = "文具" });
                db.ProductCategories.Add(new ProductCategory() { Id = 2, Name = "禮品" });
                db.ProductCategories.Add(new ProductCategory() { Id = 3, Name = "書籍" });
                db.ProductCategories.Add(new ProductCategory() { Id = 4, Name = "美勞用具" });
                db.SaveChanges();
                data = db.ProductCategories.ToList();
            }
#endif

            return View(data);
        }

        // 商品列表
        public ActionResult ProductList(int id, int p = 1)
        {
            var productCategory = db.ProductCategories.Find(id);

            if (productCategory != null)
            {
                var data = productCategory.Products.ToList();

#if DEBUG
                // 插入範例資料 (測試用)
                if (data.Count == 0)
                {
                    productCategory.Products.Add(new Product() { Name = productCategory.Name + "類別下的商品1", Color = Color.Red, Description = "test", Price = 99, PublishOn = DateTime.Now, ProductCategory = productCategory });
                    productCategory.Products.Add(new Product() { Name = productCategory.Name + "類別下的商品2", Color = Color.Blue, Description = "N/A", Price = 150, PublishOn = DateTime.Now, ProductCategory = productCategory });
                    //productCategory.Products.Add(new Product() { Name = productCategory.Name + "類別下的商品3", Color = Color.Blue, Description = "N/A", Price = 150, PublishOn = DateTime.Now, ProductCategory = productCategory });
                    //productCategory.Products.Add(new Product() { Name = productCategory.Name + "類別下的商品4", Color = Color.Blue, Description = "N/A", Price = 150, PublishOn = DateTime.Now, ProductCategory = productCategory });
                    //productCategory.Products.Add(new Product() { Name = productCategory.Name + "類別下的商品5", Color = Color.Blue, Description = "N/A", Price = 150, PublishOn = DateTime.Now, ProductCategory = productCategory });
                    //productCategory.Products.Add(new Product() { Name = productCategory.Name + "類別下的商品6", Color = Color.Blue, Description = "N/A", Price = 150, PublishOn = DateTime.Now, ProductCategory = productCategory });
                    //productCategory.Products.Add(new Product() { Name = productCategory.Name + "類別下的商品7", Color = Color.Blue, Description = "N/A", Price = 150, PublishOn = DateTime.Now, ProductCategory = productCategory });
                    //productCategory.Products.Add(new Product() { Name = productCategory.Name + "類別下的商品8", Color = Color.Blue, Description = "N/A", Price = 150, PublishOn = DateTime.Now, ProductCategory = productCategory });
                    //productCategory.Products.Add(new Product() { Name = productCategory.Name + "類別下的商品9", Color = Color.Blue, Description = "N/A", Price = 150, PublishOn = DateTime.Now, ProductCategory = productCategory });
                       
                    db.SaveChanges();

                    data = productCategory.Products.ToList();
                }
#endif
                var pagedData = data.ToPagedList(pageNumber: p, pageSize: 10);

                return View(pagedData);
            }
            else
            {
                return HttpNotFound();
            }
        }

        // 商品明細
        public ActionResult ProductDetail(int id)
        {
            var data = db.Products.Find(id);

            return View(data);
        }
    }
}
