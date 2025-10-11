# frontend/Dockerfile (JÁ ESTÁ CORRETO)

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

COPY --from=build /app/dist /usr/share/nginx/html

# Esta linha vai funcionar assim que você renomear o arquivo
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]