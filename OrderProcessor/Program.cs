using Commons;
using Commons.Model;
using Commons.Model.Order;
using Commons.Model.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderProcessor;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Orders API",
            Version = "v1"
        }
     );
    c.ExampleFilters();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddTransient<IOrdersHistoryRepository, OrdersHistoryRepository>();
builder.Services.AddTransient<IProductLocationRepository, ProductLocationRepository>();
builder.Services.AddTransient<IWebHookEventPublisher, WebHookEventPublisher>();
builder.Services.AddTransient<OrderHandler>();
builder.Services.AddTransient<ListOrderExample>();
builder.Services.AddSwaggerExamples();

builder.Services.AddDbContext<WarehouseDbContext>(options =>
                options.UseInMemoryDatabase(MyConstants.WAREHOUSE_DB));

var serviceProvider =  builder.Services.BuildServiceProvider();
PopulateWarehouseDB(serviceProvider.GetRequiredService<WarehouseDbContext>());


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

static void PopulateWarehouseDB(WarehouseDbContext context)
{
    context.Add(new ProductLocation() { Id = 1, ProductId = 1001, WarehouseArea = "A", WarehouseFloor = 0, WarehouseSection = 1 });
    context.Add(new ProductLocation() { Id = 2, ProductId = 2001, WarehouseArea = "B", WarehouseFloor = 0, WarehouseSection = 20 });
    context.Add(new ProductLocation() { Id = 3, ProductId = 2002, WarehouseArea = "B", WarehouseFloor = 0, WarehouseSection = 21 });
    context.SaveChanges();
}