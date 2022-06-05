namespace BackgroundProcessing.Workers.Show;

public interface IShowProcessor
{
    Task DeleteShows(CancellationToken cancellationToken);
}