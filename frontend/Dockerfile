# --- ESTÁGIO 1: Build (Construção) ---
FROM node:20-alpine AS build
WORKDIR /app

# Copia os package.json e instala (o .dockerignore impede a node_modules local de vir)
COPY package*.json ./
RUN npm install

# Copia o resto do código
COPY . .

# Roda o build
RUN npm run build

# --- ESTÁGIO 2: Serve (Produção) ---
FROM nginx:1.25-alpine

# Copia o resultado do build para o Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Copia a configuração do Nginx para o Vue Router funcionar
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]