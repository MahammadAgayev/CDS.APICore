﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDS.APICore.Entities
{
    public class ItemCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string DisplayName { get; set; }
    }
}
