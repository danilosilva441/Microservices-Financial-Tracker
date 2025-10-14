# nginx/Dockerfile (versão final)
FROM nginx:1.25-alpine

# Instala as ferramentas necessárias (bash para o script e gettext para o envsubst)
RUN apk add --no-cache bash gettext

# Remove a configuração padrão do Nginx
RUN rm /etc/nginx/conf.d/default.conf

# Copia o nosso "molde" e o script de inicialização
COPY nginx.template /etc/nginx/templates/default.conf.template
COPY start.sh /start.sh

# Dá permissão de execução ao script
RUN chmod +x /start.sh

EXPOSE 80

# Comando para iniciar o contentor: executa o nosso script
CMD ["/start.sh"]