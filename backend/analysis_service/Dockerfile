FROM node:20-alpine

WORKDIR /app

# Instala o curl, necessário para o nosso script
RUN apk add --no-cache curl

COPY package*.json ./
RUN npm install

# Copia todo o código-fonte (src, etc.)
COPY . .

# Copia o script da sua nova localização local
# (procura por ./scripts/create-system-user.sh)
COPY scripts/create-system-user.sh /app/create-system-user.sh

# Dá permissão de execução ao script
RUN chmod +x /app/create-system-user.sh

EXPOSE 3000

# Comando final: executa o script de configuração E DEPOIS inicia a aplicação Node.js
CMD ["/bin/sh", "-c", "/app/create-system-user.sh && npm start"]