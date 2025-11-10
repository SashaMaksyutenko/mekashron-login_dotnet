# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore --no-cache
COPY . ./
RUN dotnet publish -c Release -o /app/out

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENV ASPNETCORE_URLS=http://0.0.0.0
ENTRYPOINT ["dotnet", "LoginApp.dll"]
