# -------- Base runtime image --------
FROM defradigital/dotnetcore-development AS base
WORKDIR /app
EXPOSE 8080
USER app

# -------- Build image with SDK --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files for caching
# COPY src/Apha.BST.sln ./
# COPY src/Apha.BST/Apha.BST.Web/Apha.BST.Web.csproj Apha.BST/Apha.BST.Web/
# COPY src/Apha.BST/Apha.BST.Core/Apha.BST.Core.csproj Apha.BST/Apha.BST.Core/
# COPY src/Apha.BST/Apha.BST.Application/Apha.BST.Application.csproj Apha.BST/Apha.BST.Application/
# COPY src/Apha.BST/Apha.BST.DataAccess/Apha.BST.DataAccess.csproj Apha.BST/Apha.BST.DataAccess/
# (skip UnitTests for now unless you're testing in Docker)

# Copy full source
#COPY src/. .
COPY . .

# Restore dependencies
RUN dotnet restore Apha.BST.sln



# Build
RUN dotnet build Apha.BST/Apha.BST.Web/Apha.BST.Web.csproj \
    -c $BUILD_CONFIGURATION -o /app/build



# -------- Publish stage --------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Apha.BST/Apha.BST.Web/Apha.BST.Web.csproj \
    -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------- Final runtime image --------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Apha.BST.Web.dll"]
