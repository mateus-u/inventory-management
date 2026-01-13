using Application.Common.Interfaces;
using Application.Common.Services.AuditService.Interface;
using Application.Common.Services.WMSService.Interface;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Application.FunctionalTests;

public class TestingBase : IAsyncLifetime
{
    protected IServiceProvider ServiceProvider { get; private set; } = null!;
    protected ApplicationDbContext DbContext { get; private set; } = null!;
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        // Configure InMemory Database with shared root
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(_databaseName)
                   .EnableSensitiveDataLogging());

        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());

        // Mock external services
        var mockAuditService = new Mock<IAuditService>();
        services.AddScoped(_ => mockAuditService.Object);

        var mockWmsService = new Mock<IWmsService>();
        services.AddScoped(_ => mockWmsService.Object);

        // Add Application services
        services.AddMediator(typeof(IApplicationDbContext).Assembly);

        ServiceProvider = services.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (DbContext != null)
        {
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.DisposeAsync();
        }

        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    protected async Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, Task<T>> action)
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await action(context);
    }

    protected async Task ExecuteDbContextAsync(Func<ApplicationDbContext, Task> action)
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await action(context);
    }

    protected T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }
}
