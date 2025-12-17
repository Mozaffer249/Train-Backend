# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["Sudan_Train.MessagingApi/Sudan_Train.MessagingApi.csproj", "Sudan_Train.MessagingApi/"]
RUN dotnet restore "Sudan_Train.MessagingApi/Sudan_Train.MessagingApi.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Sudan_Train.MessagingApi"
RUN dotnet build "Sudan_Train.MessagingApi.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Sudan_Train.MessagingApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sudan_Train.MessagingApi.dll"]
