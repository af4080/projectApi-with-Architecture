FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# העתקה ישירה כי הקובץ נמצא באותה תיקייה
COPY ["projectApiAngular.csproj", "./"]
RUN dotnet restore "projectApiAngular.csproj"

# העתקת כל שאר הקבצים
COPY . .
RUN dotnet build "projectApiAngular.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "projectApiAngular.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "projectApiAngular.dll"]