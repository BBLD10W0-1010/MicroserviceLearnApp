FROM mcr.microsoft.com/dotnet/sdk:latest AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:latest
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "CommandService.dll"]