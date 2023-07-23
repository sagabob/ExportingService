using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApiTests;

public class Helper
{
    public static ReportRequest GetFakeReportRequest()
    {
        var request = new ReportRequest
        {
            Title = "Sample Report",
            Product = ReportProduct.Profile,
            Urls = new[]
            {
                new()
                {
                    Url = "https://profile.id.com.au/adelaide/ancestry",
                    Title = "Ancestry"
                },
                new ReportUrl
                {
                    Url = "https://profile.id.com.au/adelaide/industries",
                    Title = "Industries"
                }
            }
        };

        return request;
    }
}