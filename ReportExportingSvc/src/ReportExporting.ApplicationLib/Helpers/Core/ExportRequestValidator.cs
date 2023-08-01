using FluentValidation;
using ReportExporting.Core;
using System.Text.RegularExpressions;

namespace ReportExporting.ApplicationLib.Helpers.Core;

public class ExportRequestValidator : AbstractValidator<ReportRequest>
{
    public ExportRequestValidator()
    {
        RuleFor(p => p.Title).NotNull().NotEmpty();

        RuleFor(p => p.EmailAddress).NotNull().NotEmpty()
            .Matches(@"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$");

        RuleForEach(x => x.Urls).NotNull().SetValidator(new ReportUrlValidator());
    }
}

public class ReportUrlValidator : AbstractValidator<ReportUrl>
{
    public ReportUrlValidator()
    {
        RuleFor(x => x.Url).NotNull().NotEmpty();
    }
}