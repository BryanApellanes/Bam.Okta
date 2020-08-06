#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Bam.Okta.RazorPages/Bam.Okta.RazorPages.csproj", "Bam.Okta.RazorPages/"]
RUN dotnet restore "Bam.Okta.RazorPages/Bam.Okta.RazorPages.csproj"
COPY . .
WORKDIR "/src/Bam.Okta.RazorPages"
RUN dotnet build "Bam.Okta.RazorPages.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bam.Okta.RazorPages.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bam.Okta.RazorPages.dll"]