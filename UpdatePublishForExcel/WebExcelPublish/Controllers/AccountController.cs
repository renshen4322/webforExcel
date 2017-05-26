﻿using Newtonsoft.Json;
using publishCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebExcelPublish.Models;

namespace WebExcelPublish.Controllers
{
    public class AccountController : Controller
    {
        private string tokenUrl = "https://api.vidahouse.com/Token"; //获取key url
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Dictionary<string, string> httpParams = new Dictionary<string, string>();
                httpParams.Add("UserName", model.UserName);
                httpParams.Add("grant_type", "password");
                httpParams.Add("Password", model.Password);               
                var respon = HttpRequstHelper.OperateRequest(EnumExtenstions.GetDescription(MethodType.POST), tokenUrl, httpParams, "application/x-www-form-urlencoded");
                TokenResponseEntity entity = new TokenResponseEntity();
                if (respon.StatusCode != 200)
                {
                    ModelState.AddModelError("", "用户名或密码不正确");
                }
                else
                {
                    if (model.RememberMe == true)
                    {
                        HttpCookie cookie = new HttpCookie("COOKIE_NAME_FOR_USER");
                        cookie.Expires = DateTime.Now.AddMinutes(30);
                        cookie["COOKIE_USER_NAME"] = model.UserName;
                        cookie["COOKIE_USER_PASS"] = model.Password;
                        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("COOKIE_NAME_FOR_USER");
                        cookie.Expires = DateTime.Now.AddMinutes(-30);
                        Request.Cookies.Add(cookie);
                        cookie["COOKIE_USER_NAME"] = null;
                        cookie["COOKIE_USER_PASS"] = null;
                        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                    }
                    var result = JsonConvert.DeserializeObject<TokenResponseEntity>(respon.Body.ToString());                    
                    SessionHelper.SetSession("UserName", result.UserName);
                    SessionHelper.SetSession("Token", result.Access_token);                    
                    return RedirectToAction("Index", "PublishExcel");
                }
               
            }
            
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogOff()
        {            
            Session.RemoveAll();
            return RedirectToAction("Login", "Account");
        }
    }
}