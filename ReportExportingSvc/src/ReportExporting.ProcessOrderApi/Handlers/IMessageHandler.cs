namespace ReportExporting.ProcessOrderApi.Handlers
{
    public interface IMessageHandler
    {
        Task Register();
        Task CloseQueueAsync();
        ValueTask DisposeAsync();
    }
}
