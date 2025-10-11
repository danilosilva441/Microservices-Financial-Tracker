# --- ESTÁGIO 1: Build (Construção) ---
FROM node:20-alpine AS build

WORKDIR /app

# Copia TODOS os arquivos do projeto primeiro
# O .dockerignore garante que a node_modules local não seja copiada
COPY . .

# Agora que todos os arquivos estão aqui, incluindo package.json, instala as dependências
RUN npm install

# Roda o comando de build do Vite
RUN npm run build

# --- ESTÁGIO 2: Serve (Produção) ---
FROM nginx:1.25-alpine

# Copia apenas o resultado do build (a pasta /dist) para o servidor Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Copia a configuração do Nginx para que o Vue Router funcione
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]