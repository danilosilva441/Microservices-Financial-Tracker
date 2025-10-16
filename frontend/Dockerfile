# --- ESTÁGIO 1: Build ---
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# --- ESTÁGIO 2: Serve ---
FROM nginx:1.25-alpine

COPY --from=build /app/dist /usr/share/nginx/html

# Esta linha agora vai encontrar o ficheiro nginx.conf e copiá-lo
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

RUN ls -la /etc/nginx/conf.d/