# Usa a imagem oficial e leve do Nginx
FROM nginx:1.25-alpine

# O Nginx Alpine não vem com o 'envsubst', então o instalamos
RUN apk add --no-cache gettext

# Copia nosso arquivo de template para uma pasta dentro do contêiner
COPY nginx.template /etc/nginx/templates/default.conf.template

# Expõe a porta padrão
EXPOSE 80

# Comando de inicialização:
# 1. Lê o template, substitui as variáveis ($...) e cria o arquivo .conf final.
# 2. Em seguida (&&), inicia o Nginx com a configuração gerada.
CMD ["/bin/sh", "-c", "envsubst < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]