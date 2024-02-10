using DebateAIApi;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileService>();

var keyVaultUri = new Uri(builder.Configuration.GetSection("KeyVaultURL").Value!);
var azureCredential = new DefaultAzureCredential();
var secretClient = new SecretClient(keyVaultUri, azureCredential);

var storageKey = secretClient.GetSecret(builder.Configuration.GetSection("Storage:Key").Value!);
builder.Configuration["StorageKey"] = storageKey.Value.Value;
builder.Configuration["StorageAccount"] = builder.Configuration.GetSection("Storage:Name").Value;

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
