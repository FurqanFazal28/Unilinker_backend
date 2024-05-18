using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Core.Data.DataContext;
using Core.Hubs;
using FYP__.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Access the configuration
var configuration = builder.Configuration;

string kvurl = configuration["KeyVaultConfig:KVUrl"];
string tenantId = configuration["KeyVaultConfig:TenantID"];
string clientId = configuration["KeyVaultConfig:ClientID"];
string clientsecretId = configuration["KeyVaultConfig:ClientSecretID"];

var credentials = new ClientSecretCredential(tenantId, clientId, clientsecretId);

var client = new SecretClient(new Uri(kvurl), credentials);

// Add Azure Key Vault as a configuration source
configuration.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions());
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
new ServiceConfiguration(builder.Services);
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

//app.UseHttpsRedirection();
// Use CORS middleware with specific settings
app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:3000", "http://localhost:7100", "http://localhost:5173", "https://discussionforum-mhqd49pbb-huzaifas-projects-c4dd683b.vercel.app", "https://discussionforum-liard.vercel.app", "https://unilinker-frontend-main-furqan-fazas-projects.vercel.app", "https://unilinker-frontend-main-4k8v16nk3-furqan-fazas-projects.vercel.app", "https://unilinkapiv1.azurewebsites.net/api/Consultation/GetConsultation")
    .AllowCredentials()
    
           .AllowAnyHeader()
           .AllowAnyMethod();
});
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Hello World!");
app.MapHub<ChatHub>("/chat");
app.MapControllers();

app.Run();
