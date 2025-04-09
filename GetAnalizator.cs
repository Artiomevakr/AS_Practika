using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedLab
{
    public class GetAnalizator
    {
        public string patient { get; set; }
        public List<Services> Services { get; set; }
        public int progress { get; set; }
    }
}
