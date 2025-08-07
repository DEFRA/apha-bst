# -------- Set base image version --------
ARG PARENT_VERSION=latest

# ================================
# -------- Development Stage --------
# ================================
FROM defradigital/dotnetcore-development:$PARENT_VERSION AS development

# Label image metadata
LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

# Use a non-root user
USER dotnet
WORKDIR /home/dotnet

# Create project directory structure
RUN mkdir -p Apha.BST.Web

# Copy project file and restore dependencies
COPY --chown=dotnet:dotnet ./src/Apha.BST/Apha.BST.Web/*.csproj ./Apha.BST.Web/
RUN dotnet restore ./Apha.BST.Web/Apha.BST.Web.csproj

# Copy full source code for build
COPY --chown=dotnet:dotnet ./src/Apha.BST/Apha.BST.Web/ ./Apha.BST.Web/

# Build and publish
RUN dotnet publish ./Apha.BST.Web -c Release -o /home/dotnet/out /p:UseAppHost=false

# Set up environment and exposed port for dev
ARG PORT=8080
ENV PORT=${PORT}
EXPOSE ${PORT}

# Default command for development
CMD dotnet watch --project ./Apha.BST.Web run --urls http://*:${PORT}

# ================================
# -------- Production Stage --------
# ================================
FROM defradigital/dotnetcore:$PARENT_VERSION AS production

# Label image metadata
LABEL uk.gov.defra.parent-image=defra-dotnetcore:${PARENT_VERSION}

# Set environment for ASP.NET Core runtime
ARG PORT=8080
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}

# Use non-root user and set working dir
USER dotnet
WORKDIR /home/dotnet/app

# Copy published output from dev stage
COPY --from=development /home/dotnet/out/ ./

# Default entrypoint
CMD ["dotnet", "Apha.BST.Web.dll"]
