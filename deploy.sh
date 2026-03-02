#!/bin/bash

# --- Загрузка переменных из env.config ---
ENV_CONFIG_FILE="env.config"

if [[ -f "$ENV_CONFIG_FILE" ]]; then
    echo "Загрузка переменных из $ENV_CONFIG_FILE..."
    # Используем source для загрузки переменных окружения из файла
    source "$ENV_CONFIG_FILE"
else
    echo "ОШИБКА: Файл конфигурации '$ENV_CONFIG_FILE' не найден в текущей директории."
    exit 1
fi

# --- Объявление ассоциативного массива: [имя_пользователя]=[имя_базы_данных] ---
# ВАЖНО: Замените эту строку на нужный вам набор пользователь-база
declare -A DB_USER_MAP=(
    ["masofaanaliticreport"]="masofa-analitic-report-prod"
    ["masofaanaliticreporthistory"]="masofa-analitic-report-history-prod"
    ["masofacommonhistory"]="masofa-common-history-prod"
    ["masofacommon"]="masofa-common-prod"
    ["masofacropmonitoringhistory"]="masofa-crop-monitoring-history-prod"
    ["masofacropmonitoring"]="masofa-crop-monitoring-prod"
    ["masofadictionarieshistory"]="masofa-dictionaries-history-prod"
    ["masofadictionaries"]="masofa-dictionaries-prod"
    ["masofaera"]="masofa-era-prod"
    ["masofaibm"]="masofa-ibm-prod"
    ["masofaidentityhistory"]="masofa-identity-history-prod"
    ["masofaidentity"]="masofa-identity-prod"
    ["masofaindices"]="masofa-indices-prod"
    ["masofalandsathistory"]="masofa-landsat-history-prod"
    ["masofalandsat"]="masofa-landsat-prod"
    ["masofaquartz"]="masofa-quartz-prod"
    ["masofasentinelhistory"]="masofa-sentinel-history-prod"
    ["masofasentinel"]="masofa-sentinel-prod"
    ["masofatileshistory"]="masofa-tiles-history-prod"
    ["masofatiles"]="masofa-tiles-prod"
    ["masofauav"]="masofa-uav-prod"
    ["masofaugm"]="masofa-ugm-prod"
    ["masofaweathererareport"]="masofa-weather-era-report-prod"
    ["masofaweatherhistory"]="masofa-weather-history-prod"
    ["masofaweather"]="masofa-weather-prod"
    # Пример добавления:
    # ["another_user"]="another_db"
)

    declare -A DB_MIGRATION_MAP=(
    ["masofaanaliticreport"]="Masofa.DataAccess/Migrations/AnaliticReport/apply_all_migrations.sql"
    ["masofaanaliticreporthistory"]="Masofa.DataAccess/Migrations/AnaliticReportHistory/apply_all_migrations.sql"
    ["masofacommonhistory"]="Masofa.DataAccess/Migrations/CommonHistory/apply_all_migrations.sql"
    ["masofacommon"]="Masofa.DataAccess/Migrations/Common/apply_all_migrations.sql"
    ["masofacropmonitoringhistory"]="Masofa.DataAccess/Migrations/CropMonitoringHistory/apply_all_migrations.sql"
    ["masofacropmonitoring"]="Masofa.DataAccess/Migrations/CropMonitoring/apply_all_migrations.sql"
    ["masofadictionarieshistory"]="Masofa.DataAccess/Migrations/DictionariesHistory/apply_all_migrations.sql"
    ["masofadictionaries"]="Masofa.DataAccess/Migrations/Dictionaries/apply_all_migrations.sql"
    ["masofaera"]="Masofa.DataAccess/Migrations/Era/apply_all_migrations.sql"
    ["masofaibm"]="Masofa.DataAccess/Migrations/IBMWeather/apply_all_migrations.sql"
    ["masofaidentityhistory"]="Masofa.DataAccess/Migrations/IdentityHistory/apply_all_migrations.sql"
    ["masofaidentity"]="Masofa.DataAccess/Migrations/Identity/apply_all_migrations.sql"
    ["masofaindices"]="Masofa.DataAccess/Migrations/Indices/apply_all_migrations.sql"
    ["masofalandsathistory"]="Masofa.DataAccess/Migrations/LandsatHistory/apply_all_migrations.sql"
    ["masofalandsat"]="Masofa.DataAccess/Migrations/Landsat/apply_all_migrations.sql"
    ["masofaquartz"]="Scripts/quartz_migration_script.sql"
    ["masofasentinelhistory"]="Masofa.DataAccess/Migrations/SentinelHistory/apply_all_migrations.sql"
    ["masofasentinel"]="Masofa.DataAccess/Migrations/Sentinel/apply_all_migrations.sql"
    ["masofatileshistory"]="Masofa.DataAccess/Migrations/TileHistory/apply_all_migrations.sql"
    ["masofatiles"]="Masofa.DataAccess/Migrations/Tile/apply_all_migrations.sql"
    ["masofauav"]="Masofa.DataAccess/Migrations/Uav/apply_all_migrations.sql"
    ["masofaugm"]="Masofa.DataAccess/Migrations/Ugm/apply_all_migrations.sql"
    ["masofaweathererareport"]="Masofa.DataAccess/Migrations/WeatherReport/apply_all_migrations.sql"
    ["masofaweatherhistory"]="Masofa.DataAccess/Migrations/WeatherHistory/apply_all_migrations.sql"
    ["masofaweather"]="Masofa.DataAccess/Migrations/Weather/apply_all_migrations.sql"
    # Пример добавления:
    # ["another_user"]="another_db"
)

