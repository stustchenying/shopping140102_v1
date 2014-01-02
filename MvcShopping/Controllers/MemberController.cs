using MvcShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcShopping.Controllers
{
    public class MemberController : BaseController
    {
        // 會員註冊頁面
        public ActionResult Register()
        {
            return View();
        }

        // 密碼雜湊所需的 Salt 亂數值
        private string pwSalt = "A1rySq1oPe2Mh784QQwG6jRAfkdPpDa90J0i";

        // 寫入會員資料
        [HttpPost]
        public ActionResult Register([Bind(Exclude="RegisterOn,AuthCode")] Member member)
        {
            // 檢查會員是否已存在
            var chk_member = db.Members.Where(p => p.Email == member.Email).FirstOrDefault();
            if (chk_member != null)
            {
                ModelState.AddModelError("Email", "您輸入的 Email 已經有人註冊過了!");
            }

            if (ModelState.IsValid)
            {
                // 將密碼加鹽(Salt)之後進行雜湊運算以提升會員密碼的安全性
                member.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(pwSalt + member.Password, "SHA1");
                // 會員註冊時間
                member.RegisterOn = DateTime.Now;
                // 會員驗證碼，採用 Guid 當成驗證碼內容，避免有會員使用到重覆的驗證碼
                member.AuthCode = Guid.NewGuid().ToString();

                db.Members.Add(member);
                db.SaveChanges();

                SendAuthCodeToMember(member);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        private void SendAuthCodeToMember(Member member)
        {
            // 準備郵件內容
            string mailBody = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/MemberRegisterEMailTemplate.htm"));

            mailBody = mailBody.Replace("{{Name}}", member.Name);
            mailBody = mailBody.Replace("{{RegisterOn}}", member.RegisterOn.ToString("F"));
            var auth_url = new UriBuilder(Request.Url)
            {
                Path = Url.Action("ValidateRegister", new { id = member.AuthCode }),
                Query = ""
            };
            mailBody = mailBody.Replace("{{AUTH_URL}}", auth_url.ToString());

            // 發送郵件給會員
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("stustchenying", "qaz123456789");
                SmtpServer.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("stustchenying@gmail.com");
                mail.To.Add(member.Email);
                mail.Subject = "【我的電子商務網站】會員註冊確認信";
                mail.Body = mailBody;
                mail.IsBodyHtml = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                //throw ex;
                // 發生郵件寄送失敗，需紀錄進資料庫備查，以免有會員無法登入
            }
        }

        public ActionResult ValidateRegister(string id)
        {
            if (String.IsNullOrEmpty(id))
                return HttpNotFound();
            
            var member = db.Members.Where(p => p.AuthCode == id).FirstOrDefault();

            if (member != null)
            {
                TempData["LastTempMessage"] = "會員驗證成功，您現在可以登入網站了!";
                // 驗證成功後要將 member.AuthCode 的內容清空
                member.AuthCode = null;
                db.SaveChanges();
            }
            else
            {
                TempData["LastTempMessage"] = "查無此會員驗證碼，您可能已經驗證過了!";
            }

            return RedirectToAction("Login", "Member");
        }

        // 顯示會員登入頁面
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        // 執行會員登入
        [HttpPost]
        public ActionResult Login(string email, string password, string returnUrl)
        {
            if (ValidateUser(email, password))
            {
                FormsAuthentication.SetAuthCookie(email, false);

                if (String.IsNullOrEmpty(returnUrl)) {
                    return RedirectToAction("Index", "Home");
                } else {
                    return Redirect(returnUrl);
                }
            }

            return View();
        }

        private bool ValidateUser(string email, string password)
        {
            var hash_pw = FormsAuthentication.HashPasswordForStoringInConfigFile(pwSalt + password, "SHA1");

            var member = (from p in db.Members
                          where p.Email == email && p.Password == hash_pw
                          select p).FirstOrDefault();

            // 如果 member 物件不為 null 則代表會員的帳號、密碼輸入正確
            if (member != null) {
                if (member.AuthCode == null) {
                    return true;
                } else {
                    ModelState.AddModelError("", "您尚未通過會員驗證，請收信並點擊會員驗證連結!");
                    return false;
                }
            } else {
                ModelState.AddModelError("", "您輸入的帳號或密碼錯誤");
                return false;
            }
        }

        // 執行會員登出
        public ActionResult Logout()
        {
            // 清除表單驗證的 Cookies
            FormsAuthentication.SignOut();

            // 清除所有曾經寫入過的 Session 資料
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult CheckDup(string Email)
        {
            var member = db.Members.Where(p => p.Email == Email).FirstOrDefault();

            if (member != null)
                return Json(false);
            else
                return Json(true);
        }

    }
}
