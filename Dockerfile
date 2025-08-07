# -------- Set base image version --------
ARG PARENT_VERSION=latest

# ================================
# -------- Development Stage --------
# ================================
FROM defradigital/dotnetcore-development:$PARENT_VERSION AS development

# Label image metadata
LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

WORKDIR /home/dotnet/src

# Copy all source code into the image under /home/dotnet/src
COPY --chown=dotnet:dotnet src/. .

# Restore, build and publish the Web project
RUN dotnet restore ./Apha.BST/Apha.BST.Web/Apha.BST.Web.csproj
RUN dotnet publish ./Apha.BST/Apha.BST.Web -c Release -o /home/dotnet/out /p:UseAppHost=false

ARG PORT=8080
ENV PORT=${PORT}
EXPOSE ${PORT}

# ================================
# -------- Production Stage --------
# ================================
FROM defradigital/dotnetcore:$PARENT_VERSION AS production
ARG PARENT_VERSION=latest
LABEL uk.gov.defra.parent-image=defra-dotnetcore:${PARENT_VERSION}

ARG PORT=8080
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}

USER dotnet
WORKDIR /home/dotnet/app

COPY --from=development /home/dotnet/out/ ./

# Define entry point
ENTRYPOINT ["dotnet", "Apha.BST.Web.dll"]
