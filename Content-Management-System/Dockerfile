# Use official .NET Core SDK image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Content_Management_System/Content_Management_System.csproj", "Content_Management_System/"]
RUN dotnet restore "Content_Management_System/Content_Management_System.csproj"
COPY . .
WORKDIR "/src/Content_Management_System"
RUN dotnet build "Content_Management_System.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Content_Management_System.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY wwwroot /app/wwwroot  # Add this line to ensure static files are copied correctly
ENTRYPOINT ["dotnet", "Content_Management_System.dll"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Content_Management_System.dll"]
