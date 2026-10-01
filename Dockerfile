FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY global.json .
COPY src/SadaTaskManager.Domain/SadaTaskManager.Domain.csproj src/SadaTaskManager.Domain/
COPY src/SadaTaskManager.Application/SadaTaskManager.Application.csproj src/SadaTaskManager.Application/
COPY src/SadaTaskManager.Infrastructure/SadaTaskManager.Infrastructure.csproj src/SadaTaskManager.Infrastructure/
COPY src/SadaTaskManager.Api/SadaTaskManager.Api.csproj src/SadaTaskManager.Api/
RUN dotnet restore src/SadaTaskManager.Api/SadaTaskManager.Api.csproj

COPY src/ src/
RUN dotnet publish src/SadaTaskManager.Api/SadaTaskManager.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app ./
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "SadaTaskManager.Api.dll"]
