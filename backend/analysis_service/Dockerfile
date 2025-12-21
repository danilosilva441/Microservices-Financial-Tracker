FROM node:20-alpine

WORKDIR /app

# Instala dependências do sistema
RUN apk add --no-cache curl

# Copia arquivos de dependências
COPY package*.json ./

# Instala dependências da aplicação
RUN npm install

# Copia o código da aplicação
COPY . .

# Copia e configura o script (certifique-se do caminho correto)
COPY ./scripts/create-system-user.sh /app/create-system-user.sh



RUN chmod +x /app/create-system-user.sh

# Expõe a porta
EXPOSE 3000

# Comando de inicialização
CMD ["npm", "start"]