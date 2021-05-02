using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDS.APICore.Models.Order
{
    public class AddItemCategeoryRequest
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public string DisplayName { get; set; }
    }
}
