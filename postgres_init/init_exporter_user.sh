#!/bin/bash
set -e

echo "Running initialization script for database '$POSTGRES_DB' using separate psql commands..."

echo "Ensuring pg_stat_statements extension exists..."
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
    -c "CREATE EXTENSION IF NOT EXISTS pg_stat_statements;"

echo "Checking and creating user '$EXPORTER_USER' if necessary..."
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
    -c "DO \$\$ BEGIN IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = '${EXPORTER_USER}') THEN RAISE NOTICE 'User \"${EXPORTER_USER}\" does not exist. Creating user...'; CREATE USER ${EXPORTER_USER} WITH PASSWORD '${EXPORTER_PASSWORD}'; ELSE RAISE NOTICE 'User \"${EXPORTER_USER}\" already exists.'; END IF; END \$\$;"

echo "Granting privileges to user '$EXPORTER_USER'..."

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
    -c "GRANT CONNECT ON DATABASE ${POSTGRES_DB} TO ${EXPORTER_USER};"

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
    -c "GRANT pg_monitor TO ${EXPORTER_USER};"

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
    -c "GRANT SELECT ON pg_stat_statements TO ${EXPORTER_USER};"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
    -c "GRANT SELECT ON pg_stat_statements_info TO ${EXPORTER_USER};"

# Надаємо право на скидання статистики (закоментуйте, якщо не потрібно)
# echo "Granting function execution privilege (optional)..."
# psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
#    -c "GRANT EXECUTE ON FUNCTION pg_stat_statements_reset() TO ${EXPORTER_USER};" 
# Або з повною сигнатурою:
# psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
#    -c "GRANT EXECUTE ON FUNCTION pg_stat_statements_reset(oid, oid, bigint, boolean) TO ${EXPORTER_USER};"

echo "Initialization script finished for database '$POSTGRES_DB'."