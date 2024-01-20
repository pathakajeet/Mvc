using ClientEnquiry.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientEnquiry.Controllers
{
    [Authorize(Roles ="Client")]
    public class HomeController : Controller
    {
        public static string ConStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public ActionResult Index()
        {
            var role = System.Web.Security.Roles.GetRolesForUser().Single();

            return View();
        }
        public ActionResult Requests()
        {
            SqlConnection con = null;
            DataSet ds = null;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Request", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClientId", User.Identity.Name.Split('|')[0]);
                cmd.Parameters.AddWithValue("@ActionType", "Select");
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                ds = new DataSet();
                da.Fill(ds);
                
            }
            catch (Exception)
            {
                ViewBag.Message = "Somthing went wrong, please try again later";
            }

            return View(ds);
        }
        [HttpPost]
        public ActionResult SubmitRequest(RequestModel _requestModel)
        {
            SqlConnection con = null;
            string Message = string.Empty;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Request", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ClientId", User.Identity.Name.Split('|')[0]);
                cmd.Parameters.AddWithValue("@SupportType", _requestModel.supportType);
                cmd.Parameters.AddWithValue("@Priority", _requestModel.priority);
                cmd.Parameters.AddWithValue("@Department", _requestModel.department);
                cmd.Parameters.AddWithValue("@PersonName", _requestModel.personName);
                cmd.Parameters.AddWithValue("@PersonMobile", _requestModel.personMobile);
                cmd.Parameters.AddWithValue("@PersonEmail", _requestModel.personEmail);
                cmd.Parameters.AddWithValue("@Details", _requestModel.details);
                cmd.Parameters.AddWithValue("@ActionType", "Insert");
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i >= 1)
                {
                    Message = "Success";
                }
                else
                {
                    Message = "Somthing went wrong, please try again later";
                }
            }
            catch (Exception)
            {
                Message = "Somthing went wrong, please try again later";
            }

            return Json(new { Message = Message, JsonRequestBehavior.AllowGet });
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
                        cmd.Parameters.AddWithValue("@ChangeFor", "Client");
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

    }
}