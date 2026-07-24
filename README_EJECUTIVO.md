# README ejecutivo

## Visión general

Esta solución combina un frontend en Next.js con una arquitectura de microservicios en ASP.NET Core 8 para ofrecer un flujo completo de comercio electrónico: catálogo de productos, autenticación, carrito, checkout y creación de pedidos.

## Qué ofrece la plataforma

- Registro e inicio de sesión con JWT.
- Catálogo de productos con disponibilidad real.
- Creación de pedidos con validación de inventario.
- Gateway unificado en `http://localhost:8080` para exponer los servicios.
- Separación de responsabilidades entre identidad, catálogo y órdenes.

## Estado actual

El flujo principal ya está validado end-to-end:

- El usuario puede autenticarse.
- El catálogo muestra productos y existencias.
- El backend crea órdenes y descuenta stock.
- El frontend refleja si un producto tiene disponibilidad o está agotado.

## Requisitos

- Docker Desktop
- Node.js 20+
- .NET 8 SDK (para desarrollar o extender el backend)

## Ejecución rápida

1. Levanta el backend con Docker Compose.
2. Ajusta el archivo [.env.example](.env.example) o crea `.env.local`.
3. Instala dependencias con `npm install`.
4. Inicia el frontend con `npm run dev`.

## Archivos clave

- [src/features/catalog/product-grid.tsx](src/features/catalog/product-grid.tsx)
- [src/features/auth/auth-form.tsx](src/features/auth/auth-form.tsx)
- [backend/src/IdentityService](backend/src/IdentityService)
- [backend/src/CatalogService](backend/src/CatalogService)
- [backend/src/OrderService](backend/src/OrderService)
