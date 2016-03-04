using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTests.TestDomain
{
    public class Son
    {
        public int Id { get; set; }

        public int Age { get; set; }

        public Parent Parent { get; set; }
    }
}
