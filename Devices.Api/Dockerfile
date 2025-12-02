# ---------------------------
# Build stage
# ---------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia todos os projetos da solução
COPY . .

# Move para a pasta do projeto API
WORKDIR /src/Devices.Api

# Restaura dependências
RUN dotnet restore "Devices.Api.csproj"

# Compila e publica para Release
RUN dotnet publish "Devices.Api.csproj" -c Release -o /app/publish

# ---------------------------
# Runtime stage
# ---------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copia os binários publicados
COPY --from=build /app/publish .

# Expõe porta HTTP
EXPOSE 80

# Entry point
ENTRYPOINT ["dotnet", "Devices.Api.dll"]
