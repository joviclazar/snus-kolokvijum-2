var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryDataStore>();
builder.Services.AddHostedService<CupidWorker>();
builder.Services.AddSignalR();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapHub<CupidHub>("/cupidHub");

app.Run();