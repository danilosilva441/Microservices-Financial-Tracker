# --- ESTÁGIO 1: Build (Construção) ---
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
RUN chmod +x /app/node_modules/.bin/vite
COPY . .
RUN npm run build

# --- ESTÁGIO 2: Serve (Produção) ---
FROM nginx:1.25-alpine

# Copia os arquivos estáticos gerados no estágio de 'build'
COPY --from=build /app/dist /usr/share/nginx/html

# --- CORREÇÃO AQUI ---
# Copia nosso nginx.conf para o local correto, RENOMEANDO para default.conf
COPY nginx.template /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]