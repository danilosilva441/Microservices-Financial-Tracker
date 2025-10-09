# Usa a imagem oficial do Nginx
FROM nginx:1.25-alpine

# Copia nosso arquivo de template para uma pasta dentro do contêiner
COPY nginx.template /etc/nginx/templates/default.conf.template

# O Nginx Alpine não vem com o 'envsubst', então instalamos
RUN apk add --no-cache gettext

# Expõe a porta
EXPOSE 80

# Comando de inicialização: roda um script que primeiro processa o template
# e depois inicia o Nginx.
CMD ["/bin/sh", "-c", "envsubst < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]