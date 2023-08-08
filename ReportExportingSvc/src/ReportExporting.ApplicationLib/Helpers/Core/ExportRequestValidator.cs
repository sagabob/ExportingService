using FluentValidation;
using ReportExporting.Core;

namespace ReportExporting.ApplicationLib.Helpers.Core;

public class ExportRequestValidator : AbstractValidator<ReportRequest>
{
    public ExportRequestValidator()
    {
        RuleFor(p => p.Title).NotNull().NotEmpty();

        RuleFor(p => p.EmailAddress).NotNull().NotEmpty()
            .Matches(@"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$");

        RuleFor(p => p.Urls).NotNull();

        RuleForEach(x => x.Urls).NotNull().SetValidator(new ReportUrlValidator());

        RuleFor(p => p.Product).NotNull().IsInEnum();

        RuleFor(p => p.Format).NotNull().IsInEnum();
    }
}