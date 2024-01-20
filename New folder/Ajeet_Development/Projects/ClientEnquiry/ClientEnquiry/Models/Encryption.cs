using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ClientEnquiry.Models
{
    public class Encryption
    {    
        public static string EncryptPassword(string password)
        {
            string md5_password = string.Empty;
            using (MD5 hash = MD5.Create())
            {
                md5_password = string.Join("", hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("x2")));
            }
            return md5_password;
        }
    }
}