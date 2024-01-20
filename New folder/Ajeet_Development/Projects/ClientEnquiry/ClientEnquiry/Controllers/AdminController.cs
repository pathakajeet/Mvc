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
    public class AdminController : Controller
    {
        public static string ConStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
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
                    SqlCommand cmd = new SqlCommand("usp_VerifyEmployee", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", _loginModel.Username);
                    cmd.Parameters.AddWithValue("@Password", Encryption.EncryptPassword(_loginModel.Password));
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    ds = new DataSet();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        FormsAuthentication.SetAuthCookie(ds.Tables[0].Rows[0]["EmployeeData"].ToString(), false);

                        if(ds.Tables[0].Rows[0]["EmployeeData"].ToString().Split('|')[3]  == "Employee")
                        {
                            return RedirectToAction("AllEmpRequests", "Admin");
                        }
                        else if(ds.Tables[0].Rows[0]["EmployeeData"].ToString().Split('|')[3] == "Admin")
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        else
                        {
                            ViewBag.Message = "Somthing went wrong, please try again later";
                        }                        
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
        [Authorize(Roles = "Admin")]
        public ActionResult Dashboard()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Clients()
        {
            SqlConnection con = null;
            DataSet ds = null;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Client", con);
                cmd.CommandType = CommandType.StoredProcedure;                
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
        public ActionResult AddClient(Client _client)
        {
            SqlConnection con = null;
            DataTable dt = null;
            string Message = string.Empty;
            try
            {                
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Client", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CreatedBy", User.Identity.Name.Split('|')[0]);
                cmd.Parameters.AddWithValue("@Name", _client.ClinetName);
                cmd.Parameters.AddWithValue("@EmailId", _client.ClientEmail);
                cmd.Parameters.AddWithValue("@Password", Encryption.EncryptPassword("123"));
                cmd.Parameters.AddWithValue("@ActionType", "Insert");
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                dt = new DataTable();
                da.Fill(dt);
                if(dt.Rows[0]["msg"].ToString() == "Exist")
                {
                    Message = "Account is already exists with this email id, try other.";
                }
                else if(dt.Rows[0]["msg"].ToString() == "Success")
                {
                    Message = "Created Successfully";
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
        [HttpPost]
        public ActionResult DeleteClient(Client _client)
        {
            SqlConnection con = null;
            string Message = string.Empty;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Client", con);
                cmd.CommandType = CommandType.StoredProcedure;                
                cmd.Parameters.AddWithValue("@ClientId", _client.ClientId);                
                cmd.Parameters.AddWithValue("@ActionType", "Delete");
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i >= 1)
                {
                    Message = "Deleted Successfully";
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
        [HttpPost]
        public ActionResult UpdateClient(Client _client)
        {
            SqlConnection con = null;
            string Message = string.Empty;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Client", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClientId", _client.ClientId);
                cmd.Parameters.AddWithValue("@Name", _client.ClinetName.Trim());
                cmd.Parameters.AddWithValue("@ActionType", "Update");
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i >= 1)
                {
                    Message = "Updated Successfully";
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

        [Authorize(Roles = "Admin")]
        public ActionResult AllRequests(string s, string d, string e)
        {          
            SqlConnection con = null;
            DataSet ds = null;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Request", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", e == "All" ? 0 : Convert.ToInt32(e));
                cmd.Parameters.AddWithValue("@ActionType", "SelectByAdmin");
                cmd.Parameters.AddWithValue("@Status", s);
                cmd.Parameters.AddWithValue("@Department", d);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                ds = new DataSet();
                da.Fill(ds);

            }
            catch (Exception ex)
            {
                ViewBag.Message = "Somthing went wrong, please try again later";
            }

            return View(ds);
        }

        [Authorize(Roles = "Employee")]
        public ActionResult AllEmpRequests()
        {
            SqlConnection con = null;
            DataSet ds = null;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Request", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", User.Identity.Name.Split('|')[0]);
                cmd.Parameters.AddWithValue("@ActionType", "SelectByEmpId");
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                ds = new DataSet();
                da.Fill(ds);

            }
            catch (Exception ex)
            {
                ViewBag.Message = "Somthing went wrong, please try again later";
            }

            return View(ds);
        }
        [Authorize(Roles = "Employee")]
        public ActionResult UpdateEmpRequest(RequestModel _request)
        {
            SqlConnection con = null;
            string Message = string.Empty;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Request", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ReqId", _request.ReqId);
                cmd.Parameters.AddWithValue("@Status", _request.status);
                cmd.Parameters.AddWithValue("@Remark", _request.remark);
                cmd.Parameters.AddWithValue("@ActionType", "UpdateByEmpId");
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i >= 1)
                {
                    Message = "Updated Successfully";
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

            return Json(new { Message = Message, JsonRequestBehavior.AllowGet });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Employees()
        {
            SqlConnection con = null;
            DataSet ds = null;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Employee", con);
                cmd.CommandType = CommandType.StoredProcedure;
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
        public ActionResult AddEmployee(Employee _emp)
        {
            SqlConnection con = null;
            DataTable dt = null;
            string Message = string.Empty;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Employee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CreatedBy", User.Identity.Name.Split('|')[0]);
                cmd.Parameters.AddWithValue("@Name", _emp.EmpName);
                cmd.Parameters.AddWithValue("@EmailId", _emp.EmpEmail);
                cmd.Parameters.AddWithValue("@Department", _emp.EmpDepartment);
                cmd.Parameters.AddWithValue("@EmpRole", _emp.EmpRole);
                cmd.Parameters.AddWithValue("@Password", Encryption.EncryptPassword("123"));
                cmd.Parameters.AddWithValue("@ActionType", "Insert");
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows[0]["msg"].ToString() == "Exist")
                {
                    Message = "Account is already exists with this email id, try other.";
                }
                else if (dt.Rows[0]["msg"].ToString() == "Success")
                {
                    Message = "Created Successfully";
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

        public ActionResult DeleteEmployee(Employee _emp)
        {
            SqlConnection con = null;
            string Message = string.Empty;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Employee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", _emp.EmpId);                
                cmd.Parameters.AddWithValue("@ActionType", "Delete");
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i >= 1)
                {
                    Message = "Deleted Successfully";
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
        [HttpPost]
        public ActionResult UpdateEmployee(Employee _emp)
        {
            SqlConnection con = null;
            string Message = string.Empty;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Employee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", _emp.EmpId);
                cmd.Parameters.AddWithValue("@Name", _emp.EmpName.Trim());
                cmd.Parameters.AddWithValue("@Department", _emp.EmpDepartment.Trim());
                cmd.Parameters.AddWithValue("@ActionType", "Update");
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i >= 1)
                {
                    Message = "Updated Successfully";
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

            return Json(new { Message = Message, JsonRequestBehavior.AllowGet });
        }
        [HttpPost]
        public ActionResult GetEmpListByDepartment(RequestModel _reqModel)
        {
            SqlConnection con = null;
            DataSet ds = null;           
            List<Employee> empList = new List<Employee>();
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Employee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Department", _reqModel.department);
                cmd.Parameters.AddWithValue("@ActionType", "SelectByDepartment");              
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                ds = new DataSet();
                da.Fill(ds);

                if(ds.Tables.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt = ds.Tables[0];
                    foreach(DataRow dr in dt.Rows)
                    {
                        Employee emp = new Employee();
                        emp.EmpId = dr["Id"].ToString();
                        emp.EmpName = dr["EmpName"].ToString();
                        emp.EmpEmail = dr["EmpMailId"].ToString();
                        empList.Add(emp);
                    }
                }               

            }
            catch (Exception ex)
            {
                ViewBag.Message = "Somthing went wrong, please try again later";
            }
            return Json(new { EmployeeList = empList, JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public ActionResult AssignRequest(RequestModel _request)
        {
            SqlConnection con = null;
            string Message = string.Empty;
            try
            {
                con = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand("usp_Request", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ReqId", _request.ReqId);
                cmd.Parameters.AddWithValue("@EmpId", User.Identity.Name.Split('|')[0]);
                cmd.Parameters.AddWithValue("@Department", _request.department);
                cmd.Parameters.AddWithValue("@AssignedTo", _request.assignedTo);
                cmd.Parameters.AddWithValue("@ActionType", "AssignToEmployee");
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i >= 1)
                {
                    Message = "Assigned Successfully";
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

            if (_changePassword != null && _changePassword.Count ==  3)
            {
                if(_changePassword["NewPassword"] == _changePassword["ConfirmPassword"])
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
                                Message = "Password successfully changed";
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