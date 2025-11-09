using AutoMapper;
using config_api.Dtos;
using config_domain;
using config_infraestructura;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Linq;

namespace config_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigApi : ControllerBase
    {
        private IRepositorio _repositorio;
        private IMapper _mapper;
        private IValidator<EntornoDto> _validator;
        private IValidator<VariableDto> _variableValidator;
        public ConfigApi(IRepositorio repositorio,IMapper mapper, IValidator<EntornoDto> validator
        , IValidator<VariableDto> variableValidator)
        {
            _repositorio = repositorio;
            _mapper = mapper;
            _validator = validator;
            _variableValidator = variableValidator;
        }
        [HttpGet("/status")]
        public IActionResult Status()
        {
            return Ok("pong");
        }

        [HttpGet("/enviroments")]
        public async Task<ActionResult<object>> ObtenerEntornos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new { message = "Pagina debe ser mayor o igual 1 - numero de pagina entre 1 y 100" });
            }

            var todosLosEntornos = await _repositorio.ConsultarEntornos();
            var totalItems = todosLosEntornos.Count();
            var entornosPaginados = todosLosEntornos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (totalItems == 0)
            {
                return NoContent();
            }

            var resultado = new
            {
                page,
                pageSize,
                totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                data = entornosPaginados
            };
            return Ok(resultado);
        }

        [HttpPost("/enviroments")]
        public async Task<ActionResult<EntornoDto>> CrearEntorno(EntornoDto entornoDto)
        {
            var result = await _validator.ValidateAsync(entornoDto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            var entorno = new Entornos()
            {
                name = entornoDto.name,
                description = entornoDto.description
            };
            var nombre = await _repositorio.CrearEntorno(entorno);
            return CreatedAtAction(nameof(ObtenerEntornos), new { Id = nombre }, entorno);
        }

        [HttpGet("/enviroments/{env_name}")]
        public async Task<ActionResult<Entornos>> ObtenerEntorno(string env_name)
        {
            var entorno = await _repositorio.ObtenerEntornoPorNombre(env_name);
            if (entorno == null)
            {
                return NotFound(new { message = $"Entorno '{env_name}' no encontrado" });
            }
            return Ok(entorno);
        }

        [HttpPut("/enviroments/{env_name}")]
        public async Task<ActionResult> ActualizarEntorno(string env_name,ActualizarEntornoDto entornoDto)
        {
            var resultado = await _repositorio.ObtenerEntornoPorNombre(env_name);
            if (resultado == null)
            {
                return NotFound(new { message = $"Entorno no encontrado o no especificado correctamente" });
            }
            resultado.description = entornoDto.description;
            resultado.updated_at = DateTime.UtcNow;
            await _repositorio.ActualizarEntorno(resultado);
            return NoContent();
        }

        [HttpPatch("/enviroments/{env_name}")]
        public async Task<ActionResult> ActualizarParcialmenteEntorno(string env_name, ParcialEntornoDto entorno)
        {
            var resultado = await _repositorio.ObtenerEntornoPorNombre(env_name);
            if (resultado == null)
            {
                return NotFound(new { message = $"Entorno no encontrado o no especificado correctamente" });
            }
            _mapper.Map(entorno, resultado);
            resultado.updated_at = DateTime.UtcNow;
            await _repositorio.ActualizarEntorno(resultado);
            return Ok();
        }

        [HttpDelete("/enviroments/{env_name}")]
        public async Task<IActionResult> BorrarEntorno(string env_name)
        {
            var entorno = await _repositorio.ObtenerEntornoPorNombre(env_name);
            if (entorno == null)
            {
                return NotFound(new { message = $"Entorno '{env_name}' no encontrado" });
            }
            await _repositorio.BorrarEntorno(entorno);
            return NoContent();
        }


        [HttpGet("/enviroments/{env_name}/variables")]
        public async Task<ActionResult<object>> ObtenerVariablesEntorno(
        string env_name,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            // Verificar que el entorno existe
            var entorno = await _repositorio.ObtenerEntornoPorNombre(env_name);
            if (entorno == null)
            {
                return NotFound(new { message = $"Entorno '{env_name}' no encontrado" });
            }

            // Verificar si el entrono tiene variables
            var variables = _repositorio.ConsultarVariablesEntorno(env_name);
            if (variables.Count() == 0)
            {
                return NoContent();
            }

            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new { message = "Pagina debe ser mayor o igual 1 - numero de pagina entre 1 y 100" });
            }         

            var todasLasVariables = _repositorio.ConsultarVariablesEntorno(env_name).ToList();
            var totalItems = todasLasVariables.Count();

            var variablesPaginadas = todasLasVariables
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (totalItems == 0)
            {
                return NoContent();
            }

            var resultado = new
            {
                page,
                pageSize,
                totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                data = variablesPaginadas
            };

            return Ok(resultado);
        }

        [HttpPost("/enviroments/{env_name}/variables")]
        public async Task<ActionResult> CrearVariableEntorno(string env_name,VariableDto variableDto)
        {
            var result = await _variableValidator.ValidateAsync(variableDto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            var entorno = await _repositorio.ObtenerEntornoPorNombre(env_name);
            if (entorno == null)
            {
                return NotFound(new { message = $"Entorno '{env_name}' no encontrado" });
            }
            var variable = new Variables()
            {
                name = variableDto.name,
                description = variableDto.description,
                is_sensitive = variableDto.is_sensitive,
                value = variableDto.value,
                name_entorno = env_name
            };
            var nombre = await _repositorio.CrearVariableEntorno(variable);
            return CreatedAtAction(nameof(ObtenerVariableEntorno), new { env_name, var_name = nombre }, variable);
        }

        [HttpGet("/enviroments/{env_name}/variables/{var_name}")]
        public async Task<ActionResult<Variables>> ObtenerVariableEntorno(string env_name, string var_name)
        {
            var entorno = await _repositorio.ObtenerVariablePorNombreEntorno(var_name,env_name);
            if (entorno == null)
            {
                return NotFound(new { message = $"Entorno '{env_name}' no encontrado" });
            }
            return Ok(entorno);
        }

        [HttpPut("/enviroments/{env_name}/variables/{var_name}")]
        public async Task<IActionResult> ActualizarVariableEntorno(string env_name, string var_name, ActualizarVariableDto variableDto)
        {
            var resultado = await _repositorio.ObtenerVariablePorNombreEntorno(var_name,env_name);
            if (resultado == null)
            {
                return NotFound(new { message = $"Entorno o variable no encontrada o no especificada correctamente" });
            }
            resultado.description = variableDto.description;
            resultado.value = variableDto.value;
            resultado.is_sensitive = variableDto.is_sensitive;
            resultado.updated_at = DateTime.UtcNow;
            await _repositorio.ActualizarVariableEntorno(resultado);
            return NoContent();
        }

        [HttpPatch("/enviroments/{env_name}/variables/{var_name}")]
        public async Task<IActionResult> ActualizarParcialmenteVariable(string env_name, string var_name, ParcialVariableDto parcialVariableDto)
        {
            var resultado = await _repositorio.ObtenerVariablePorNombreEntorno(var_name, env_name);
            if (resultado == null)
            {
                return NotFound(new { message = $"Entorno o variable no encontrada o no especificada correctamente" });
            }
            _mapper.Map(parcialVariableDto, resultado);
            resultado.updated_at = DateTime.UtcNow;
            await _repositorio.ActualizarVariableEntorno(resultado);
            return NoContent();
        }

        [HttpDelete("/enviroments/{env_name}/variables/{var_name}")]
        public async Task<IActionResult> BorrarVariableEntorno(string env_name, string var_name)
        {
            var variable = await _repositorio.ObtenerVariablePorNombreEntorno(var_name,env_name);
            if (variable == null)
            {
                return NotFound(new { message = $"Entorno '{var_name}' no encontrado" });
            }
            await _repositorio.BorrarVariableEntorno(variable);
            return NoContent();
        }

        [HttpGet("/enviroments/{env_name}.json")]
        public async Task<IActionResult> ConsultarEntornoJson(string env_name)
        {
            var entorno = await _repositorio.ObtenerEntornoPorNombre(env_name);
            if (entorno == null)
            {
                return NotFound(new { message = $"Entorno '{env_name}' no encontrado" });
            }

            var variables = _repositorio.ConsultarVariablesEntorno(env_name);
            if (variables.Count() == 0)
            {
                return NoContent();
            }

            var dictionario = variables.ToDictionary(v => v.name,v => v.value);

            return Ok(dictionario);
    }
    }
}
