using Microsoft.EntityFrameworkCore;
using Nlpc_Pension_Project.Application.MappingProfile;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Nlpc_Pension_Project.Application.Services.Interface;
using Nlpc_Pension_Project.Common.validations;
using Nlpc_Pension_Project.Application.Services.Implementations;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Infrastructure.AppDbContext;
using Nlpc_Pension_Project.Infrastructure.BackGroundJobs;
using Nlpc_Pension_Project.Infrastructure.Repository;
using Nlpc_Pension_Project.Application.Services;









var builder = WebApplication.CreateBuilder(args);


// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<MemberValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployerDtoValidator>();
//builder.Services.AddValidatorsFromAssemblyContaining<BenefitRequestDto>();
//builder.Services.AddValidatorsFromAssemblyContaining<BenefitRequestDtoValidator>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

// Configure services
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IContributionService, ContributionService>();
builder.Services.AddScoped<ICalculateBenefit, CalculateBenefitService>();
builder.Services.AddScoped<IEmployerService, EmployerService>();
builder.Services.AddScoped<IBackgroundJobService, BackgroundJobs>();

// Configure Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();





builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Register background jobs with scoped service resolution


// Optional: Enable Hangfire dashboard if needed
app.UseHangfireDashboard("/hangfire");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Apply EF Core migrations automatically
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}
using (var scope = app.Services.CreateScope())
{
    var jobService = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
    jobService.RegisterJobs();
}

app.Run();



