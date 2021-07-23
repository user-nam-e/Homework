using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SummerBootCampTask2.Services
{
    public class HashService
    {
        public string GetHashCode(string str)
        {
            return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)).ToString();
        }
    }
}
