#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Seach-Page-With-BlazorApp/Seach-Page-With-BlazorApp.csproj", "Seach-Page-With-BlazorApp/"]
RUN dotnet restore "Seach-Page-With-BlazorApp/Seach-Page-With-BlazorApp.csproj"
COPY . .
WORKDIR "/src/Seach-Page-With-BlazorApp"
RUN dotnet build "Seach-Page-With-BlazorApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seach-Page-With-BlazorApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seach-Page-With-BlazorApp.dll"]