# --- Проверка обязательных переменных ---
check_var() {
    local var_name="$1"
    if [[ -z "${!var_name}" ]]; then
        echo "ОШИБКА: Переменная '$var_name' не задана в файле '$ENV_CONFIG_FILE'."
        exit 1
    else
        # Выводим, что переменная загружена (для отладки, можно убрать)
        # echo "  Переменная $var_name: ${!var_name}"
        echo "  Переменная $var_name загружена."
    fi
}

echo "Проверка обязательных переменных..."
check_var "POSTGRES_USER"
check_var "POSTGRES_PASSWORD"
check_var "POSTGRES_DB"
check_var "MINIO_USER"
check_var "MINIO_PASSWORD"
check_var "GEOSERVER_ADMIN_USER"
check_var "GEOSERVER_ADMIN_PASSWORD"
check_var "DEPLOY_ROOT_FOLDER"
check_var "MODE"
check_var "ASPNETCORE_ENVIRONMENT"
echo "Все обязательные переменные загружены."

# --- Конфигурационные переменные (берутся из env.config) ---
# Значения по умолчанию больше не нужны, так как проверка происходит выше
POSTGRES_USER="${POSTGRES_USER}"
POSTGRES_PASSWORD="${POSTGRES_PASSWORD}"
POSTGRES_DB="${POSTGRES_DB}"
MINIO_USER="${MINIO_USER}"
MINIO_PASSWORD="${MINIO_PASSWORD}"
GEOSERVER_ADMIN_USER="${GEOSERVER_ADMIN_USER}"
GEOSERVER_ADMIN_PASSWORD="${GEOSERVER_ADMIN_PASSWORD}"
DEPLOY_ROOT="${DEPLOY_ROOT_FOLDER}" # Используем синоним для удобства
MODE="${MODE}"
ASPNETCORE_ENV="${ASPNETCORE_ENVIRONMENT}"

# --- Функции ---

# Проверка, установлен ли пакет
is_package_installed() {
    local package="$1"
    if command -v dpkg &> /dev/null; then
        dpkg -l "$package" &> /dev/null
    elif command -v snap &> /dev/null; then
        snap list "$package" &> /dev/null
    else
        return 1
    fi
}

# Установка .NET 9.0
install_dotnet() {
    echo "Установка .NET 9.0 SDK..."
    if is_package_installed "dotnet9-sdk"; then
        echo "  .NET 9.0 SDK уже установлен."
    else
        wget -q https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
        sudo dpkg -i packages-microsoft-prod.deb
        sudo apt update
        sudo apt install -y dotnet-sdk-9.0
        echo "  .NET 9.0 SDK установлен."
    fi
}

