# ===== Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore (capa cacheable: solo el csproj)
COPY Abank.Api/Abank.Api.csproj Abank.Api/
RUN dotnet restore Abank.Api/Abank.Api.csproj

# Publish
COPY Abank.Api/ Abank.Api/
RUN dotnet publish Abank.Api/Abank.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===== Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Abank.Api.dll"]
