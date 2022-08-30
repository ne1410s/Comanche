using DemoLibrary.Validation;
using FluentValidation;

namespace DemoLibrary.Tests.Validation
{
    public class TestModelValidator : FluentValidatorBase<TestModel>
    {
        protected override void DefineModelValidity()
        {
            RuleFor(m => m.Name)
                .MinimumLength(3)
                .WithMessage("Gimme a name");

            RuleFor(m => m.Magnitude)
                .GreaterThan(12)
                .WithMessage("Gimme some magnitude");
        }
    }
}
