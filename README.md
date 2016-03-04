# EntityFramework.Include
##Usage
    //Example Entity...
    public class Parent
    {
        public int Id { get; set; }

        public int Age { get; set; }
        
        [NotMapped]
        public int ChildrenCount { get; set; }

        public List<Child> Children { get; set; } = new List<Child>();
    }
    
    public class Child
    {
        public int Id { get; set; }

        public int Age { get; set; }
    }
    
    //In code...
    var list = context.ParentSet.Include(p => p.Children, p => p.Children.Take(10).ToList()).Include(p => P.ChildrenCount, p => p.Children.Count).ToListWithInclude();
    
    Console.WriteLine(list.First().Children.Count); //20
    Console.WriteLine(list.First().ChildrenCount); //output all Children count
