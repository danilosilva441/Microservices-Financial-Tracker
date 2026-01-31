#!/bin/sh
# v2.0 - Atualizado para o AuthService refatorado

set -e 

echo "--- Executando script de configuração do utilizador de sistema (v2.0) ---"

# Espera o AuthService estar disponível
echo "A aguardar pelo AuthService em $AUTH_SERVICE_URL..."
until curl -s -f "$AUTH_SERVICE_URL/health"; do
  echo "AuthService indisponível - a aguardar 5 segundos..."
  sleep 5
done
echo "✅ AuthService está online!"

echo "A tentar criar o utilizador de sistema: $SYSTEM_EMAIL"

# --- 1. MUDANÇA: Chama o endpoint /api/users/register (v2.0) ---
CREATE_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" -X POST \
  -H "Content-Type: application/json" \
  -d "{\"email\": \"$SYSTEM_EMAIL\",  \"password\": \"$SYSTEM_PASSWORD\", \"role\": \"Admin\", \"systemCreationKey\": \"$BOOT_STRAP_SECRET\"}" \
  "$AUTH_SERVICE_URL/api/users/system-user") # <-- ROTA CORRIGIDA

if [ "$CREATE_RESPONSE" = "201" ]; then
  echo "✅ Utilizador de sistema '$SYSTEM_EMAIL' criado com sucesso (Perfil: Admin)."
elif [ "$CREATE_RESPONSE" = "400" ]; then
  echo "ℹ️  Utilizador de sistema '$SYSTEM_EMAIL' já existe. A continuar."
else
  echo "❌ Erro inesperado ao criar o utilizador de sistema. Código de resposta: $CREATE_RESPONSE"
fi

echo "--- Script de configuração v2.0 concluído ---"