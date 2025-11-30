# Caminho: backend/analysis_service/Dockerfile
FROM node:20-alpine

WORKDIR /app

# Instala dependências do sistema
RUN apk add --no-cache curl

# 1. Copia TUDO de uma vez
COPY backend/analysis_service/ .

# 2. Verifica se o script existe
RUN ls -la /app/scripts/ && \
    chmod +x /app/scripts/create-system-user.sh

EXPOSE 3000

# 3. Executa o script
CMD sh -c "/app/scripts/create-system-user.sh && npm start"