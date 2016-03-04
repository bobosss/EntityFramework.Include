using System;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.Include.Extensions;
using EntityFramework.Include.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityFramework.Include.Tests
{
    [TestClass]
    public class IncludeTest
    {

        [TestMethod]
        public void Include_Type_Check()
        {
            var queryable = CreateTestData().AsQueryable();
            var list = queryable.Include("ClassBs", _ => _.ClassBs.Take(10).ToList()).ToListWithInclude();

            Assert.AreEqual(list.First().GetType().Name, typeof(TestClassA).Name);
        }

        [TestMethod]
        public void Include_Path_Single()
        {
            var queryable = CreateTestData().AsQueryable();
            var list = queryable.Include("ClassBs", _ => _.ClassBs.Take(10).ToList()).Where(_ => _.Id > 10).ToListWithInclude();

            Assert.IsTrue(list.All(_ => _.ClassBs.Count == 10));
        }

        [TestMethod]
        public void Include_Path_Multiple()
        {
            var queryable = CreateTestData().AsQueryable();
            var list = queryable.Include("ClassBs", _ => _.ClassBs.Take(10).ToList())
                                .Include("ClassCs", _ => _.ClassCs.Take(10).ToList())
                                .Where(_ => _.Id > 10).ToListWithInclude();

            Assert.IsTrue(list.All(_ => _.ClassBs.Count == 10 && _.ClassCs.Count == 10));
        }

        [TestMethod]
        public void Include_Selector_Single()
        {
            var queryable = CreateTestData().AsQueryable();
            var list = queryable.Include(_ => _.ClassBs, _ => _.ClassBs.Take(10).ToList()).Where(_ => _.Id > 10).ToListWithInclude();

            Assert.IsTrue(list.All(_ => _.ClassBs.Count == 10));
        }

        [TestMethod]
        public void Include_Selector_Multiple()
        {
            var queryable = CreateTestData().AsQueryable();
            var list = queryable.Include(_ => _.ClassBs, _ => _.ClassBs.Take(10).ToList())
                                .Include(_ => _.ClassCs, _ => _.ClassCs.Take(10).ToList())
                                .Where(_ => _.Id > 10).ToListWithInclude();

            Assert.IsTrue(list.All(_ => _.ClassBs.Count == 10 && _.ClassCs.Count == 10));
        }

        [TestMethod]
        public void Include_But_Type_Changed()
        {
            var queryable = CreateTestData().AsQueryable();
            var list = queryable.Include("ClassBs", _ => _.ClassBs.Take(50).ToList())
                                .Select(_ => new {ClassBs = _.ClassBs.Take(10).ToList()}).ToListWithInclude();

            Assert.IsTrue(list.All(_ => _.ClassBs.Count == 10));
        }

        [TestMethod]
        public void Include_Path_Duplicate_Property_Set()
        {
            var queryable = CreateTestData().AsQueryable();
            var list = queryable.Include("ClassBs", _ => _.ClassBs.Take(50).ToList())
                                .Include("ClassBs", _ => _.ClassBs.Take(10).ToList())
                                .Where(_ => _.Id > 10).ToListWithInclude();

            Assert.IsTrue(list.All(_ => _.ClassBs.Count == 10));
        }

        [TestMethod]
        public void Include_Selector_Duplicate_Property_Set()
        {
            var queryable = CreateTestData().AsQueryable();
            var list = queryable.Include(_ => _.ClassBs, _ => _.ClassBs.Take(50).ToList())
                                .Include(_ => _.ClassBs, _ => _.ClassBs.Take(10).ToList())
                                .Where(_ => _.Id > 10).ToListWithInclude();

            Assert.IsTrue(list.All(_ => _.ClassBs.Count == 10));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Include_Path_Not_Class_Member()
        {
            var queryable = CreateTestData().AsQueryable();
            var list = queryable.Include("ClassDs", _ => _.ClassBs.Take(50).ToList())
                                .Where(_ => _.Id > 10).ToListWithInclude();
        }

        #region TestData
        private static List<TestClassA> CreateTestData()
        {
            var list = Enumerable.Range(0, 100).Select(_ => new TestClassA() { Id = _ }).ToList();
            list.ForEach(_ =>
            {
                _.ClassBs = Enumerable.Range(0, 100).Select(__ => new TestClassB() { Id = __ }).ToList();
                _.ClassCs = Enumerable.Range(0, 100).Select(__ => new TestClassC() { Id = __ }).ToList();
            });

            return list;
        }
        #endregion
    }
}