# Установка PostgreSQL и PostGIS
install_postgres() {
    echo "Установка PostgreSQL и PostGIS..."
    if is_package_installed "postgresql" && is_package_installed "postgis"; then
        echo "  PostgreSQL и PostGIS уже установлены."
    else
        sudo apt update
        sudo apt install -y postgresql postgis postgresql-contrib
        echo "  PostgreSQL и PostGIS установлены."
    fi
}

# Установка MinIO
install_minio() {
    echo "Установка MinIO..."
    if command -v minio &> /dev/null; then
        echo "  MinIO уже установлен."
    else
        wget -O minio https://dl.min.io/server/minio/release/linux-amd64/minio
        chmod +x minio
        sudo mv minio /usr/local/bin/minio
        sudo useradd -r minio-user || true # Игнорировать ошибку, если пользователь уже существует
        echo "  MinIO установлен."
    fi
}

# Установка GeoServer
install_geoserver() {
    echo "Установка GeoServer..."
    if is_package_installed "openjdk-17-jdk"; then
        echo "  OpenJDK 17 уже установлен."
    else
        sudo apt install -y openjdk-17-jdk
        echo "  OpenJDK 17 установлен."
    fi
    if is_package_installed "geoserver"; then
        echo "  GeoServer уже установлен."
    else
        echo "  GeoServer не установлен."
        sudo add-apt-repository ppa:ubuntugis/ppa && sudo apt update && sudo apt install geoserver
    fi
    #sudo add-apt-repository ppa:ubuntugis/ppa && sudo apt update && sudo apt install geoserver
    # GeoServer обычно устанавливается вручную или из PPA
    # 
    # Или скачивание и распаковка вручную
    # Здесь просто проверим, установлен ли Java, так как это основная зависимость
}

# Установка NGINX
install_nginx() {
    echo "Установка NGINX..."
    if is_package_installed "nginx"; then
        echo "  NGINX уже установлен."
    else
        sudo apt install -y nginx
        echo "  NGINX установлен."
    fi
}

# Установка GDAL
install_gdal() {
    echo "Установка GDAL..."
    if is_package_installed "gdal-bin"; then
        echo "  GDAL уже установлен."
    else
        sudo apt install -y gdal-bin libgdal-dev proj-data proj-bin
        echo "  GDAL установлен."
    fi
}

# Сброс паролей/настроек
reset_postgres() {
    echo "Сброс настроек PostgreSQL..."
    # Проверяем, запущен ли сервис
    if sudo systemctl is-active --quiet postgresql; then
        echo "  Сервис PostgreSQL запущен."
    else
        echo "  Сервис PostgreSQL не запущен. Попробуем запустить..."
        sudo systemctl start postgresql
        sleep 3 # Подождать запуска
    fi

    # Создание/обновление пользователя и базы данных
    sudo -u postgres psql -c "DO \$\$ BEGIN IF NOT EXISTS (SELECT FROM pg_user WHERE usename = '${POSTGRES_USER}') THEN CREATE USER ${POSTGRES_USER} WITH PASSWORD '${POSTGRES_PASSWORD}'; END IF; END \$\$;"
    sudo -u postgres psql -c "ALTER USER ${POSTGRES_USER} WITH PASSWORD '${POSTGRES_PASSWORD}';"
    echo "  Пароль пользователя '${POSTGRES_USER}' обновлен."

    # Проверяем, существует ли база данных
    if sudo -u postgres psql -tAc "SELECT 1 FROM pg_database WHERE datname='${POSTGRES_DB}'" | grep -q 1; then
        echo "  База данных '${POSTGRES_DB}' уже существует."
    else
        sudo -u postgres createdb -O "${POSTGRES_USER}" "${POSTGRES_DB}"
        echo "  База данных '${POSTGRES_DB}' создана."
    fi

    # Проверяем, активировано ли расширение PostGIS
    if sudo -u postgres psql -d "${POSTGRES_DB}" -tAc "SELECT 1 FROM pg_extension WHERE extname = 'postgis';" | grep -q 1; then
        echo "  Расширение PostGIS уже активировано в '${POSTGRES_DB}'."
    else
        sudo -u postgres psql -d "${POSTGRES_DB}" -c "CREATE EXTENSION postgis;"
        echo "  Расширение PostGIS активировано в '${POSTGRES_DB}'."
    fi
}

