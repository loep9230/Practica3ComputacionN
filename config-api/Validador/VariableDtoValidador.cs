using config_api.Dtos;
using FluentValidation;

namespace config_api.Validador
{
    public class VariableDtoValidador: AbstractValidator<VariableDto>
    {
        public VariableDtoValidador()
        {
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("El nombre de la variable es obligatorio.")
                .Matches("^[a-z0-9-]+$").WithMessage("El nombre de la variable contiene caracteres no válidos.");
            RuleFor(x => x.value)
                .NotEmpty().WithMessage("El valor de la variable es obligatorio.");
        }
    }
}
