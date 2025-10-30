using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace config_domain
{
    public interface IRepositorio
    {
        Task<IEnumerable<Entornos>> ConsultarEntornos();
        Task<string> CrearEntorno(Entornos entorno);
        Task<Entornos?> ObtenerEntornoPorNombre(string nombre);
        Task ActualizarEntorno(Entornos entorno);
        Task BorrarEntorno(Entornos entorno);
        IEnumerable<Variables> ConsultarVariablesEntorno(string env_name);
        Task<string> CrearVariableEntorno(Variables variable);
        Task<Variables?> ObtenerVariablePorNombreEntorno(string nombreVariable, string nombreEntorno);
        Task ActualizarVariableEntorno(Variables variable);
        Task BorrarVariableEntorno(Variables variable);
    }
}
