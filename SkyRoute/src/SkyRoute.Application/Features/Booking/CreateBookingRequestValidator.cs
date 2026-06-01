using FluentValidation;

namespace SkyRoute.Application.Features.Booking;

public sealed class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty();

        RuleFor(x => x.FlightNumber)
            .NotEmpty();

        RuleFor(x => x.Passengers)
            .NotEmpty()
                .WithMessage("At least one passenger is required.");

        RuleForEach(x => x.Passengers)
            .SetValidator(new CreatePassengerRequestValidator());
    }
}
