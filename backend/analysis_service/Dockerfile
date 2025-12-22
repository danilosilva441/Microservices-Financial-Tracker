FROM node:20-alpine

WORKDIR /app

# Instala dependências do sistema
RUN apk add --no-cache curl dos2unix

# 1. Copia package.json
COPY backend/analysis_service/package*.json ./

# 2. Instala dependências
RUN npm install

# 3. Copia o código
COPY backend/analysis_service/ .

# 4. Corrige permissões do script
RUN dos2unix scripts/*.sh && \
    chmod +x scripts/*.sh

EXPOSE 3000

# 5. MUDANÇA AQUI:
# Em vez de 'npm run dev' (que usa nodemon), chamamos o node direto.
# Assumindo que seu arquivo principal é src/index.js (baseado nos logs anteriores)
CMD ["sh", "-c", "./scripts/create-system-user.sh && node src/index.js"]