FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN curl -sL https://deb.nodesource.com/setup_13.x |  bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["RoomTemp/RoomTemp.csproj", "RoomTemp/"]
COPY ["RoomTemp.Data/RoomTemp.Data.csproj", "RoomTemp.Data/"]
RUN dotnet restore "RoomTemp/RoomTemp.csproj"
COPY . .
WORKDIR "/src/RoomTemp"
RUN dotnet build "RoomTemp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RoomTemp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoomTemp.dll"]