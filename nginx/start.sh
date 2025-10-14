#!/bin/sh
set -e

echo "======================================="
echo "🚀 Iniciando configuração dinâmica do Nginx"
echo "======================================="

# --- VARIÁVEIS OBRIGATÓRIAS ---
REQUIRED_VARS="AUTH_SERVICE_HOST AUTH_SERVICE_PORT BILLING_SERVICE_HOST BILLING_SERVICE_PORT ANALYSIS_SERVICE_HOST ANALYSIS_SERVICE_PORT FRONTEND_HOST FRONTEND_PORT"

# Verifica se todas as variáveis estão definidas
for var in $REQUIRED_VARS; do
  if [ -z "$(eval echo \$$var)" ]; then
    echo "❌ ERRO: Variável de ambiente $var não definida!"
    MISSING_VARS=true
  else
    echo "✅ $var = $(eval echo \$$var)"
  fi
done

# --- CORREÇÃO: Também mostra a PORT que será usada ---
echo "✅ PORT = ${PORT:-80}"

if [ "$MISSING_VARS" = true ]; then
  echo "❌ Interrompendo: faltam variáveis de ambiente obrigatórias."
  exit 1
fi

# --- CORREÇÃO: Adiciona PORT às variáveis para substituição ---
echo "🔧 Gerando configuração do Nginx..."
envsubst '${PORT} ${AUTH_SERVICE_HOST} ${AUTH_SERVICE_PORT} ${BILLING_SERVICE_HOST} ${BILLING_SERVICE_PORT} ${ANALYSIS_SERVICE_HOST} ${ANALYSIS_SERVICE_PORT} ${FRONTEND_HOST} ${FRONTEND_PORT}' \
  < /etc/nginx/templates/default.conf.template \
  > /etc/nginx/conf.d/default.conf

echo "✅ Ficheiro /etc/nginx/conf.d/default.conf gerado com sucesso!"
echo "---------------------------------------"

# --- CORREÇÃO: Testa a configuração antes de iniciar ---
echo "🔧 Testando configuração do Nginx..."
nginx -t

echo "======================================="
echo "🚀 Iniciando configuração dinâmica do Nginx"
echo "======================================="
echo "✅ PORT definida como: $PORT"

# Inicia o Nginx
exec nginx -g 'daemon off;'