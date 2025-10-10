FROM nginx:1.25-alpine

# Copia template
COPY nginx.template /etc/nginx/templates/default.conf.template

# Instala envsubst
RUN apk add --no-cache gettext

# Porta
EXPOSE 80

# Substitui variáveis e inicia nginx
CMD envsubst '$AUTH_SERVICE_HOST $AUTH_SERVICE_PORT $BILLING_SERVICE_HOST $BILLING_SERVICE_PORT $ANALYSIS_SERVICE_HOST $ANALYSIS_SERVICE_PORT $FRONTEND_HOST $FRONTEND_PORT' \
    < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'
