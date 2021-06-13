# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Conduit/*.csproj ./
RUN dotnet restore

RUN dotnet tool install -g dotnet-ef
ENV PATH $PATH:/root/.dotnet/tools

# Copy everything else and build
COPY . ./

# RUN chmod +x ./entrypoint.sh
# RUN ["/bin/bash", "./entrypoint.sh"]
RUN dotnet publish asp-dotnet-realworld-conduit.sln -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS=http://*:80

ENTRYPOINT ["dotnet", "Conduit.dll"]
