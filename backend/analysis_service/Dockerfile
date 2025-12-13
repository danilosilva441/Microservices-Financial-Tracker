FROM node:20-alpine

WORKDIR /app

# Instala dependências do sistema e o dos2unix (para corrigir arquivos do Windows)
RUN apk add --no-cache curl dos2unix

# 1. Copia primeiro o package.json (Melhora o cache do Docker)
COPY backend/analysis_service/package*.json ./

# 2. Instala as dependências do Node (Faltava isso!)
RUN npm install

# 3. Copia o restante do código fonte
COPY backend/analysis_service/ .

# 4. Corrige a quebra de linha do script (CRLF -> LF) e dá permissão
# O '*' garante que pegue o arquivo independente se você renomeou ou não, 
# mas o ideal é que o arquivo esteja em minúsculo.
RUN dos2unix scripts/*.sh && \
    chmod +x scripts/*.sh

EXPOSE 3000

# 5. Executa o script e depois inicia o servidor
# Atenção: Certifique-se que o arquivo na pasta chama-se 'create-system-user.sh'
CMD ["sh", "-c", "./scripts/create-system-user.sh && npm run dev"]