reset_minio() {
    echo "Сброс настроек MinIO..."
    # Проверяем существование директории данных
    MINIO_DATA_DIR="${DEPLOY_ROOT}/data-minio-${MODE}"
    sudo mkdir -p "${MINIO_DATA_DIR}"
    sudo chown -R minio-user:minio-user "${MINIO_DATA_DIR}"

    echo "  Папка данных MinIO: ${MINIO_DATA_DIR}"
    echo "  Для сброса учетных данных, перезапустите MinIO с переменными MINIO_ROOT_USER и MINIO_ROOT_PASSWORD."
    echo "  Пример команды запуска (требуется sudo):"
    echo "  sudo -u minio-user MINIO_ROOT_USER='${MINIO_USER}' MINIO_ROOT_PASSWORD='${MINIO_PASSWORD}' minio server ${MINIO_DATA_DIR} --console-address :9001"
}

reset_geoserver() {
    echo "Сброс настроек GeoServer..."
    echo "  Сброс пароля администратора GeoServer требует ручного редактирования файлов конфигурации в папке данных."
    echo "  Папка данных GeoServer (обычно): /opt/geoserver/data_dir (если установлен стандартно)"
    echo "  Или в ${DEPLOY_ROOT}/geoserver-data-${MODE} (если используется пользовательская)."
    echo "  Учетные данные: ${GEOSERVER_ADMIN_USER} / ${GEOSERVER_ADMIN_PASSWORD}"
}

reset_nginx() {
    echo "Сброс настроек NGINX..."
    if sudo systemctl is-active --quiet nginx; then
        echo "  Сервис NGINX запущен."
    else
        echo "  Сервис NGINX не запущен. Попробуем запустить..."
        sudo systemctl start nginx
    fi
    echo "  Проверьте конфигурацию NGINX в /etc/nginx/sites-available/ и связанные файлы."
}

reset_dotnet_app() {
    echo "Сброс настроек .NET приложения..."
    if command -v dotnet &> /dev/null; then
        echo "  .NET SDK установлен."
    else
        echo "  ОШИБКА: .NET SDK не установлен. Выполните 'Install/Repair Requirements'."
        return 1
    fi
    echo "  Для запуска приложения, выполните сборку и публикацию: dotnet publish"
    echo "  Убедитесь, что переменные окружения (например, ASPNETCORE_ENVIRONMENT) установлены."
    echo "  Текущее окружение: ${ASPNETCORE_ENV}"
}

