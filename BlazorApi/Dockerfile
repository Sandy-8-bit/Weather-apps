# Use the official ASP.NET runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081

# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["BlazorApi.csproj", "."]
RUN dotnet restore "BlazorApi.csproj"

# Copy the rest of the application files
COPY . .
WORKDIR "/src"

# Build the application
RUN dotnet build "BlazorApi.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "BlazorApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage: use the ASP.NET runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the entry point to run the application
CMD ["dotnet", "BlazorApi.dll"]
