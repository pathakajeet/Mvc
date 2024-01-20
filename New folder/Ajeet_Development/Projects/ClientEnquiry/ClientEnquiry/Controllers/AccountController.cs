using ClientEnquiry.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ClientEnquiry.Controllers
{
    public class AccountController : Controller
    {
        public static string ConStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        [HttpGet]
        public ActionResult Login()
        {            
            return View();
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel _loginModel)
        {           
            SqlConnection con = null;
            DataSet ds = null;
            if (ModelState.IsValid)
            {
                try
                {
                    con = new SqlConnection(ConStr);
                    SqlCommand cmd = new SqlCommand("usp_VerifyClient", con);
                    cmd.CommandType = CommandType.StoredProcedure;                    
                    cmd.Parameters.AddWithValue("@Username", _loginModel.Username);
                    cmd.Parameters.AddWithValue("@Password", Encryption.EncryptPassword(_loginModel.Password));
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    ds = new DataSet();
                    da.Fill(ds);
                    if(ds.Tables[0].Rows.Count > 0)
                    {                        
                        FormsAuthentication.SetAuthCookie(ds.Tables[0].Rows[0]["ClientDetails"].ToString(), false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Message = "Invalid Username/ Password";
                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "Somthing went wrong, please try again later";
                }
            }
            else
            {
                ViewBag.Message = "Invalid data entered";
            }
            

            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(Dictionary<string, string> _changePassword)
        {
            SqlConnection con = null;
            string Message = string.Empty;

            if (_changePassword != null && _changePassword.Count == 3)
            {
                if (_changePassword["NewPassword"] == _changePassword["ConfirmPassword"])
                {
                    try
                    {
                        con = new SqlConnection(ConStr);
                        SqlCommand cmd = new SqlCommand("usp_ChangePassword", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmpId", User.Identity.Name.Split('|')[0]);
                        cmd.Parameters.AddWithValue("@OldPassword", Encryption.EncryptPassword(_changePassword["OldPassword"]));
                        cmd.Parameters.AddWithValue("@NewPassword", Encryption.EncryptPassword(_changePassword["NewPassword"]));
                        cmd.Parameters.AddWithValue("@ChangeFor", "Emloyee");
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0]["msg"].ToString() == "Changed")
                            {
                                Message = "Password successfully chnaged";
                            }
                            else
                            {
                                Message = "You entered wrong old password";
                            }
                        }
                        else
                        {
                            Message = "Somthing went wrong, please try again later";
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = "Somthing went wrong, please try again later";
                    }
                }
                else
                {
                    Message = "New password and confirm password are not matching";
                }

            }
            else
            {
                Message = "Invalid data supplied";
            }

            return Json(new { Message = Message, JsonRequestBehavior.AllowGet });
        }

        public ActionResult Logout() 
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login"); 
        }

    }
}