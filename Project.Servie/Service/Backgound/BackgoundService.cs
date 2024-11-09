using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Project.Service.Service.Orders;
using Project.Service.Service.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class BackgoundService : BackgroundService
{

    private readonly ILogger<BackgoundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public BackgoundService(ILogger<BackgoundService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IOrderService>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                //await service.UpdateAllTransactionsStatusToFailedAsync();
                await service.UpdatePendingOrdersToConfirmedAsync();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing scheduled tasks.");
            }

            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }
    }

}

