using FluentValidation;
using ReportExporting.Core;

namespace ReportExporting.ApplicationLib.Helpers.Core;

public class ExportRequestValidator : AbstractValidator<ReportRequest>
{
    public ExportRequestValidator()
    {
        RuleFor(p => p.Title).NotNull();

        RuleFor(p => p.EmailAddress).EmailAddress();

        RuleForEach(x => x.Urls).NotNull().SetValidator(new ReportUrlValidator());
    }
}

public class ReportUrlValidator : AbstractValidator<ReportUrl>
{
    public ReportUrlValidator()
    {
        RuleFor(x => x.Url).NotNull();
    }
}