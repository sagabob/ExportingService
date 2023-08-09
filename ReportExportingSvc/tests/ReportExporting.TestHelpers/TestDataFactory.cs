using ReportExporting.Core;

namespace ReportExporting.TestHelpers;

public class TestDataFactory
{
    public static ReportRequest GetFakeReportRequest()
    {
        var request = new ReportRequest
        {
            Title = "Sample Report",
            Product = ReportProduct.Profile,
            EmailAddress = "bobpham.tdp@gmail.com",
            Urls = new[]
            {
                new()
                {
                    Url = "https://profile.id.com.au/albany/volunteering",
                    Title = "Volunteering"
                },
                new ReportUrl
                {
                    Url = "https://profile.id.com.au/albany/unpaid-childcare",
                    Title = "Unpaid Childcare"
                }
            }
        };

        return request;
    }

    public static ReportRequest GetFakeSingleReportRequest()
    {
        var request = new ReportRequest
        {
            Title = "Sample Report",
            Product = ReportProduct.Profile,
            EmailAddress = "bobpham.tdp@gmail.com",
            Urls = new[]
            {
                new ReportUrl
                {
                    Url = "https://profile.id.com.au/albany/unpaid-childcare",
                    Title = "Unpaid Childcare"
                }
            }
        };

        return request;
    }
}