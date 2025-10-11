# nginx/Dockerfile (versão final)

FROM nginx:1.25-alpine

# Remove a configuração padrão do Nginx
RUN rm /etc/nginx/conf.d/default.conf

# Copia nosso arquivo de configuração final para o lugar certo
COPY nginx.conf /etc/nginx/conf.d/default.conf