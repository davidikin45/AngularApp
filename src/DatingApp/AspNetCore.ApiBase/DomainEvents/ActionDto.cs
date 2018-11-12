using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ApiBase.DomainEvents
{
    public class ActionDto
    {
        public string Action { get; set; }
        public dynamic Args { get; set; }
    }
}
