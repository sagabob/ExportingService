using FluentValidation;
using ReportExporting.Core;

namespace ReportExporting.ApplicationLib.Helpers.Core;

public class ReportUrlValidator : AbstractValidator<ReportUrl>
{
    public ReportUrlValidator()
    {
        RuleFor(x => x.Url).NotNull().NotEmpty().Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute));
    }
}