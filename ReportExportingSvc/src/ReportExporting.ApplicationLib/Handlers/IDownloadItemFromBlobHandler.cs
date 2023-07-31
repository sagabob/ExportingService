using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Handlers;

public interface IDownloadItemFromBlobHandler
{
    Task<ReportRequestObject> Handle(Stream fileStream, ReportRequestObject request);
}