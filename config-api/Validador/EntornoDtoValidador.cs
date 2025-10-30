using config_api.Dtos;
using FluentValidation;

namespace config_api.Validador
{
    public class EntornoDtoValidador: AbstractValidator<EntornoDto>
    {
        public EntornoDtoValidador()
        {
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("El nombre del entorno es obligatorio.")
                .Matches("^[a-z0-9-]+$");
        }
    }
}
