# ====================================
# Stage 1: BUILD
# ====================================
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
# Esto se hace primero para aprovechar el caché de Docker
COPY ["config-api.csproj", "."]
RUN dotnet restore "./config-api/config-api.csproj"

# Copiar el resto del código fuente
COPY . ./

# Compilar y publicar la aplicación en modo Release
RUN dotnet publish -c Release -o /app/publish --no-restore

# ====================================
# Stage 2: RUNTIME
# ====================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

# Instalar PostgreSQL client para healthcheck y migraciones
RUN apk add --no-cache postgresql-client

WORKDIR /app

# Copiar los archivos publicados desde el stage de build
COPY --from=build /app/publish .

# Copiar el script de entrypoint
COPY entrypoint_script.sh /app/entrypoint_script.sh
RUN chmod +x /app/entrypoint_script.sh

# Exponer puertos HTTP y HTTPS
EXPOSE 5129 7258

# Configurar punto de entrada
ENTRYPOINT ["/app/entrypoint_script.sh"]
