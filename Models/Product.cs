using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gendaccc.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Category { get; set; }
        public double Price { get; set; }
    }

}