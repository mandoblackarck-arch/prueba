# Comercio Web

Aplicación de comercio electrónico full-stack con Next.js y microservicios ASP.NET Core 8. El frontend ahora refleja el estado real del catálogo, incluyendo la disponibilidad del inventario y el bloqueo de productos sin stock.

## Resumen ejecutivo

- El usuario puede registrarse e iniciar sesión desde la interfaz.
- El catálogo consume los productos desde el gateway del backend en `http://localhost:8080`.
- El carrito y el checkout están preparados para operar sobre pedidos reales con autenticación JWT.
- La disponibilidad de inventario se muestra en tiempo real en la UI, evitando agregar productos agotados.

## Inicio rápido

1. Levanta los servicios del backend con Docker Compose siguiendo [BACKEND.md](BACKEND.md).
2. Copia [.env.example](.env.example) a `.env.local` y ajusta `NEXT_PUBLIC_API_URL` si es necesario.
3. Instala dependencias con `npm install`.
4. Inicia el frontend con `npm run dev`.

## Documentación adicional

- README ejecutivo: [README_EJECUTIVO.md](README_EJECUTIVO.md)
- Arquitectura: [ARCHITECTURE.md](ARCHITECTURE.md)
