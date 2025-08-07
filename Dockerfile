# -------- Set base image version --------
ARG PARENT_VERSION=latest

# -------- Globalization Setup Snippet --------
# Define common ENV and ICU install logic in ARGs so they can be reused
ARG DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ARG ICU_INSTALL="apt-get update && apt-get install -y --no-install-recommends libicu-dev && rm -rf /var/lib/apt/lists/*"


# ================================
# -------- Development Stage --------
# ================================
FROM defradigital/dotnetcore-development:$PARENT_VERSION AS development

LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

# Reuse common args
ARG DOTNET_SYSTEM_GLOBALIZATION_INVARIANT
ARG ICU_INSTALL
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=$DOTNET_SYSTEM_GLOBALIZATION_INVARIANT
RUN bash -c "$ICU_INSTALL"

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

LABEL uk.gov.defra.parent-image=defra-dotnetcore:${PARENT_VERSION}

# Reuse common args
ARG DOTNET_SYSTEM_GLOBALIZATION_INVARIANT
ARG ICU_INSTALL
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=$DOTNET_SYSTEM_GLOBALIZATION_INVARIANT
RUN bash -c "$ICU_INSTALL"

ARG PORT=8080
EXPOSE ${PORT}

USER dotnet
WORKDIR /home/dotnet/app
COPY --from=development /home/dotnet/out/ ./

ENTRYPOINT ["dotnet", "Apha.BST.Web.dll"]
