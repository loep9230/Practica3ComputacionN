# =========================================
# Variables de entorno para Docker Compose
# =========================================

# Base de datos PostgreSQL
POSTGRES_DB=Practica3CN
POSTGRES_USER=postgres
POSTGRES_PASSWORD=1234

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Development

# Puertos externos
HTTP_PORT=8080
HTTPS_PORT=8081

# Certificados SSL (opcional para desarrollo)
CERT_PASSWORD=
CERT_PATH=
CERT_VOLUME=./certs

# Control de migraciones
SKIP_MIGRATIONS=false
