using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTests.TestDomain
{
    public class TestContext : DbContext
    {
        public DbSet<Parent> Parents { get; set; }

        public DbSet<Son> Sons { get; set; }

        public DbSet<Daughter> Daughters { get; set; }

        public TestContext() : base(ConnectionString)
        {
            Database.SetInitializer(new Testinitializer());
            Configuration.ValidateOnSaveEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
        }

        public static string ConnectionString
            =>
                @"Data Source=(localdb)\ProjectsV12;Initial Catalog=TestDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }

    public class Testinitializer : DropCreateDatabaseIfModelChanges<TestContext>
    {
        protected override void Seed(TestContext context)
        {
            var r = new Random();

            var parents = Enumerable.Range(0, 100).Select(_ => new Parent() { Age = r.Next(100) }).ToList();
            parents.ForEach(p =>
            {
                p.Sons = Enumerable.Range(0, 100).Select(_ => new Son() { Age = r.Next(80), Parent = p}).ToList();
                p.Daughters = Enumerable.Range(0, 100).Select(_ => new Daughter() { Age = r.Next(80), Parent = p }).ToList();
            });

            context.Parents.AddRange(parents);
            context.SaveChanges();
        }
    }
}
