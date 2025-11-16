FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["Turnstile.csproj", "./"]
RUN dotnet restore "Turnstile.csproj"

# Copy everything else and publish
COPY . .
RUN dotnet publish "Turnstile.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Configure ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Optional: environment-based Turnstile config (can be overridden at run time)
# ENV CloudflareTurnstile__SiteKey=""
# ENV CloudflareTurnstile__SecretKey=""

ENTRYPOINT ["dotnet", "Turnstile.dll"]