FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["password-usermanagement/password-usermanagement.csproj", "password-usermanagement/"]
RUN dotnet restore "password-usermanagement/password-usermanagement.csproj"
COPY . .
WORKDIR "/src/password-usermanagement"
RUN dotnet build "password-usermanagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "password-usermanagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "password-usermanagement.dll"]
