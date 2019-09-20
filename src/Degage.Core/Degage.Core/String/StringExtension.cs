﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Degage
{
    /// <summary>
    /// 为 <see cref="String"/> 类型增加常用扩展方法
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 判断字符串是否不为空引用或者空字符
        /// </summary>
        public static Boolean IsNotNullOrEmpty(this String str)
        {
            return !String.IsNullOrEmpty(str);
        }

        /// <summary>
        ///判断字符串是空引用或者是空字符串
        /// </summary>
        public static Boolean IsNullOrEmpty(this String str)
        {
            return String.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 去除字符串两端的空白字符串，若字符串为空则返回空引用
        /// </summary>
        public static String TrimWithNull(this String str)
        {
            String result = str.Trim();
            if (str.IsNullOrEmpty())
            {
                return null;
            }
            return result;
        }

        /// <summary>
        ///判断字符串是空引用，或者完全是由空白字符组成的
        /// </summary>
        public static Boolean IsNullOrWhiteSpace(this String str)
        {
            return String.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        ///判断字符串是否完全由数字和小数点（一个）构成
        /// </summary>
        public static Boolean IsNumeric(this String str)
        {
            return Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        ///判断字符串是否是一个合法的中国大陆身份证号码
        /// </summary>
        public static Boolean IsIdCardNumber(this String str)
        {
            return Regex.IsMatch(str, @"\d{17}[Xx\d]");
        }
        /// <summary>
        ///判断字符串是否是一个合法的中国大陆手机号码
        /// </summary>
        public static Boolean IsMobileNumber(this String str)
        {
            if (str.IsNullOrEmpty()) return false;
            return Regex.IsMatch(str, @"\d{11}");
        }

        /// <summary>
        /// 判断字符串是否是一个合法的中国大陆电话号码
        /// </summary>
        public static Boolean IsPhoneNumber(this String str)
        {
            return Regex.IsMatch(str, @"\d{4}-?\d{7}");
        }

        /// <summary>
        /// 判断字符串是否是一个有效的邮箱地址
        /// </summary>
        public static Boolean IsEmailAddress(this String str)
        {
            return Regex.IsMatch(str, @"\S+@\S+\.\S+");
        }
    }
}
