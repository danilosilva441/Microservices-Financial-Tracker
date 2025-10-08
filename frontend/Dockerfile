# --- ESTÁGIO 1: Build (Construção) ---
# CORREÇÃO: Usa a imagem 'slim' que é mais robusta com permissões
FROM node:20-slim AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

# A linha 'RUN chmod +x ...' foi REMOVIDA

COPY . .

RUN npm run build

# --- ESTÁGIO 2: Serve (Produção) ---
# (O resto do arquivo continua igual)
FROM nginx:1.25-alpine

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]