# --- Функция для сброса настроек PostgreSQL с созданием нескольких БД и пользователей ---
reset_postgres_multiple_dbs() {
    echo "Сброс настроек PostgreSQL с созданием нескольких БД и пользователей..."

    # Проверяем, запущен ли сервис
    if sudo systemctl is-active --quiet postgresql; then
        echo "  Сервис PostgreSQL запущен."
    else
        echo "  Сервис PostgreSQL не запущен. Попробуем запустить..."
        sudo systemctl start postgresql
        sleep 3 # Подождать запуска
    fi

    # --- Файл для сохранения строк подключения ---
    CONNECTION_STRINGS_FILE="connection_strings.txt"
    # Очищаем файл перед записью
    > "$CONNECTION_STRINGS_FILE"

    # --- Цикл по массиву для создания пользователей и баз ---
    for user in "${!DB_USER_MAP[@]}"; do
        db="${DB_USER_MAP[$user]}"
        password_var_name="${user^^}_PASSWORD" # Пример: USER1_PASSWORD
        # Генерация или получение пароля
        if [[ -n "${!password_var_name}" ]]; then
            password="${!password_var_name}"
            echo "  Используется пароль из переменной: $password_var_name"
        else
            password=$(LC_ALL=C tr -dc 'A-Za-z0-9' </dev/urandom | head -c 12)
            echo "  Сгенерирован случайный пароль для '$user'"
        fi

        echo "  Обработка пользователя: $user, БД: $db"

        # Создание/обновление пользователя с паролем
        sudo -u postgres psql -c "DO \$\$ BEGIN IF NOT EXISTS (SELECT FROM pg_user WHERE usename = '${user}') THEN CREATE USER ${user} WITH PASSWORD '${password}'; END IF; END \$\$;"

        # Обновление пароля (на случай, если пользователь уже существует)
        sudo -u postgres psql -c "ALTER USER ${user} WITH PASSWORD '${password}';"
        echo "  Пароль пользователя '${user}' обновлен (или пользователь создан)."

        # Проверяем, существует ли база данных
        if sudo -u postgres psql -tAc "SELECT 1 FROM pg_database WHERE datname='${db}'" | grep -q 1; then
            echo "  База данных '${db}' уже существует."
        else
            sudo -u postgres createdb -O "${user}" "${db}"
            echo "  База данных '${db}' создана и принадлежит пользователю '${user}'."
        fi

        migration_sql_path="${DB_MIGRATION_MAP[$user]}"
        echo " Файл миграции для '${db}' => '${migration_sql_path}'"

        # Применение миграции, если файл указан и существует
        if [[ -n "$migration_sql_path" && -f "$migration_sql_path" ]]; then
            echo "  Применяется миграция: $migration_sql_path к БД '$db' от имени пользователя '$user'..."
            sudo -u postgres psql -d "$db" -f "$migration_sql_path" > /dev/null
            if [[ $? -eq 0 ]]; then
                echo "  Миграция успешно применена."
            else
                echo "  ОШИБКА: Не удалось применить миграцию '$migration_sql_path' к БД '$db'."
            fi
        elif [[ -n "$migration_sql_path" ]]; then
            echo "  ПРЕДУПРЕЖДЕНИЕ: Файл миграции не найден: $migration_sql_path"
        else
            echo "  Миграция не указана для пользователя '$user'."
        fi

        # Генерация строки подключения
        # ВАЖНО: Замените Host и Port на актуальные для вашего сервера
        host="127.0.0.1" # Укажите нужный хост
        port="5432"           # Укажите нужный порт
        conn_str="Host=${host};Port=${port};Database=${db};User ID=${user};Password=${password}"
        echo "$conn_str" >> "$CONNECTION_STRINGS_FILE"
        echo "  Строка подключения для '${user}' добавлена в $CONNECTION_STRINGS_FILE"

        # Активация расширения PostGIS в этой базе данных (если оно требуется)
        # ВАЖНО: Если PostGIS нужен для всех баз, раскомментируйте следующие строки:
        if sudo -u postgres psql -d "${db}" -tAc "SELECT 1 FROM pg_extension WHERE extname = 'postgis';" | grep -q 1; then
            echo "  Расширение PostGIS уже активировано в '${db}'."
        else
            sudo -u postgres psql -d "${db}" -c "CREATE EXTENSION postgis;"
            echo "  Расширение PostGIS активировано в '${db}'."
        fi
    done

    echo "  Все пользователи и базы данных созданы/обновлены."
    echo "  Строки подключения сохранены в файл: $CONNECTION_STRINGS_FILE"
}

