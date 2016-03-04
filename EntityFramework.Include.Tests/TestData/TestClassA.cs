using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Include.Tests.TestData
{

    public class TestClassA
    {
        public int Id { get; set; }
        public List<TestClassB> ClassBs { get; set; } = new List<TestClassB>();
        public List<TestClassC> ClassCs { get; set; } = new List<TestClassC>();
    }
}
