# Project Masofa

## Deploy

### Общее

1. Если нет скриптов для применения миграций, вам понадобяться SQL скрипты для применения миграций, для генерации скрипта используйте команду 

```ps1
dotnet ef migrations script  --idempotent  --context <ИмяКонтекста> --output Migrations/<ИмяМодуля>/apply_all_migrations.sql 
```

2. Проверьте скрипт, в нём должны быть секции, которые применяют миграции, если не записи в таблице миграции, наприммер:
```sql
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250715124740_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;
```

3. Если баз данных нет, то создайте базы-данных
3. 
4. Примините полученные скрипты на ваши базы данных ***(если у вас база лежит в докер контейнере, то сначала поднимите контейнер pgsql)***
```ps1
psql -U your_username -d your_database_name -f /path/to/script.sql
```

### Development(local)

1. Создайте копию файла ./.env_example в ./.env
2. Заполните переменные среды в ./.env
3. Создайте копию файла appsettings.Example.json в appsettings.Development.json
4. Обновите строки подключения к БД в файле ./Masofa.Web.Monolith/appsettings.Development.json. *Для запуска в докере адрес сервера должен быть именем контейнера*
5. Обновите конфигурацию подключения к MinIO - подключение к контейнеру minio смотри файл `docker-compose.yml`
6. Обновите конфигурацию Nlog
```json
  "NLog": {
    "throwConfigExceptions": true,
    "targets": {
      "async": true,
      "logfile": {
        "type": "File",
        "fileName": "${shortdate}_${date:format=HH_mm}.log"
      },
      "logconsole": {
        "type": "ColoredConsole"
      }
    },
    "rules": [
      {
        "logger": "*",
        "minLevel": "Info",
        "writeTo": "logconsole"
      },
      {
        "logger": "*",
        "minLevel": "Debug",
        "writeTo": "logfile"
      }
    ]
  },
```
7. Выполните команду для сборки контейнеров
```ps1
docker-compose build --pull
```
8. Выполните команду для поднятие контейнеров
```ps1
docker-compose up -d
```


### Test - только для тествого сервера

1. Создайте копию файла ./.env_example в ./.env
2. Заполните переменные среды в ./.env
3. Создайте копию файла appsettings.Example.json в appsettings.Testing.json
4. Обновите строки подключения к БД в файле ./Masofa.Web.Monolith/appsettings.Testing.json. *Для запуска в докере адрес сервера должен быть именем контейнера*
5. Обновите конфигурацию подключения к MinIO:
```json
"MinIO": {
    "Endpoint": "http://minio.example",
    "AccessKey":"UserLogin",
    "SecretKey":"UserPass",
    "Secure": false
}
```
6. Обновите конфигурацию Nlog
```json
  "NLog": {
    "throwConfigExceptions": true,
    "targets": {
      "async": true,
      "logfile": {
        "type": "File",
        "fileName": "/app/logs/${shortdate}/${date:format=HH_mm}.log"
      }
    },
    "rules": [
      {
        "logger": "*",
        "minLevel": "Info",
        "writeTo": "logconsole"
      },
      {
        "logger": "*",
        "minLevel": "Debug",
        "writeTo": "logfile"
      }
    ]
  },
```

7. Выполните команду для сборки контейнеров
```bash
docker-compose build --pull
```
8. Выполните команду для поднятие контейнеров
```bash
docker-compose up -d
```

### Prod

