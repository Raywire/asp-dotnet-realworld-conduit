# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0
COPY . /app
WORKDIR /app
RUN dotnet tool install -g dotnet-ef
ENV PATH $PATH:/root/.dotnet/tools
RUN dotnet ef --version
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 80/tcp
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh
