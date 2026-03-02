FROM alpine:latest

RUN apk add --no-cache vsftpd shadow

RUN adduser -D -s /sbin/nologin sixgrainftp

RUN echo 'sixgrainftp:MnOpQrStUvWx' | chpasswd

COPY ./Deploy/ftp/vsftpd.conf /etc/vsftpd/vsftpd.conf

RUN mkdir -p /home/sixgrainftp/ftp && \
    chown sixgrainftp:sixgrainftp /home/sixgrainftp && \
    chmod 755 /home/sixgrainftp

EXPOSE 20 21 21100-21110

CMD ["vsftpd", "-N", "/etc/vsftpd/vsftpd.conf"]