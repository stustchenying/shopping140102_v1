using MvcShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcShopping.Controllers
{
    public class CartController : BaseController
    {
        // 加入產品項目到購物車，如果沒有傳入 Amount 參數則預設購買數量為 1
        [HttpPost] // 因為知道要透過 AJAX 呼叫這個 Action，所以可以先標示 [HttpPost] 屬性
        public ActionResult AddToCart(int ProductId, int Amount = 1)
        {
            var product = db.Products.Find(ProductId);

            // 驗證產品是否存在
            if (product == null)
                return HttpNotFound();

            var existingCart = this.Carts.FirstOrDefault(p => p.Product.Id == ProductId);
            if (existingCart != null)
            {
                existingCart.Amount += 1;
            }
            else
            {
                this.Carts.Add(new Cart() { Product = product, Amount = Amount });
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.Created);
        }

        // 顯示目前的購物車項目
        public ActionResult Index()
        {
            return View(this.Carts);
        }

        // 移除購物車項目
        [HttpPost] // 因為知道要透過 AJAX 呼叫這個 Action，所以可以先標示 [HttpPost] 屬性
        public ActionResult Remove(int ProductId)
        {
            var existingCart = this.Carts.FirstOrDefault(p => p.Product.Id == ProductId);
            if (existingCart != null)
            {
                this.Carts.Remove(existingCart);
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        // 更新購物車中特定項目的購買數量
        [HttpPost] // 因為知道要透過 AJAX 呼叫這個 Action，所以可以先標示 [HttpPost] 屬性
        public ActionResult UpdateAmount(List<Cart> Carts)
        {
            foreach (var item in Carts)
            {
                var existingCart = this.Carts.FirstOrDefault(p => p.Product.Id == item.Product.Id);
                if (existingCart != null)
                {
                    existingCart.Amount = item.Amount;
                }
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}