1. Создайте копию файла appsettings.Example.json в appsettings.Production.json
2. Обновите строки подключения к БД в файле ./Masofa.Web.Monolith/appsettings.Production.json - *БД поднята в standalone pgsql на продакшион сервер. Исключение wearher она поднята на Сервере 3*
3. Обновите конфигурацию подключения к MinIO:
```json
"MinIO": {
    "Endpoint": "http://minio.example",
    "AccessKey":"UserLogin",
    "SecretKey":"UserPass",
    "Secure": false
}
```
4. Обновите конфигурацию Nlog
```json
  "NLog": {
    "throwConfigExceptions": true,
    "targets": {
      "async": true,
      "logfile": {
        "type": "File",
        "fileName": "/deploy/logs/${shortdate}/${date:format=HH_mm}.log"
      }
    },
    "rules": [
      {
        "logger": "*",
        "minLevel": "Info",
        "writeTo": "logconsole"
      },
      {
        "logger": "*",
        "minLevel": "Debug",
        "writeTo": "logfile"
      }
    ]
  }
```
5. Выполните публикацию проекта *выпонять внутри папки Masofa.Web.Monolith*
```bash
sudo dotnet publish -c Release -o /path-to-publish/masofa-web-monolith
```
6. Установите на папку и файлы в ней права на чтение и исполения файлоы
```bash
cd /path-to-publish
sudo chmod -R 644 ./masofa-web-monolith
```
7. Создайте файл /systemd/system/masofa-web-monolith.service , **если этого файла нет**, с таким содержимом
```service
[Unit]
Description=Masofa Monolith WebAPI Service
After=network.target

[Service]
WorkingDirectory=/path-to-publish/masofa-web-monolith
ExecStart=/path-to-publish/masofa-web-monolith/Masofa.Web.Monolith
Restart=always

# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=masofa-web-monolith
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

8. Перезапустите демон
```bash
sudo systemctl daemon-reload
```
9. Перезапустите сервис
```bash
sudo systemctl restart masofa-web-monolith
```

10. Проверьте статус сервиса
```bash
sudo systemctl status masofa-web-monolith
```

## Simple Start

### Общее 
Этот способ для поднятия проекта в режиме отладки. Проект будет ориентирован на предпрод. На нём находяться данные копия с прода. 

1. Добавить в корень файл .env с таким содержимом. **ВАЖНО** прописать адреса для переменных 
    1. DEPLOY_ROOT_FOLDER - это коневая папка на вашей машине, куда будут складываться файлы: логов, загрузок и так далее
    1. GEOSERVER_TILES_DIR - это коневая папка на вашей машине, куда будут складываться файлы тайлов карты, которые генерирует бекенд

```env
#shared
MODE=local
ASPNETCORE_ENVIRONMENT=Development
DEPLOY_ROOT_FOLDER=.


#postgres
POSTGRES_PORT=20010
POSTGRES_PASSWORD=AllRight73!
POSTGRES_USER=viki

#Masofa.Web.Monolith
MONOLITH_HTTP_PORT=20020
MONOLITH_HTTPS_PORT=20021
MONOLITH_HTTP2_PORT=20022

#Masofa.Client.Dict
DICT_HTTP_PORT=20030

#minio
MINIO_HTTP_PORT=20040
MINIO_CLI_PORT=20041
MINIO_USER=sixgrain
MINIO_PASSWORD=AllRight73!

#geoserver
GEOSERVER_HTTP_PORT=20060
GEOSERVER_ADMIN_USER=sixgrain
GEOSERVER_ADMIN_PASSWORD=AllRight73!
GEOSERVER_TILES_DIR=D:\Debug\masofa\weatherLayers

