FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY tsconfig*.json vite.config.ts index.html ./
COPY src ./src
COPY .env.example ./

ARG VITE_GRPC_HOST
ENV VITE_GRPC_HOST=${VITE_GRPC_HOST}
RUN npm run build

# Serve static assets with nginx
FROM nginx:1.25-alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html

RUN printf 'server {\n  listen 80;\n  server_name _;\n  root /usr/share/nginx/html;\n  include /etc/nginx/mime.types;\n  location / {\n    try_files $uri $uri/ /index.html;\n  }\n}\n' > /etc/nginx/conf.d/default.conf

EXPOSE 80
