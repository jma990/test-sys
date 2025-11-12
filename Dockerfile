FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore (speed up rebuilds)
COPY Content-Management-System/Content-Management-System.csproj ./Content-Management-System/
RUN dotnet restore Content-Management-System/Content-Management-System.csproj

# Copy the rest and publish
COPY . .
RUN dotnet publish Content-Management-System/Content-Management-System.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

# Copy published output from build stage
COPY --from=build /app/publish .

# Ensure Kestrel binds to Railway's PORT env (fallback to 80)
ENV ASPNETCORE_URLS="http://*:${PORT:-80}"

# Run the published DLL (WORKDIR is /app so use the DLL name)
ENTRYPOINT ["dotnet", "Content-Management-System.dll"]
