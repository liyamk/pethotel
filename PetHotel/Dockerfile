#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# dotnet build "PetHotel.csproj" -c Release -o ./app/build
#  dotnet publish "PetHotel.csproj" -c Release -o ./app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY /app/publish .
ENTRYPOINT ["dotnet", "PetHotel.dll"]