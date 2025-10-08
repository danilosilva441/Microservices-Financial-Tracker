# --- ESTÁGIO 1: Build (Construção) ---
FROM node:20-slim AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

# 🔧 Corrige permissões do Vite (evita "Permission denied")
RUN chmod +x node_modules/.bin/vite

COPY . .

# 🔧 Usa o Vite via npx (garante execução mesmo sem PATH)
RUN npx vite build

# --- ESTÁGIO 2: Serve (Produção) ---
FROM nginx:1.25-alpine

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
