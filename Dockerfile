# -------- Build stage (SDK image with source) --------
ARG PARENT_VERSION=dotnet8.0
FROM defradigital/dotnetcore-development:$PARENT_VERSION AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy source files
COPY src/. .

# Restore dependencies
RUN dotnet restore Apha.BST.sln

# Build and publish the application
RUN dotnet publish Apha.BST/Apha.BST.Web/Apha.BST.Web.csproj \
    -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

# -------- Final runtime image (clean, smaller) --------
FROM defradigital/dotnetcore:$PARENT_VERSION AS final

# Set working directory for runtime
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set non-root user
USER app

# Define entry point
ENTRYPOINT ["dotnet", "Apha.BST.Web.dll"]
