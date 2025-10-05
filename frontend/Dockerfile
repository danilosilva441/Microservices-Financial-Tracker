# --- ESTÁGIO 1: Build (Construção) ---
# Usamos uma imagem Node.js para construir o projeto Vue
FROM node:20-alpine AS build

# Define o diretório de trabalho
WORKDIR /app

# Copia os arquivos de dependência e instala
COPY package*.json ./
RUN npm install

# Copia todo o resto do código do frontend
COPY . .

# Roda o comando de build do Vite, que gera a pasta /dist
RUN npm run build

# --- ESTÁGIO 2: Serve (Produção) ---
# Usamos uma imagem super leve do Nginx para servir os arquivos
FROM nginx:1.25-alpine

# Copia os arquivos estáticos gerados no estágio de 'build' 
# para a pasta padrão do Nginx que serve conteúdo web
COPY --from=build /app/dist /usr/share/nginx/html

# Copia a configuração customizada do Nginx para SPAs (Single-Page Applications)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expõe a porta 80 (padrão do Nginx)
EXPOSE 80

# Comando para iniciar o Nginx
CMD ["nginx", "-g", "daemon off;"]