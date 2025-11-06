# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj file and restore the dependencies
COPY /Content_Management_System/Content_Management_System.csproj .
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Publish the app to the /app/publish directory
RUN dotnet publish -c Release -o /app/publish

# Use the .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

# Set the working directory in the container
WORKDIR /app

# Copy the app from the build stage
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "Content_Management_System.dll"]
