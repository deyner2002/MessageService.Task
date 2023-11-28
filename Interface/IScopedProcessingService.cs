namespace App.ScopedService;

public interface IScopedProcessingService
{
    void DoWorkAsync(CancellationToken stoppingToken);
}