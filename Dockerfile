# -------- Base runtime image --------
FROM defradigital/dotnetcore-development AS base
WORKDIR /app
EXPOSE 8080
USER app

# -------- Build image with SDK --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR src

# Copy only solution and project files first for better build caching
COPY ./Apha.BST.sln ./
COPY ./Apha.BST/Apha.BST.Web/Apha.BST.Web.csproj Apha.BST/Apha.BST.Web/
# Repeat for any other referenced projects if needed:
# COPY ./Apha.BST/Apha.BST.Core/Apha.BST.Core.csproj Apha.BST/Apha.BST.Core/

# Restore dependencies
RUN dotnet restore "Apha.BST.sln"

# Copy remaining source files
COPY ./Apha.BST/ Apha.BST/

# Build the application
RUN dotnet build "./Apha.BST/Apha.BST.Web/Apha.BST.Web.csproj" \
    -c $BUILD_CONFIGURATION -o /app/build

# -------- Publish image --------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Apha.BST/Apha.BST.Web/Apha.BST.Web.csproj" \
    -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------- Final runtime image --------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Apha.BST.Web.dll"]
