# Multi-stage build dla React Frontend
# Stage 1: Build
FROM node:18-alpine AS build

WORKDIR /app

# Kopiuj pliki package
COPY package*.json ./

# Zainstaluj zależności
RUN npm ci

# Kopiuj kod źródłowy
COPY . .

# Zbuduj aplikację w production mode
# Vite automatycznie używa production mode gdy NODE_ENV=production
ENV NODE_ENV=production
RUN npm run build

# Stage 2: Production - Nginx
FROM nginx:alpine

# Skopiuj zbudowane pliki z stage 1
COPY --from=build /app/dist /usr/share/nginx/html

# Skopiuj konfigurację nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 80
EXPOSE 80

# Nginx startuje automatycznie
CMD ["nginx", "-g", "daemon off;"]
