# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-dev

WORKDIR /app

# Copy project files
COPY ["Sudan_Train/Trains.Api.csproj", "Sudan_Train/"]
COPY ["Sudan_Train.Core/Trains.Core.csproj", "Sudan_Train.Core/"]
COPY ["Sudan_Train.Data/Trains.Data.csproj", "Sudan_Train.Data/"]
COPY ["Sudan_Train.Infrastructure/Trains.Infrastructure.csproj", "Sudan_Train.Infrastructure/"]
COPY ["Sudan_Train.Service/Trains.Service.csproj", "Sudan_Train.Service/"]

# Restore dependencies
RUN dotnet restore "Sudan_Train/Trains.Api.csproj"
RUN dotnet restore "Sudan_Train.Core/Trains.Core.csproj"
RUN dotnet restore "Sudan_Train.Data/Trains.Data.csproj"
RUN dotnet restore "Sudan_Train.Infrastructure/Trains.Infrastructure.csproj"
RUN dotnet restore "Sudan_Train.Service/Trains.Service.csproj"

# Copy everything else
COPY . ./

# Build and publish
RUN dotnet publish "Sudan_Train/Trains.Api.csproj" -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Expose ports
EXPOSE 80
EXPOSE 443

# Copy published output from build stage
COPY --from=build-dev /app/out .

# Set entry point
ENTRYPOINT [ "dotnet", "Trains.Api.dll" ]

