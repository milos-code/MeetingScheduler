FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080 
ENV ASPNETCORE_URLS=http://*:8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MeetingScheduler.Api/MeetingScheduler.Api.csproj", "MeetingScheduler.Api/"]
COPY ["MeetingScheduler.Bussines/MeetingScheduler.Bussines.csproj", "MeetingScheduler.Bussines/"]
COPY ["MeetingScheduler.Infrastructure/MeetingScheduler.Infrastructure.csproj", "MeetingScheduler.Infrastructure/"]
RUN dotnet restore "./MeetingScheduler.Api/MeetingScheduler.Api.csproj"
COPY . .
WORKDIR "/src/MeetingScheduler.Api"
RUN dotnet build "./MeetingScheduler.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./MeetingScheduler.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeetingScheduler.Api.dll"]