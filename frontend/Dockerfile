# --- ESTÁGIO 1: Build (Construção) ---
FROM node:20-slim AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

# ✅ Evita erro de permissão com o Vite no Railway
RUN node node_modules/vite/bin/vite.js build

# --- ESTÁGIO 2: Serve (Produção) ---
FROM nginx:1.25-alpine

# Copia os arquivos gerados pelo Vite (Vue 3)
COPY --from=build /app/dist /usr/share/nginx/html

# Copia a configuração customizada do Nginx (importante para rotas do Vue)
COPY nginx.template /etc/nginx/conf.d/default.conf

EXPOSE 80
COPY start.sh /start.sh
RUN chmod +x /start.sh
CMD ["/start.sh"]


