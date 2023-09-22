#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ReportExporting.NotificationApi/ReportExporting.NotificationApi.csproj", "src/ReportExporting.NotificationApi/"]
COPY ["src/ReportExporting.ApplicationLib/ReportExporting.ApplicationLib.csproj", "src/ReportExporting.ApplicationLib/"]
COPY ["src/ReportExporting.Core/ReportExporting.Core.csproj", "src/ReportExporting.Core/"]
RUN dotnet restore "src/ReportExporting.NotificationApi/ReportExporting.NotificationApi.csproj"
COPY . .
WORKDIR "/src/src/ReportExporting.NotificationApi"
RUN dotnet build "ReportExporting.NotificationApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReportExporting.NotificationApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportExporting.NotificationApi.dll"]