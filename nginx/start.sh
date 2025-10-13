#!/bin/sh
set -e  # Faz o script parar se houver qualquer erro

echo "======================================="
echo "🚀 Iniciando configuração dinâmica do Nginx"
echo "======================================="

# Lista de variáveis obrigatórias
REQUIRED_VARS="AUTH_SERVICE_URL BILLING_SERVICE_URL ANALYSIS_SERVICE_URL FRONTEND_URL"

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
  echo "======================================="
  echo "❌ Interrompendo: faltam variáveis de ambiente obrigatórias."
  echo "======================================="
  exit 1
fi

# Substitui variáveis do template
echo "🛠️  Gerando configuração final..."
envsubst '\$AUTH_SERVICE_URL \$BILLING_SERVICE_URL \$ANALYSIS_SERVICE_URL \$FRONTEND_URL' \
  < /etc/nginx/conf.d/nginx.template \
  > /etc/nginx/conf.d/default.conf

echo "✅ Arquivo /etc/nginx/conf.d/default.conf gerado com sucesso!"

# Exibe as 20 primeiras linhas do arquivo final (para debug)
echo "---------------------------------------"
head -n 20 /etc/nginx/conf.d/default.conf
echo "---------------------------------------"

# Inicia o Nginx
echo "🚀 Iniciando Nginx..."
nginx -g 'daemon off;'
