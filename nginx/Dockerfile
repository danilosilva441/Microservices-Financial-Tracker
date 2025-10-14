FROM nginx:1.25-alpine

COPY nginx.template /etc/nginx/conf.d/nginx.template
COPY start.sh /start.sh

RUN chmod +x /start.sh
#sdsd
EXPOSE 80
CMD ["/start.sh"]
