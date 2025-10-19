var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddSingleton<ProductService>();

// Add a named CORS policy for local development and only allow the Blazor client origin(s).
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev",
        policy => policy
            .WithOrigins("http://localhost:5048", "https://localhost:5048")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enable CORS for requests from the client during development. Apply this before
// HTTPS redirection so CORS preflight (OPTIONS) isn't redirected and blocked.
app.UseCors("LocalDev");

app.UseHttpsRedirection();

// Map attribute-routed controllers (e.g. /api/products)
app.MapControllers();

app.Run();


