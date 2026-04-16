# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore (speeds up builds if using caching)
COPY *.sln .
COPY *.csproj ./
RUN for file in $(ls *.csproj 2>/dev/null); do echo "found $file"; done || true

COPY . .
RUN dotnet publish JobBoard.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose a port (optional). Render provides $PORT at runtime.
EXPOSE 5000

# Use shell form so we can expand the PORT env var at runtime (with a default of 5000)
ENTRYPOINT ["sh","-c","exec dotnet JobBoard.dll --urls http://0.0.0.0:${PORT:-5000}"]
