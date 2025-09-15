# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY *.sln .
COPY ["VemQueCabe.Api/*.csproj", "VemQueCabe.Api/"]
COPY ["VemQueCabe.Application/*.csproj", "VemQueCabe.Application/"]
COPY ["VemQueCabe.Domain/*.csproj", "VemQueCabe.Domain/"]
COPY ["VemQueCabe.Infra/*.csproj", "VemQueCabe.Infra/"]
RUN dotnet restore "VemQueCabe.Api/VemQueCabe.Api.csproj"
COPY . .
WORKDIR /app/VemQueCabe.Api

# Stage 2: Publish the application
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "VemQueCabe.Api.dll"]