#!/bin/sh
# Este script garante que o utilizador de sistema para o AnalysisService exista e seja um Admin.

set -e # Faz o script parar imediatamente se houver um erro

echo "--- Executando script de configuração do utilizador de sistema ---"

# Espera o AuthService estar disponível e a responder
echo "A aguardar pelo AuthService em $AUTH_SERVICE_URL..."
until curl -s -f "$AUTH_SERVICE_URL/health"; do
  echo "AuthService indisponível - a aguardar 5 segundos..."
  sleep 5
done
echo "✅ AuthService está online!"

echo "A tentar criar o utilizador de sistema: $SYSTEM_EMAIL"

# Tenta criar o utilizador. O 'curl' retorna um código de erro se a resposta não for 2xx.
# Usamos '|| true' para que o script não pare se o utilizador já existir (o que retorna 400).
CREATE_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" -X POST \
  -H "Content-Type: application/json" \
  -d "{\"email\": \"$SYSTEM_EMAIL\", \"password\": \"$SYSTEM_PASSWORD\"}" \
  "$AUTH_SERVICE_URL/api/users")

if [ "$CREATE_RESPONSE" = "201" ]; then
  echo "✅ Utilizador de sistema '$SYSTEM_EMAIL' criado com sucesso."
elif [ "$CREATE_RESPONSE" = "400" ]; then
  echo "ℹ️  Utilizador de sistema '$SYSTEM_EMAIL' já existe. A continuar."
else
  echo "❌ Erro inesperado ao criar o utilizador de sistema. Código de resposta: $CREATE_RESPONSE"
  # Não saímos com erro, pois pode ser um problema temporário e o cron tentará novamente.
fi

echo "A tentar promover o utilizador de sistema a Admin..."

PROMOTE_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" -X POST \
  -H "Content-Type: application/json" \
  -d "{\"email\": \"$SYSTEM_EMAIL\"}" \
  "$AUTH_SERVICE_URL/api/admin/promote-to-admin")

if [ "$PROMOTE_RESPONSE" = "200" ]; then
  echo "✅ Utilizador de sistema promovido a Admin com sucesso."
else
  # Se a resposta não for 200 (ex: 404, 500), apenas informa.
  echo "ℹ️  Promoção do utilizador de sistema terminou com o código $PROMOTE_RESPONSE (provavelmente já é Admin ou houve um erro)."
fi

echo "--- Script de configuração concluído ---"