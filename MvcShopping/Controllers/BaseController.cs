using MvcShopping.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MvcShopping.Controllers
{
    public class BaseController : Controller
    {
        protected MvcShoppingContext db = new MvcShoppingContext();

        protected List<Cart> Carts
        {
            get
            {
                if (Session["Carts"] == null)
                {
                    Session["Carts"] = new List<Cart>();
                }
                return (Session["Carts"] as List<Cart>);
            }
            set { Session["Carts"] = value; }
        }
    }
}
