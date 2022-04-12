using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ValueObject
{
    class ExecuteBaseInfoResult
    {
        public ExecuteState ExecuteResultState { get; set; }
        public string ExecuteResultMsg { get; set; }

    }
    enum ExecuteState
    {
        /// <summary>
        /// 当前操作需要等待
        /// </summary>
        Wait,
        /// <summary>
        /// 当前操作执行成功
        /// </summary>
        Success,
        /// <summary>
        /// 当前操作执行出错
        /// </summary>
        Error,
    }
}
