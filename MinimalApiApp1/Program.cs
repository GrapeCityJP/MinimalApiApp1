using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TestDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TestDb"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/products/read", async (TestDbContext testDb) =>
{
    var products = await testDb.Products.ToListAsync();

    if (products == null) return Results.NotFound();

    return Results.Ok(products);
});

app.MapGet("/products/read/{id}", async (int id, TestDbContext testDb) =>
{
    var product = await testDb.Products.FindAsync(id);

    if (product == null) return Results.NotFound();

    return Results.Ok(product);
});

app.MapGet("/productsdto/read", async (TestDbContext testDb) =>
{
    var productsdto = await testDb.Products.Select(x => new ProductDto(x)).ToListAsync();

    if (productsdto == null) return Results.NotFound();

    return Results.Ok(productsdto);

});

app.MapGet("/productsdto/read/{id}", async (int id, TestDbContext testDb) =>
{
    var product = await testDb.Products.FindAsync(id);

    if (product == null) return Results.NotFound();

    return Results.Ok(new ProductDto(product));
});


app.Run();

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ToTable("Product", "SalesLT");
    }
}

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string ProductNumber { get; set; } = null!;
    public string? Color { get; set; }
    public decimal StandardCost { get; set; }
    public decimal ListPrice { get; set; }
    public string? Size { get; set; }
    public decimal? Weight { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? ProductModelId { get; set; }
    public DateTime SellStartDate { get; set; }
    public DateTime? SellEndDate { get; set; }
    public DateTime? DiscontinuedDate { get; set; }
    public byte[]? ThumbNailPhoto { get; set; }
    public string? ThumbnailPhotoFileName { get; set; }
    public Guid Rowguid { get; set; }
    public DateTime ModifiedDate { get; set; }
}

public class ProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string ProductNumber { get; set; } = null!;
    public string? Color { get; set; }
    public decimal ListPrice { get; set; }
    public string? Size { get; set; }
    public decimal? Weight { get; set; }

    public ProductDto(Product product) =>
    (ProductId, Name, ProductNumber, Color, ListPrice, Size, Weight) = (product.ProductId, product.Name, product.ProductNumber, product.Color, product.ListPrice, product.Size, product.Weight);
}

