# --- ESTÁGIO 1: Build (Vue/Vite) ---
FROM node:20-slim AS build

WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN node node_modules/vite/bin/vite.js build

# --- ESTÁGIO 2: Servir com Nginx padrão ---
FROM nginx:1.25-alpine

# Copia o build do Vite
COPY --from=build /app/dist /usr/share/nginx/html

# Substitui o default.conf por um básico, se quiser
RUN rm /etc/nginx/conf.d/default.conf

# (Opcional) cria um default.conf simples
RUN echo 'server { listen 80; root /usr/share/nginx/html; index index.html; try_files $uri /index.html; }' \
    > /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
