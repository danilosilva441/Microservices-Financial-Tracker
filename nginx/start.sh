#!/bin/sh
set -e

echo "======================================="
echo "🚀 Iniciando configuração dinâmica do Nginx"
echo "======================================="

# --- CORREÇÃO: Lista de variáveis agora corresponde ao template ---
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

if [ "$MISSING_VARS" = true ]; then
  echo "❌ Interrompendo: faltam variáveis de ambiente obrigatórias."
  exit 1
fi

# --- CORREÇÃO: Lista de variáveis para o envsubst agora corresponde ao template ---
# Substitui as variáveis e gera o ficheiro de configuração final
envsubst '$AUTH_SERVICE_HOST $AUTH_SERVICE_PORT $BILLING_SERVICE_HOST $BILLING_SERVICE_PORT $ANALYSIS_SERVICE_HOST $ANALYSIS_SERVICE_PORT $FRONTEND_HOST $FRONTEND_PORT' \
  < /etc/nginx/templates/default.conf.template \
  > /etc/nginx/conf.d/default.conf

echo "✅ Ficheiro /etc/nginx/conf.d/default.conf gerado com sucesso!"
echo "---------------------------------------"
head -n 20 /etc/nginx/conf.d/default.conf
echo "---------------------------------------"

# Inicia o Nginx
echo "🚀 Iniciando Nginx..."
nginx -g 'daemon off;'