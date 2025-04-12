﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Shared
{
    public static class StringExtensions
    {
        public static string GetMD5(this string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            var hash = MD5.HashData(Encoding.Default.GetBytes(data));
            return Convert.ToBase64String(hash);//将加密后的字节数组转换为加密字符串
        }
    }
}
