#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DummyAdminPanel/DummyAdminPanel.csproj", "DummyAdminPanel/"]
RUN dotnet restore "DummyAdminPanel/DummyAdminPanel.csproj"
COPY . .
WORKDIR "/src/DummyAdminPanel"
RUN dotnet build "DummyAdminPanel.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DummyAdminPanel.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DummyAdminPanel.dll"]