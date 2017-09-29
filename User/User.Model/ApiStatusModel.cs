using System;
using System.Collections.Generic;
using System.Text;

namespace User.Model
{
    public class ApiStatusModel<T>
    {
        /// <summary>
        /// 状态代码
        /// </summary>
        public string statecode { get; set; }

        /// <summary>
        /// 返回的内容
        /// </summary>
        public T content { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string errorcode { get; set; }
    }
}
