# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY ["BinDaddy.Backend.csproj", "./"]

# Restore dependencies
RUN dotnet restore "BinDaddy.Backend.csproj"

# Copy source code
COPY . .

# Build application
RUN dotnet build "BinDaddy.Backend.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BinDaddy.Backend.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Expose port
EXPOSE 5000

# Set environment
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Run application
ENTRYPOINT ["dotnet", "BinDaddy.Backend.dll"]
