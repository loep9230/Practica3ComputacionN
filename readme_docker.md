# Despliegue de config-api con Docker

Este documento describe cómo desplegar la aplicación **config-api** con PostgreSQL utilizando Docker y Docker Compose.

---

## 📋 Requisitos Previos

- **Docker Engine**: v20.10 o superior
- **Docker Compose**: v2.0 o superior
- **Sistema Operativo**: Linux (Debian/Ubuntu) o compatible

Verificar instalación:
```bash
docker --version
docker-compose --version
```

---

## 📁 Estructura de Archivos

```
config-api/
├── Dockerfile                    # Imagen multistage de .NET 8.0
├── docker-compose.yml            # Orquestación de servicios
├── entrypoint.sh                 # Script de inicialización
├── .dockerignore                 # Archivos excluidos del contexto
├── .env                          # Variables de entorno (no versionar)
├── appsettings.json              # Configuración base
├── appsettings.Docker.json       # Configuración para Docker
└── [resto del proyecto .NET]
```

---

## 🚀 Inicio Rápido

### 1. Clonar o posicionarse en el directorio del proyecto

```bash
cd /ruta/a/config-api
```

### 2. Configurar variables de entorno (opcional)

Editar el archivo `.env` si necesitas cambiar valores por defecto:

```bash
# .env
POSTGRES_DB=Practica3CN
POSTGRES_USER=postgres
POSTGRES_PASSWORD=1234
HTTP_PORT=8080
HTTPS_PORT=8081
```

### 3. Construir y levantar los contenedores

```bash
docker-compose up --build
```

O en modo detached (segundo plano):

```bash
docker-compose up --build -d
```

### 4. Verificar el estado de los servicios

```bash
docker-compose ps
```

Salida esperada:
```
NAME                  STATUS              PORTS
config-api-app        Up (healthy)        0.0.0.0:8080->5129/tcp, 0.0.0.0:8081->7258/tcp
config-api-postgres   Up (healthy)        0.0.0.0:5432->5432/tcp
```

---

## 🔍 Verificación del Despliegue

### Health Check del servicio

```bash
curl http://localhost:8080/status
```

Respuesta esperada: `"pong"`

### Acceder a Swagger UI

- **HTTP**: http://localhost:8080/swagger
- **HTTPS**: https://localhost:8081/swagger

### Verificar logs

```bash
# Logs de todos los servicios
docker-compose logs -f

# Solo logs de la aplicación
docker-compose logs -f config-api

# Solo logs de PostgreSQL
docker-compose logs -f postgres-db
```

---

## 🗃️ Gestión de la Base de Datos

### Ejecutar migraciones manualmente

Las migraciones se ejecutan automáticamente al iniciar el contenedor. Para ejecutarlas manualmente:

```bash
docker-compose exec config-api dotnet ef database update
```

### Conectarse a PostgreSQL

```bash
docker-compose exec postgres-db psql -U postgres -d Practica3CN
```

Comandos útiles en psql:
```sql
\dt              -- Listar tablas
\d+ Entornos     -- Describir tabla Entornos
\d+ Variables    -- Describir tabla Variables
SELECT * FROM "Entornos";
```

### Crear backup de la base de datos

```bash
docker-compose exec postgres-db pg_dump -U postgres Practica3CN > backup.sql
```

### Restaurar backup

```bash
docker-compose exec -T postgres-db psql -U postgres Practica3CN < backup.sql
```

---

## 🛠️ Comandos Útiles

### Detener los servicios

```bash
docker-compose down
```

### Detener y eliminar volúmenes (¡CUIDADO! Elimina datos)

```bash
docker-compose down -v
```

### Reconstruir sin usar caché

```bash
docker-compose build --no-cache
docker-compose up
```

### Ver uso de recursos

```bash
docker stats
```

### Acceder al contenedor de la aplicación

```bash
docker-compose exec config-api sh
```

### Reiniciar un servicio específico

```bash
docker-compose restart config-api
```

---

## 🔧 Configuración Avanzada

### Cambiar puertos externos

Editar `.env`:
```bash
HTTP_PORT=9090
HTTPS_PORT=9091
```

Y reiniciar:
```bash
docker-compose up -d
```

### Omitir migraciones al iniciar

