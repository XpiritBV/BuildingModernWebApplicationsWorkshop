### Build ###
FROM node:12.13.0-alpine AS build

WORKDIR /app

COPY RetroGamingSPA/. .

RUN npm install && npm run build

### Run ###
FROM nginx:alpine

COPY --from=build /app/dist/* /usr/share/nginx/html/