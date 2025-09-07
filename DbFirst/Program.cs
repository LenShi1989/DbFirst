using DbFirst.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(Program));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<TodoContext>();


try
{
    // 嘗試打開資料庫連線
    await context.Database.OpenConnectionAsync();
    Console.WriteLine("連線成功！");
    await context.Database.CloseConnectionAsync();
}
catch (Exception ex)
{
    Console.WriteLine("連線失敗：");
    Console.WriteLine(ex.Message);
}


app.Run();




