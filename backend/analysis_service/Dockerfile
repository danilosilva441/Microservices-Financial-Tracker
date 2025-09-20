# Usa uma imagem base oficial do Node.js, leve e otimizada
FROM node:18-alpine

# Define o diretório de trabalho dentro do contêiner
WORKDIR /app

# Copia os arquivos de dependência primeiro para aproveitar o cache do Docker
COPY package*.json ./

# Instala as dependências do projeto
RUN npm install

# Copia o restante do código da aplicação
COPY . .

# Expõe a porta em que o nosso servidor Express está rodando
EXPOSE 3000

# Comando para iniciar a aplicação quando o contêiner subir
CMD [ "node", "index.js" ]