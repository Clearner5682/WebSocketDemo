using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    public class MessageInfo
    {
        public string FromUser { get; set; }
        public string Message { get; set; }
        public bool IsToAll { get; set; }
        public string ToUser { get; set; }
    }
}
