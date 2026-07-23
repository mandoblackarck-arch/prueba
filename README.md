# Comercio Web

Interfaz de e-commerce con Next.js (App Router) preparada para un backend de ASP.NET Core Minimal APIs.

## Ejecutar

1. Copia `.env.example` a `.env.local` y ajusta `NEXT_PUBLIC_API_URL`.
2. `npm install`
3. `npm run dev`

## Contratos de API esperados

- `POST /auth/login`, `POST /auth/register`
- `GET /products`, `GET /products/{id}`
- `POST /orders` (autenticado), `GET /orders/{number}/status` (público)
- `POST /payments/simulate` (autenticado)
- `POST /events` (autenticado; auditoría de acciones)

Los servicios están aislados en `src/features/*/services`; cambia sus implementaciones de demostración por llamadas HTTP al conectar el backend.
