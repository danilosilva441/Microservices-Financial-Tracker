# --- ESTÁGIO 1: Build (Construção) ---
FROM node:20-alpine AS build
WORKDIR /app

# Copia os arquivos de dependência e instala (sem a sua node_modules local)
COPY package*.json ./
RUN npm install

# Copia o resto do código do seu projeto
COPY . .

# Roda o comando de build do Vite
RUN npm run build

# --- ESTÁGIO 2: Serve (Produção) ---
FROM nginx:1.25-alpine

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]