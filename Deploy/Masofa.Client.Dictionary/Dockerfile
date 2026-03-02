FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY . .

RUN dotnet publish  "./Masofa.Client.Dictionary/Masofa.Client.Dictionary.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /var/www/web
COPY /Deploy/Masofa.Client.Dictionary/nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/publish/wwwroot .

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]