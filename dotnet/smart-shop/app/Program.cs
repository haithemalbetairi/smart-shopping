using app.Components;
using Microsoft.Data.SqlClient;
using System;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// For production scenarios, consider keeping Swagger configurations behind the environment check
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

string connectionString = app.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")!;

app.MapGet("/Cart", () => {
    var rows = new List<string>();

    using var conn = new SqlConnection(connectionString);
    conn.Open();

    var command = new SqlCommand("SELECT * FROM OrderItems", conn);
    using SqlDataReader reader = command.ExecuteReader();

    if (reader.HasRows)
    {
        while (reader.Read())
        {
            rows.Add($"{reader.GetInt32(0)}, {reader.GetInt32(1)}, {reader.GetInt32(2)}, {reader.GetInt32(3)}, {reader.GetString(4)}");
        }
    }

    return rows;
})
.WithName("GetOrderItems")
.WithOpenApi();

app.MapPost("/Cart", (OrderItem item) => {
    using var conn = new SqlConnection(connectionString);
    conn.Open();

    var command = new SqlCommand(
        "INSERT INTO dbo.OrderItems (OrderItemsId, ProductId, CartId, Quantity, SaleStatus) " +
        "VALUES (@itemId, @productId, @cartId, @quantity, @saleStatus),",
        conn);

    command.Parameters.Clear();
    command.Parameters.AddWithValue("@itemId", item.OrderItemId);
    command.Parameters.AddWithValue("@productId", item.ProductId);
    command.Parameters.AddWithValue("@cartId", item.CartId);
    command.Parameters.AddWithValue("@productId", item.Quantity);
    command.Parameters.AddWithValue("@productId", item.SaleStatus);

    using SqlDataReader reader = command.ExecuteReader();
})
.WithName("CreateOrderItem")
.WithOpenApi();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
public class Cart
{
    public int CartId { get; set; }
    public int OrderId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
public class Order
{
    public int OrderId { get; set; }
    public int PaymentId { get; set; }
    public int UserId { get; set; }
    public required string OrderStatus { get; set; }
    public double Total { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
public class OrderItem
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public int CartId { get; set; }
    public int Quantity { get; set; }
    public required string SaleStatus { get; set; }
}
public class Product
{
    public required int ProductId { get; set; }
    public required string Name { get; set; }
    public int StoreId { get; set; }
    public required float Price { get; set; }
    public float Weight { get; set; }
    public float Size { get; set; }
    public required string RestockThreshold { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ModifiedAt { get; set; }
}




