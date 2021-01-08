FROM debian:buster-slim

RUN apt-get update && apt-get install \
    cups \
    cups-filters \
    avahi-daemon \
    avahi-utils \
    cups-filters-core-drivers \
    colord \
    cups-bsd \
    cups-pdf \
    foomatic-db-compressed-ppds \
    openprinting-ppds \
    hp-ppd \
    hpijs-ppds -y

VOLUME ["./files", "/files"]

ENV CUPS_PASSWD=false \
    CUPS_LOGIN=root

COPY config/cupsd.conf /etc/cups/cupsd.conf
COPY config/avahi-daemon.conf /etc/avahi/avahi-daemon.conf

# RUN npm install -g dnssd

EXPOSE 631
EXPOSE 5353/udp

ADD init-cups.sh /init-cups.sh

CMD ["/init-cups.sh"]
