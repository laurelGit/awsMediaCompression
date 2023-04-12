# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source
COPY . .

RUN dotnet restore "Aws.Media.Convert.Api.csproj" --disable-parallel
RUN dotnet publish "Aws.Media.Convert.Api.csproj" -c Release -o /app/publish --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app
COPY --from=build /app/publish /app
EXPOSE 5167
ENTRYPOINT ["dotnet", "/app/Aws.Media.Convert.Api.dll"]