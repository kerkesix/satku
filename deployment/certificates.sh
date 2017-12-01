# Install certbot: brew install certbot

# Renew certs on a machine that was used the last time: 
# certbot renew

# ^This unfortunately does not work on manual mode, so you might need to run the full 
# creation from scratch as below:

# To do the whole thing from scratch do
# sudo certbot certonly --manual --preferred-challenges dns -d satku.kerkesix.fi,ilmo.kerkesix.fi

# Finally convert the certificates to proper format
# sudo su
# cd /etc/letsencrypt/live/satku.kerkesix.fi
# openssl pkcs12 -export -out satku.pfx -inkey privkey.pem -in cert.pem -certfile chain.pem