#ftp
FTP_PORT_20=20070
FTP_PORT_21=20071
FTP_USER=sixgrainftp
FTP_PASS=MnOpQrStUvWx
FTP_FILES_DIR=/root/Debug/liban
PASV_ADDRESS=185.100.234.107
PASV_MIN_PORT=21000
PASV_MAX_PORT=21010
```

2. Добавить в папку ./Masofa.Web.Monolith файл appsettings.Production.json

```json
//⚠️После изменения конфигурации нужно перезапустить бекенд
{
  "MonolithConfiguration": { //конфигурация запуска модулей, каждый флаг true запускает отдельный модуль. 
    "StartIdentity": true, //Контроль доступа пользователй
    "StartDictionaries": true, //Модуль словарей
    "StartCropMonitoring": true, //Модуль Мониторинг полей
    "StartSatelliteLandsat": false, //Модуль работы с Landsat
    "StartSatelliteSentinel": false, //Модуль работы с Sentinel
    "StartTiles": true, //Подсистема GIS
    "StartEra": true, //Модуль работы с ERA5
    "StartIBM": true, //Модуль работы с IBM
    "StartWeather": true, //Модуль погоды
    "StartIndices": true, //Модуль работы с индексами
    "StartJobs": false, //Подсистема планировщик задач
    "StartUgm": true, //Модуль работы с Погоды от УГМ
    "StartUav": true //Модуль БПЛА
  },
  "Logging": { //Правила логирования по минимальному уровню
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": { //Строки подключения к базам данных
    "PgSqlCommonConnection": "Host=localhost;Port=20010;Database=masofa-common-local;User ID=viki;Password=AllRight73!", //Core
    "PgSqlIdentityConnection": "Host=localhost;Port=20010;Database=masofa-identity-local;User ID=viki;Password=AllRight73!", //Контроль доступа пользователй
    "PgSqlDictionaryConnection": "Host=localhost;Port=20010;Database=masofa-dictionary-local;User ID=viki;Password=AllRight73!", //Модуль словарей
    "PgSqlBidsConnection": "Host=localhost;Port=20010;Database=masofa-bids-local;User ID=viki;Password=AllRight73!", //Модуль Мониторинг полей
    "PgSqlLandsatConnection": "Host=localhost;Port=20010;Database=landsat;User ID=viki;Password=AllRight73!", //Модуль работы с Landsat
    "PgSqlSentinelConnection": "Host=localhost;Port=20010;Database=sentinel;User ID=viki;Password=AllRight73!", //Модуль работы с Sentinel
    "PgSqlTilesConnection": "Host=localhost;Port=20010;Database=masofa-tiles-local;User ID=viki;Password=AllRight73!", //Подсистема GIS
    "PgSqlWeatherConnection": "Host=localhost;Port=20010;Database=masofa-weather-local;User ID=viki;Password=AllRight73!", //Модуль погоды
    "PgSqlEraConnection": "Host=localhost;Port=20010;Database=masofa-era-local;User ID=viki;Password=AllRight73!", //Модуль работы с ERA5
    "PgSqlQuartZConnection": "Host=localhost;Port=20010;Database=masofa-era-local;User ID=viki;Password=AllRight73!", //Подсистема планировщик задач
    "PgSqlIndicesConnection": "Host=localhost;Port=20010;Database=masofa-era-local;User ID=viki;Password=AllRight73!", //Модуль работы с индексами
    "PgSqlUgmConnection": "Host=localhost;Port=20010;Database=masofa-ugm-local;User ID=viki;Password=AllRight73!", //Модуль работы с Погоды от УГМ
    "PgSqlIBMConnection": "Host=localhost;Port=20010;Database=masofa-ibm-local;User ID=viki;Password=AllRight73!", //Модуль работы с IBM
    "PgSqlUAVConnection": "Host=localhost;Port=20010;Database=masofa-uav-local;User ID=viki;Password=AllRight73!", //Модуль работы с БПЛА
    "PgSqlDictionaryHistoryConnection": "Host=localhost;Port=20010;Database=masofa-dictionary-history-local;User ID=viki;Password=AllRight73!", //Дочерняя БД для Справочников, где храниться историчность
    "PgSqlCropMonitoringHistoryConnection": "Host=localhost;Port=20010;Database=masofa-crop-monitoring-history-local;User ID=viki;Password=AllRight73!", //Дочерняя БД для Мониторинга полей, где храниться историчность
    "PgSqlAnaliticReportHistoryConnection": "Host=localhost;Port=20010;Database=masofa-farmer-recomendation-report-history-local;User ID=viki;Password=AllRight73!", //Дочерняя БД для Отчётов, где храниться историчность
    "PgSqlCommonHistoryConnection": "Host=localhost;Port=20010;Database=masofa-common-history-local;User ID=viki;Password=AllRight73!", //Дочерняя БД для Core, где храниться историчность
    "PgSqlIdentityHistoryConnection": "Host=localhost;Port=20010;Database=masofa-identity-history-local;User ID=viki;Password=AllRight73!", //Дочерняя БД для Контроля доступа пользователей, где храниться историчность
    "PgSqlLandsatHistoryConnection": "Host=localhost;Port=20010;Database=masofa-landsat-history-local;User ID=viki;Password=AllRight73!", //Дочерняя БД для работы Landsat, где храниться историчность
    "PgSqlSentinelHistoryConnection": "Host=localhost;Port=20010;Database=masofa-sentinel-history-local;User ID=viki;Password=AllRight73!", //Дочерняя БД для Sentinel, где храниться историчность
    "PgSqlTilesHistoryConnection": "Host=localhost;Port=20010;Database=masofa-tiles-history-local;User ID=viki;Password=AllRight73!", //Дочерняя БД для GIS, где храниться историчность
    "PgSqlWeatherHistoryConnection": "Host=localhost;Port=20010;Database=masofa-weather-history-local;User ID=viki;Password=AllRight73!" //Дочерняя БД Weather, где храниться историчность
  },
  "AllowCORS": [ "https://localhost:7249", "http://localhost:5154" ], //Настройки CORS
  "AuthOptions": { //Настройки ключей генерации токенов
    "ISSUER": "MyAuthServer",
    "AUDIENCE": "MyAuthClient",
    "KEY": "mysupersecret_secretsecretsecretkey"
  },
  "NLog": { //Настройки системного логирования
    "throwConfigExceptions": true,
    "targets": {
      "async": true,
      "logfile": {
        "type": "File",
        "fileName": "log/${date:format=yyyy}/${date:format=MM}/${date:format=dd}_${date:format=HH}.log",
        "archiveFileName": "log/${date:format=yyyy}/${date:format=MM}/${date:format=dd}_${date:format=HH}.log",
        "archiveEvery": "Hour",
        "archiveNumbering": "Rolling",
        "maxArchiveFiles": 168,
        "layout": "${longdate} ${uppercase:${level}} ${logger} ${message} ${exception:format=ToString}"
      },
      "logconsole": {
        "type": "ColoredConsole"
      }
    },
    "rules": [
      {
        "logger": "*",
        "minLevel": "Info",
        "writeTo": "logconsole"
      },
      {
        "logger": "*",
        "minLevel": "Debug",
        "writeTo": "logfile"
      }
    ]
  },
  "Minio": { //Настройки доступа в MinIO
    "Endpoint": "localhost",
    "Port": "20040",
    "AccessKey": "sixgrain",
    "SecretKey": "AllRight73!",
    "Secure": false
  },
  "SmtpOptions": { //Настройки доступа к SMTP
    "CallbackUrl": "https://localhost:7088",
    "Host": "postfix",
    "Port": 587,
    "User": "",
    "Password": "",
    "From": "noreply@yourdomain.com",
    "UseSsl": false,
    "HealthCheckRecipient": "admin@yourdomain.com",
    "HealthCheckLanguage": "ru"
  },
  "GeoServerOptions": { //Настройки доступа к GeoServer
    "GeoServerUrl": "http://localhost:8080",
    "Workspace": "workspace",
    "LayerName": "layer",
    "Volume": "/data", // copy from .env
    "UserName": "admin", // copy from .env
    "Password": "geoserver" // copy from .env
  },
  "Landsat": { //Настройки доступа к Landsat
    "MetadataApiUrl": "https://url.example/api",
    "SearchApiUrl": "https://url.example/search",
    "TokenApiUrl": "https://url.example/auth",
    "UserName": "username",
    "Password": "userpassword",
    "Token": "mysecuritytoken",
    "Paths": [],
    "Rows": []
  },
  "Sentinel": {//Настройки доступа к Sentinel
    "ApiUrl": "https://scihub.copernicus.eu/dhus",
    "UserName": "username",
    "Password": "userpassword",
    "TokenApiUrl": "https://identity.dataspace.copernicus.eu/auth/realms/CDSE/protocol/openid-connect/token",
    "ProductSearchApiUrl": "https://catalogue.dataspace.copernicus.eu/odata/v1/Products",
    "ProductDownloadApiUrl": "https://download.dataspace.copernicus.eu/odata/v1/Products",
    "ConvertedTilesPath": "/layers"
  },
  "IBMWeather": {//Настройки доступа к IBM Weather
    "BaseUrl": "https://api.weather.com/v3",
    "ApiKey": "your_api_key_here",
    "Language": "en-US",
    "Format": "json",
    "Units": "s",
    "Step": 0.1
  },
  "DbSeeders": {//Настройки инициализации БД при старте
    "StartIdentity": false,
    "StartDictionaries": true,
    "StartAccessMap": true,
    "StartLockPermission": true
  },
  "Era5": {//Настройки доступа к ERA5
    "ForecastUrl": "https://example.com",
    "ArchiveUrl": "https://archive.example.com",
    "RequestParams": "some_params",
    "NumRetries": 1,
    "RetryDelayMs": 1000,
    "RateLimit": 5,
    "ArchiveStartDate": "2016-01-01",
    "ArchiveEndDate": "2025-09-24",
    "RateLimit": 5,
    "TilesFolder": "/layers"
  },
  "CountryBoundaries": {//Границы страны для построения сетки
    "LatMin": 37.00,
    "LatMax": 45.50,
    "LonMin": 56.00,
    "LonMax": 73.00,
    "Step": 0.25,
    "GeoJsonFileName": "geoBoundaries-UZB-ADM0_simplified.geojson"
  },
  "UgmOptions": {//Настройки доступа к УГМ
    "UrlCurrent": "https://meteoapi.meteo.uz/api/weather/current",
    "UrlForecast": "https://meteoapi.meteo.uz/api/weather/forecast"
  },
  "OneIdOptions": { //Настройки доступа к OneId
    "BaseUrl": "https://base.url",
    "ClientId": "clientId",
    "RedirectUrl": "https://redirect.url",
    "ClientSecret": "some_super_secret_word"
  },
  "FieldPhotosMaxCount": 500
}

```

3. В корне использовать команды 
    1. `docker-compose stop argosence.web.monolith` - для остановки, если контейнер бекенда уже запущен
    2. `docker-compose build --pull argosence.web.monolith` - для сборки контейнера бекенда
    3. `docker-compose up -d argosence.web.monolith` - для запуска контейнара бекенда