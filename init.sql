-- Este script será executado automaticamente pelo contêiner do PostgreSQL na primeira vez que ele iniciar.
-- Ele apenas cria os bancos de dados vazios. O Entity Framework cuidará de criar as tabelas dentro deles.
SELECT 'CREATE DATABASE auth_db'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'auth_db')\gexec

SELECT 'CREATE DATABASE billing_db'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'billing_db')\gexec

SELECT 'CREATE DATABASE analysis_db'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'analysis_db')\gexec