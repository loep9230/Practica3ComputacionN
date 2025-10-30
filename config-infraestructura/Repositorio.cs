using config_domain;
using Microsoft.EntityFrameworkCore;

namespace config_infraestructura
{
    public class Repositorio: IRepositorio
    {
        private Contexto _context;
        public Repositorio(Contexto context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Entornos>> ConsultarEntornos()
        {
            return await _context.Entornos.ToListAsync();
        }
        public async Task<string> CrearEntorno(Entornos entorno)
        {
            await _context.Entornos.AddAsync(entorno);
            await  _context.SaveChangesAsync();
            return entorno.name;
        }
        public async Task<Entornos?> ObtenerEntornoPorNombre(string nombre)
        {
            return await _context.Entornos.FindAsync(nombre);
        }
        public async Task ActualizarEntorno(Entornos entorno)
        {
            _context.Entornos.Update(entorno);
            await _context.SaveChangesAsync();
        }
        public async Task BorrarEntorno(Entornos entorno)
        {
            _context.Entornos.Remove(entorno);
            await _context.SaveChangesAsync();
        }
        public IEnumerable<Variables> ConsultarVariablesEntorno(string env_name)
        {
            return _context.Variables.Where(v => v.name_entorno.Equals(env_name)).ToList();
        }
        public async Task<string> CrearVariableEntorno(Variables variable)
        {
            await _context.Variables.AddAsync(variable);
            await _context.SaveChangesAsync();
            return variable.name;
        }
        public async Task<Variables?> ObtenerVariablePorNombreEntorno(string nombreVariable, string nombreEntorno)
        {
            return await _context.Variables.FindAsync(new[] { nombreVariable,nombreEntorno });
        }
        public async Task ActualizarVariableEntorno(Variables variable)
        {
            _context.Variables.Update(variable);
            await _context.SaveChangesAsync();
        }
        public async Task BorrarVariableEntorno(Variables variable)
        {
            _context.Variables.Remove(variable);
            await _context.SaveChangesAsync();
        }
    }
}
