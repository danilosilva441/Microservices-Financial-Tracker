# Usa a imagem oficial e leve do Nginx
FROM nginx:1.25-alpine

# Remove a configuração padrão do Nginx que vem na imagem
RUN rm /etc/nginx/conf.d/default.conf

# Copia o nosso arquivo de configuração personalizado para o lugar certo dentro do contêiner
COPY nginx.conf /etc/nginx/conf.d/default.conf