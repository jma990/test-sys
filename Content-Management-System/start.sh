#!/bin/bash

# Install .NET SDK
curl -sSL https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh | bash /dev/stdin

# Add dotnet to path
export PATH=$PATH:/root/.dotnet

# Run the app
dotnet Content_Management_System.dll
