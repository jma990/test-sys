# ...existing code...
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the repo into the image (keeps things simple & avoids wrong paths)
COPY . .

# Find a .csproj, restore and publish it
# Note: if you have multiple projects, replace the detection with the exact relative path to your .csproj
RUN set -ex \
 && csproj=$(sh -c 'ls **/*.csproj 2>/dev/null | head -n 1') \
 && if [ -z "$csproj" ]; then echo "No .csproj found"; exit 1; fi \
 && echo "Using project: $csproj" \
 && dotnet restore "$csproj" \
 && dotnet publish "$csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

# Copy published output
COPY --from=build /app/publish .

# Start the first DLL found in /app (robust if project assembly name differs)
ENTRYPOINT ["/bin/bash","-lc","exec dotnet /app/$(ls /app/*.dll | head -n 1)"]
 