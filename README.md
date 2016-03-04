# EntityFramework.Include
##Usage
    //Example Entity...
    public class Parent
    {
        public int Id { get; set; }

        public int Age { get; set; }

        public List<Child> Children { get; set; } = new List<Child>();
    }
    
    public class Child
    {
        public int Id { get; set; }

        public int Age { get; set; }
    }
    
    //In code...
    var list = context.ParentSet.Include(p => p.Children, p => p.Children.Take(10).ToList()).ToListWithInclude();
    
    Console.WriteLine(list.First().Children.Count); //20
