using CRUDImgs.Class;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.



//builder.Services.AddDbContext<AppContextt>();

//ADD CONNECTION STRING
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<AppContextt>(options => options.UseSqlServer(connection));

//builder.Services.AddControllersWithViews();

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

app.Run();
//app.Run(async(context)=>
//{
//    var request = context.Request;
//    var path = request.Path;

//    var response = context.Response;





//});

//async Task gettAllCars(HttpResponse httpResponse)
//{
//    await httpResponse.WriteAsJsonAsync(img);
//}

//async Task createImg(HttpResponse httpResponse, HttpRequest httpRequest)
//{
//    Images? img1 = await httpRequest.ReadFromJsonAsync<Images>();

//    if (img1 != null)
//    {
//        img1.Id = Guid.NewGuid().ToString();
//        img1.Add(car1);

//        await httpResponse.WriteAsJsonAsync(car1);
//    }
//    else
//    {
//        httpResponse.StatusCode = 400;
//        await httpResponse.WriteAsJsonAsync(new { message = "Error!400" });
//    }
//}
