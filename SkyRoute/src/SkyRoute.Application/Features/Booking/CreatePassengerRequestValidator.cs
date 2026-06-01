using FluentValidation;

namespace SkyRoute.Application.Features.Booking;

public sealed class CreatePassengerRequestValidator : AbstractValidator<CreatePassengerRequest>
{
    public CreatePassengerRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.DocumentType)
            .IsInEnum()
                .WithMessage("Invalid document type.");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty();
    }
}
