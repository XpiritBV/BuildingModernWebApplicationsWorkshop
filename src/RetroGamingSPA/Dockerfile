### Build ###
FROM node:alpine AS build

WORKDIR /app

COPY RetroGamingSPA/. .

RUN npm install && npm run build

### Run ###
FROM nginx:alpine

COPY --from=build /app/dist/* /usr/share/nginx/html/