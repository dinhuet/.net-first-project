# =====================
# Stage 1: Build
# =====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj vào container
COPY ["BlogApp.csproj", "./"]

# Restore
RUN dotnet restore "BlogApp.csproj"

# Copy toàn bộ source code
COPY . .

WORKDIR /src

# Build project
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "./BlogApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish project
FROM build AS publish
RUN dotnet publish "./BlogApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# =====================
# Stage 2: Runtime
# =====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose port HTTP + HTTPS
EXPOSE 5230
# EXPOSE 7184  

# Environment variables
ENV ASPNETCORE_URLS=http://+:5230
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Copy publish từ stage trước
COPY --from=publish /app/publish .

# Entrypoint
ENTRYPOINT ["dotnet", "BlogApp.dll"]
