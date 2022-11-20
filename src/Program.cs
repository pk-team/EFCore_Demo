await using var context = new BlogContext();
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();

context.Blogs.AddRange(
    new Blog { Name = "Blog1", Rating = 1, IsAcitive = true },
    new Blog { Name = "Blog2", Rating = 1, IsAcitive = true },
    new Blog { Name = "Blog3", Rating = 1, IsAcitive = true },
    new Blog { Name = "Blog4", Rating = 2, IsAcitive = true },
    new Blog { Name = "Blog5", Rating = 3, IsAcitive = true }
);

await context.SaveChangesAsync();
context.ChangeTracker.Clear();

await foreach(var blog in context.Blogs.Where(b => b.Rating == 1).AsAsyncEnumerable()) {
    blog.IsAcitive = false;
}
await context.SaveChangesAsync();

Console.WriteLine("----------");
await context.Blogs.Where(t => t.Rating == 1)
    .ExecuteUpdateAsync(updates => updates
    .SetProperty(p => p.IsAcitive, false));

Console.WriteLine("done");


// BLOG context

public class BlogContext : DbContext {

    private bool _isconfiguring;
    public BlogContext() {}
    public BlogContext(DbContextOptions<BlogContext> options) 
        : base(options)
        => _isconfiguring = true;

    protected override void OnConfiguring(DbContextOptionsBuilder options) {
        if (!_isconfiguring) {
            options
            .UseSqlServer(@"server=localhost,9301;database=blog;uid=sa;pwd=DevDevDude119#;Encrypt=False")
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging();
        }
    }

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Post> Posts => Set<Post>();

}

public class Blog {
    public int Id { get; set; }
    public string Name { get; set; } = "";
     public bool IsAcitive { get; set; }
    public int Rating { get; set; }
    public long Counter { get; set; }

    public ICollection<Post>? Posts { get; set; } = new List<Post>();
}

public class Post {
    public int Id { get; set; }
    public long Vies { get; set; }

    public Blog Blog { get; set; } = new Blog();
}
