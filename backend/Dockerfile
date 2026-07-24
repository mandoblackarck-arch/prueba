ARG SERVICE
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG SERVICE
WORKDIR /src
COPY . .
RUN dotnet restore "src/${SERVICE}/${SERVICE}.csproj" && dotnet publish "src/${SERVICE}/${SERVICE}.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ARG SERVICE
ENV SERVICE=${SERVICE}
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/src/${SERVICE}/appsettings.json /app/appsettings.json
ENTRYPOINT ["sh", "-c", "dotnet ${SERVICE}.dll"]