```bash
SKIP_MIGRATIONS=true docker-compose up
```

O configurar en `.env`:
```bash
SKIP_MIGRATIONS=true
```

### Usar certificado SSL personalizado

1. Colocar certificado en `./certs/certificate.pfx`
2. Configurar en `.env`:
```bash
CERT_PATH=/https/certificate.pfx
CERT_PASSWORD=tu_password_aqui
```

3. Reiniciar:
```bash
docker-compose up -d
```

---

## 🐛 Troubleshooting

### La aplicación no se conecta a PostgreSQL

**Síntoma**: Error "could not connect to server"

**Solución**:
1. Verificar que PostgreSQL esté healthy:
```bash
docker-compose ps postgres-db
```

2. Revisar logs:
```bash
docker-compose logs postgres-db
```

3. Verificar cadena de conexión en `appsettings.Docker.json`

### Migraciones fallan

**Síntoma**: "An error occurred using the connection to database"

**Solución**:
1. Verificar credenciales en `.env`
2. Asegurarse que la base de datos existe:
```bash
docker-compose exec postgres-db psql -U postgres -c "\l"
```

3. Ejecutar migraciones manualmente:
```bash
docker-compose exec config-api dotnet ef database update --verbose
```

### Puerto ya en uso

**Síntoma**: "Bind for 0.0.0.0:8080 failed: port is already allocated"

**Solución**:
1. Cambiar puertos en `.env`
2. O detener el proceso que usa el puerto:
```bash
# Linux
sudo lsof -i :8080
sudo kill -9 <PID>
```

### Contenedor se reinicia constantemente

**Solución**:
1. Ver logs detallados:
```bash
docker-compose logs --tail=100 config-api
```

2. Verificar healthcheck:
```bash
docker inspect config-api-app | grep -A 10 Health
```

---

## 📊 Monitoreo en Producción

### Ver estado de salud

```bash
# Health check de la aplicación
curl http://localhost:8080/status

# Health check de PostgreSQL
docker-compose exec postgres-db pg_isready -U postgres
```

### Configurar restart policy para producción

En `docker-compose.yml`, ya está configurado `restart: always` para ambos servicios.

---

## 🔒 Consideraciones de Seguridad

### Para Producción:

1. **Cambiar contraseñas por defecto**:
   - Editar `.env` con credenciales seguras
   - Usar variables de entorno del sistema en lugar de archivo `.env`

2. **No exponer puerto de PostgreSQL**:
   - Comentar en `docker-compose.yml`:
   ```yaml
   # ports:
   #   - "5432:5432"
   ```

3. **Usar HTTPS con certificado válido**:
   - Generar certificado con Let's Encrypt o similar
   - Configurar en Kestrel

4. **Proteger el archivo `.env`**:
   ```bash
   chmod 600 .env
   ```

5. **Usar secrets de Docker** (Docker Swarm):
   ```bash
   echo "mi_password_seguro" | docker secret create postgres_password -
   ```

---

## 📝 Notas Adicionales

- **Volumen persistente**: Los datos de PostgreSQL se guardan en el volumen `config-api-postgres-data`
- **Red interna**: Los servicios se comunican por la red `config-api-network`
- **Migraciones automáticas**: Se ejecutan en el `entrypoint.sh` antes de iniciar la aplicación
- **Healthchecks**: Ambos servicios tienen healthchecks configurados para garantizar disponibilidad

---

## 📞 Soporte

Si encuentras problemas:
1. Revisa los logs: `docker-compose logs -f`
2. Verifica el estado: `docker-compose ps`
3. Consulta la documentación de [.NET en Docker](https://docs.microsoft.com/dotnet/core/docker/)
4. Consulta la documentación de [PostgreSQL en Docker](https://hub.docker.com/_/postgres)

---

## 🎯 Comandos de Producción

```bash
# Producción: Usar appsettings.Production.json
ASPNETCORE_ENVIRONMENT=Production docker-compose up -d

# Ver logs sin timestamps (más limpio)
docker-compose logs --no-log-prefix

# Escalar horizontalmente (múltiples instancias)
docker-compose up -d --scale config-api=3
```

---

**Versión**: 1.0  
**Última actualización**: Octubre 2025
