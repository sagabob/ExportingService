using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Handlers;

public interface IUploadItemToBlobHandler
{
    Task<ReportRequestObject> Handle(Stream fileStream, ReportRequestObject request);
}