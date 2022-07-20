#SEQ

docker run  --name seq  -d --restart unless-stopped -e ACCEPT_EULA=Y -v data:/data -p 8090:80 -p 5341:5341 datalust/seq

