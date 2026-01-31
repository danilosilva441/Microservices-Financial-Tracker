# Dockerfile para frontend
FROM node:20-alpine as build

WORKDIR /app

# Copia package.json e instala dependências
COPY package*.json ./
RUN npm install

# Copia o código fonte
COPY . .

# Verifica se os arquivos essenciais existem
RUN echo "Verificando arquivos necessários..." && \
    ls -la src/ && \
    ls -la src/layouts/ || true && \
    ls -la src/components/ || true

# Build da aplicação
RUN npm run build

# --- ESTÁGIO 2: Serve ---
FROM nginx:1.25-alpine

COPY --from=build /app/dist /usr/share/nginx/html

# Esta linha agora vai encontrar o ficheiro nginx.conf e copiá-lo
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

RUN ls -la /etc/nginx/conf.d/