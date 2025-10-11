# --- ESTÁGIO 1: Build (Construção) ---
FROM node:20-alpine AS build

WORKDIR /app

# Ignora a node_modules local ao copiar (garantido pelo .dockerignore)
COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# --- ESTÁGIO 2: Serve (Produção) ---
FROM nginx:1.25-alpine

# Copia os arquivos estáticos gerados no estágio de 'build'
COPY --from=build /app/dist /usr/share/nginx/html

# --- CORREÇÃO AQUI ---
# Copia o nosso nginx.template para o local correto, RENOMEANDO para default.conf
COPY nginx.template /etc/nginx/conf.d/default.conf

EXPOSE 80

# Substitui variáveis de ambiente e inicia o Nginx
CMD envsubst '$AUTH_SERVICE_HOST $AUTH_SERVICE_PORT $BILLING_SERVICE_HOST $BILLING_SERVICE_PORT $ANALYSIS_SERVICE_HOST $ANALYSIS_SERVICE_PORT $FRONTEND_HOST $FRONTEND_PORT' \
  < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf \
  && nginx -g 'daemon off;'
    

  