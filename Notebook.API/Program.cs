using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Notebook.API;
using Notebook.API.Context;
using Notebook.API.Context.Repository;
using Notebook.API.Extensions;
using Notebook.API.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<NotebookContext>(option =>
{
    var connectionString = builder.Configuration.GetConnectionString("NotebookConnection");
    option.UseSqlite(connectionString);
}).AddUnitOfWork<NotebookContext>()
.AddCustomRepository<NotebookEntity, NoteRepository>()
.AddCustomRepository<Memo, MemoRepository>()
.AddCustomRepository<User, UserRepository>();

builder.Services.AddTransient<INotebookService, NotebookService>();
builder.Services.AddTransient<IMemoService, MemoService>();
builder.Services.AddTransient<ILoginService, LoginService>();

// Add AutoMapper
var automapperConfig = new MapperConfiguration(config =>
{
    config.AddProfile(new AutoMapperProFile());
});
builder.Services.AddSingleton(automapperConfig.CreateMapper());

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyToDo.API", Version = "v1" });
});

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //app.UseSwagger();
    //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notebook.API v1"));
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notebook.API v1"));

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapSwagger();

app.Run();
