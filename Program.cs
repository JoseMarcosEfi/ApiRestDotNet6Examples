using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);



var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);


//PADRÃO REST
//CREATE
app.MapPost("/products", (Product product) =>
{
    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product.Code);
});

//READ

app.MapGet("/products/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.GetBy(code);
    if (product != null)
        return Results.Ok(product);
    return Results.NotFound();
});

//UPDATE
app.MapPut("/products", (Product product) =>
{
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    return Results.Ok;

});
//DELETE
app.MapDelete("/products/{code}", ([FromRoute] string code) =>
{
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Remove(productSaved);
    return Results.Ok;
});

if (app.Environment.IsStaging())
    app.MapGet("/configuration/database", (IConfiguration configuration) =>
    {
        return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
    });

app.Run();
