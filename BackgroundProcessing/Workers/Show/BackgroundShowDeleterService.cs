namespace BackgroundProcessing.Workers.Show;

public class BackgroundShowDeleterService : BackgroundService
{
    private readonly IShowProcessor _showProcessor;

    public BackgroundShowDeleterService(IShowProcessor showProcessor)
    {
        _showProcessor = showProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _showProcessor.DeleteShows(stoppingToken);
    }
}