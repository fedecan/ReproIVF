FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ReproIVF.WebAPI/ReproIVF.WebAPI.csproj", "ReproIVF.WebAPI/"]
COPY ["ReproIVF.Shared/ReproIVF.Shared.csproj", "ReproIVF.Shared/"]
RUN dotnet restore "ReproIVF.WebAPI/ReproIVF.WebAPI.csproj"

COPY . .
WORKDIR /src/ReproIVF.WebAPI
RUN dotnet publish "ReproIVF.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ReproIVF.WebAPI.dll"]
