#!/bin/sh
set -e

cd "$(dirname "$0")/.."
pwd

API_BASE_URL="${API_BASE_URL:-https://localhost:7069/}"

curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh -c 8.0 -InstallDir ./dotnet

./dotnet/dotnet --version
./dotnet/dotnet publish ReproIVF.Client/ReproIVF.Client.csproj -c Release -o output

APPSETTINGS_FILE="output/wwwroot/appsettings.json"
if [ -f "$APPSETTINGS_FILE" ]; then
  sed -i "s#\"ApiBaseUrl\": \".*\"#\"ApiBaseUrl\": \"$API_BASE_URL\"#g" "$APPSETTINGS_FILE"
  echo "ApiBaseUrl configured to: $API_BASE_URL"
else
  echo "Warning: $APPSETTINGS_FILE not found, ApiBaseUrl was not updated."
fi
