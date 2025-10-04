# Usa uma imagem base oficial e leve do Node.js
FROM node:18-alpine

# Define o diretório de trabalho dentro do contêiner
WORKDIR /app

# Instala o 'curl', necessário para o healthcheck do Docker Compose funcionar
RUN apk add --no-cache curl

# Copia os arquivos de dependência primeiro para aproveitar o cache do Docker
COPY package*.json ./

# Instala as dependências do projeto
RUN npm install

# Copia todo o restante do código da aplicação (a pasta 'src')
COPY . .

# Expõe a porta que nosso serviço usará para o healthcheck
EXPOSE 3000

# Comando para iniciar a aplicação quando o contêiner subir
CMD [ "npm", "start" ]