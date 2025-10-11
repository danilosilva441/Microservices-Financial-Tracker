#!/bin/sh
# Gera o arquivo final de configuração do Nginx a partir do template
envsubst < /etc/nginx/conf.d/nginx.template > /etc/nginx/conf.d/default.conf

# Inicia o nginx em primeiro plano
nginx -g 'daemon off;'
