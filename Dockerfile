# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.21 AS base

# Set the working directory inside the container
WORKDIR /app

EXPOSE 8080
