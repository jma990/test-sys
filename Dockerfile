FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

# Copy published output
COPY --from=build /app/publish .

# Ensure Kestrel binds to the container port (uses PORT env if provided by Railway)
ENV ASPNETCORE_URLS="http://*:${PORT:-80}"

# Start the first DLL found in /app (avoid doubling /app in the path)
ENTRYPOINT ["/bin/bash","-lc","export ASPNETCORE_URLS=\"http://*:${PORT:-80}\" && exec dotnet \"$(ls /app/*.dll | head -n 1)\""]