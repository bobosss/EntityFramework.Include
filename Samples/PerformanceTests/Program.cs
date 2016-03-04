using System;
using System.Diagnostics;
using System.Linq;
using EntityFramework.Include.Extensions;
using PerformanceTests.TestDomain;

namespace PerformanceTests
{
    class Program
    {
        static void Main(string[] args)
        {
            WarmUp();

            var trial = 30;
            var watch = new Stopwatch();

            using (var context = new TestContext())
            {
                watch.Restart();
                for (int i = 0; i < trial; i++)
                {
                    var list =
                        context.Parents.Include(_ => _.Sons, _ => _.Sons.Where(__ => __.Age > 10).ToList())
                            .Include(_ => _.Daughters, _ => _.Daughters.Where(__ => __.Age > 10).ToList())
                            .ToListWithInclude();
                }
                watch.Stop();
                Console.WriteLine($"EntityFramework.Include : {watch.ElapsedMilliseconds / (double)trial}ms");

                watch.Restart();
                for (int i = 0; i < trial; i++)
                {
                    var list =
                        context.Parents.Select(
                            _ =>
                                new
                                {
                                    Id = _.Id,
                                    Age = _.Age,
                                    Sons = _.Sons.Where(__ => __.Age > 10).ToList(),
                                    Daughters = _.Daughters.Where(__ => __.Age > 10).ToList()
                                }).ToList()
                                .Select(_ => new Parent() {Id = _.Id, Age = _.Age, Sons = _.Sons, Daughters = _.Daughters}).ToList();
                }
                watch.Stop();
                Console.WriteLine($"Raw EntityFramework(Cast AnoymousType) : {watch.ElapsedMilliseconds / (double)trial}ms");
            }

            Console.ReadKey();
        }

        static void WarmUp()
        {
            using (var context = new TestContext())
            {
                var p = new Parent();
                context.Parents.Add(p);
                context.SaveChanges();

                context.Parents.Remove(context.Parents.Find(p.Id));
                context.SaveChanges();
            }
        }
    }
}
