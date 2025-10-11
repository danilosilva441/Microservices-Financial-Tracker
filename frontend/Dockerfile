# --- ESTÁGIO 1: Build (Vue/Vite) ---
FROM node:20-slim AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

RUN node node_modules/vite/bin/vite.js build

# --- ESTÁGIO 2: Produção (Nginx) ---
FROM nginx:1.25-alpine

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.template /etc/nginx/conf.d/nginx.template
COPY start.sh /start.sh

RUN chmod +x /start.sh

EXPOSE 80

CMD ["/start.sh"]
