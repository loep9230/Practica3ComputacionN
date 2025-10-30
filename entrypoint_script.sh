#!/bin/sh
set -e

echo "==================================="
echo "Config-API Entrypoint Script"
echo "==================================="

# Esperar a que PostgreSQL esté listo
echo "Esperando a que PostgreSQL esté disponible..."
until pg_isready -h postgres-db -U ${POSTGRES_USER:-postgres} > /dev/null 2>&1; do
  echo "PostgreSQL no está listo - esperando..."
  sleep 2
done

echo "PostgreSQL está listo!"

# Ejecutar migraciones de Entity Framework
if [ "$SKIP_MIGRATIONS" != "true" ]; then
  echo "Ejecutando migraciones de base de datos..."
  dotnet ef database update --no-build || {
    echo "Error: Las migraciones fallaron. Verifica la conexión a la base de datos."
    exit 1
  }
  echo "Migraciones completadas exitosamente."
else
  echo "Migraciones omitidas (SKIP_MIGRATIONS=true)"
fi

echo "==================================="
echo "Iniciando aplicación Config-API..."
echo "==================================="

# Iniciar la aplicación
exec dotnet config-api.dll
