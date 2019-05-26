using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InternetMarket.Web.Models;
using InternetMarket.Domain.Core;
using InternetMarket.Infrastructure.Data;
using System.Text;
using AutoMapper;
using System.Web.Security;

namespace InternetMarket.Web.Controllers
{
    public class AccountController : Controller
    {
        // Register action
        public ActionResult Register()
        {
            return View();
        }
        // Register POST action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            ViewBag.Status = false; 
            ViewBag.Message = "";
            // Model validation
            if (ModelState.IsValid)
            {
                // is login exist
                if (_IsLoginExist(model.Login))
                {
                    ModelState.AddModelError("LoginExist",
                        "Specified login has been already exist");
                    return View(model);
                }
                /* Password hash
                model.Password = Convert.ToBase64String(
                                  System.Security.Cryptography.SHA256.Create()
                                   .ComputeHash(Encoding.UTF8.GetBytes(model.Password)));
                model.ConfirmPassword = Convert.ToBase64String(
                                  System.Security.Cryptography.SHA256.Create()
                                   .ComputeHash(Encoding.UTF8.GetBytes(model.ConfirmPassword)));
                */
                // Save to DB
                using (UserContext db = new UserContext())
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<RegisterViewModel, User>());

                    var user = Mapper.Map<RegisterViewModel, User>(model);
                    user.Role = "Customer";
                    db.Users.Add(user);
                    db.SaveChanges();
                    ViewBag.Message = "User has been successfully created!";
                }

                ViewBag.Status = true;
            }
            else
            {
                ViewBag.Message = "Invalid request";
            }

            return View(model);
        }
        // Login action
        public ActionResult Login()
        {
            return View();
        }

        // Login POST action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string ReturnUrl)
        {
            using (UserContext db = new UserContext())
            {
                var user = db.Users.FirstOrDefault(i => i.Login == model.Login);
                if (user == null)
                {
                    ModelState.AddModelError("LoginNotExist", "Such login isn`t exist");
                }
                else if (string.Compare(user.Password, model.Password) != 0)
                {
                    ModelState.AddModelError("WrongPassword", "Wrong password");
                }
                else
                {
                    // 525600 min - 1 year
                    int timeout = model.RememberMe ? 525600 : 20; 
                    var ticket = new FormsAuthenticationTicket(
                        1,
                        model.Login,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(timeout),
                        model.RememberMe,
                        user.Role
                        );

                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(
                        FormsAuthentication.FormsCookieName,
                        encrypted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeout);
                    cookie.HttpOnly = true;
                    
                    Response.Cookies.Add(cookie);


                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(model);
        }

        // Logout action
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        protected bool _IsLoginExist(string login)
        {
            using (UserContext db = new UserContext())
            {
                return db.Users.FirstOrDefault(i => i.Login == login) != null;
            }
        }
    }
}