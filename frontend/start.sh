#!/bin/sh
# Substitui as variáveis no nginx.conf e inicia o nginx
envsubst < /etc/nginx/conf.d/nginx.conf > /etc/nginx/conf.d/default.conf
nginx -g 'daemon off;'
