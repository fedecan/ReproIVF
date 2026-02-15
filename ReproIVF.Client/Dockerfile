FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ReproIVF.Client/ReproIVF.Client.csproj", "ReproIVF.Client/"]
COPY ["ReproIVF.Shared/ReproIVF.Shared.csproj", "ReproIVF.Shared/"]
RUN dotnet restore "ReproIVF.Client/ReproIVF.Client.csproj"

COPY . .
WORKDIR /src/ReproIVF.Client
RUN dotnet publish "ReproIVF.Client.csproj" -c Release -o /app/publish

# In container deployments we route /api through nginx reverse proxy
# so the client can call the API with same-origin requests.
RUN sed -i 's#"ApiBaseUrl": ".*"#"ApiBaseUrl": "/"#g' /app/publish/wwwroot/appsettings.json

FROM nginx:1.27-alpine AS final
WORKDIR /usr/share/nginx/html

COPY ReproIVF.Client/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/publish/wwwroot .

EXPOSE 80