remove_all_requirements() {
    echo "⚠️  ВНИМАНИЕ: Эта операция удалит все компоненты, установленные скриптом."
    read -p "Продолжить? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Отмена."
        exit 0
    fi

    # 1. Остановка сервисов
    echo "Остановка сервисов..."
    sudo systemctl stop postgresql nginx minio geoserver 2>/dev/null

    # 2. Удаление .NET SDK и репозитория Microsoft
    echo "Удаление .NET..."
    sudo apt remove -y dotnet-sdk-9.0 dotnet-runtime-9.0 aspnetcore-runtime-9.0
    sudo apt autoremove -y
    sudo rm -f /etc/apt/sources.list.d/microsoft-prod.list
    sudo rm -f packages-microsoft-prod.deb
    sudo apt update

    # 3. Удаление PostgreSQL и PostGIS
    echo "Удаление PostgreSQL и PostGIS..."
    sudo apt remove -y --purge postgresql\* postgis\* postgresql-contrib
    sudo apt autoremove -y
    sudo rm -rf /etc/postgresql /var/lib/postgresql /var/log/postgresql

    # 4. Удаление MinIO
    echo "Удаление MinIO..."
    sudo rm -f /usr/local/bin/minio
    sudo userdel -r minio-user 2>/dev/null || sudo userdel minio-user 2>/dev/null
    # ⚠️ УДАЛЕНИЕ ДАННЫХ MinIO (раскомментируй, если нужно):
    # sudo rm -rf /path/to/your/minio/data  # например: ${DEPLOY_ROOT}/data-minio-*

    # 5. Удаление GeoServer
    echo "Удаление GeoServer..."
    # Если установлен через PPA:
    sudo apt remove -y --purge geoserver tomcat9\* 2>/dev/null
    # Если установлен вручную — удали вручную:
    sudo rm -rf /opt/geoserver
    # Удаляем Java только если она не нужна другим приложениям:
    # sudo apt remove -y openjdk-17-jdk openjdk-17-jre

    # 6. Удаление NGINX
    echo "Удаление NGINX..."
    sudo apt remove -y --purge nginx nginx-common nginx-full
    sudo apt autoremove -y
    sudo rm -rf /etc/nginx /var/log/nginx

    # 7. Удаление GDAL и связанных библиотек
    echo "Удаление GDAL..."
    sudo apt remove -y --purge gdal-bin libgdal-dev proj-data proj-bin python3-gdal
    sudo apt autoremove -y

    # 8. Удаление PPA ubuntugis (если остался)
    echo "Удаление PPA ubuntugis..."
    sudo add-apt-repository --remove ppa:ubuntugis/ppa 2>/dev/null || true

    # 9. Очистка кэша и зависимостей
    echo "Очистка системы..."
    sudo apt clean
    sudo apt autoclean

    echo "✅ Все компоненты, установленные скриптом, удалены."
    echo "💡 Примечание: данные приложений (БД, файлы MinIO и т.д.) могут остаться — удалите их вручную при необходимости."
}

# --- Меню ---

show_menu() {
    echo "==================================="
    echo "    Меню установки и сброса"
    echo "==================================="
    echo "1. Install//Repair Requirements"
    echo "2. Reset Configuration (PostgreSQL, MinIO, etc.)"
    echo "3. Reset Databases"
    echo "4.  ⚠️ Remove All Requirements  ⚠️"
    echo "5. Exit"
    echo "==================================="
    read -p "Выберите пункт (1-5): " choice
    return $choice
}

# --- Основной цикл ---

while true; do
    show_menu
    choice_ret=$?
    echo "Вы выбрали: $choice_ret"
    case $choice_ret in
        1)
            echo "==================================="
            echo "Выполняется: Install//Repair Requirements"
            echo "==================================="
            # Обновление системы
            sudo apt update && sudo apt upgrade -y
            sudo apt install -y curl wget gnupg apt-transport-https lsb-release software-properties-common

            install_dotnet
            install_postgres
            install_minio
            install_geoserver
            install_nginx
            install_gdal

            echo "==================================="
            echo "Установка зависимостей завершена!"
            echo "==================================="
            read -p "Нажмите Enter для возврата в меню..."
            ;;
        2)
            echo "==================================="
            echo "Выполняется: Reset Configuration"
            echo "==================================="
            reset_postgres
            reset_minio
            reset_geoserver
            reset_nginx
            reset_dotnet_app
            echo "==================================="
            echo "Сброс конфигурации завершен!"
            echo "==================================="
            read -p "Нажмите Enter для возврата в меню..."
            ;;
        3)
            echo "==================================="
            echo "Выполняется: Reset Баз данных"
            echo "==================================="
            reset_postgres_multiple_dbs
            echo "==================================="
            echo "Сброс  Баз данных завершен!"
            echo "==================================="
            read -p "Нажмите Enter для возврата в меню..."
            ;;
        4)
            echo "==================================="
            echo "Выполняется:  ⚠️ Удаление ВСЕХ зависимостей  ⚠️"
            echo "==================================="
            remove_all_requirements
            echo "==================================="
            echo "Сброс  зависимостей"
            echo "==================================="
            read -p "Нажмите Enter для возврата в меню..."
            ;;
        5)
            echo "Выход."
            exit 0
            ;;
        *)
            echo "Неверный выбор. Пожалуйста, выберите 1, 2 или 3."
            read -p "Нажмите Enter для продолжения..."
            ;;
    esac
done