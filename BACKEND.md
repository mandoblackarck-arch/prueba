# Backend de Comercio

El sistema tiene tres microservicios ASP.NET Core 8: `IdentityService` (registro, login y JWT), `CatalogService` (consulta de productos) y `OrderService` (pedidos protegidos). Cada uno tiene su propia base SQL Server y Swagger en `/swagger`.

## Ejecución local

1. Copia `.env.docker.example` a `.env` y reemplaza sus valores por secretos seguros (la clave JWT debe tener 32 caracteres como mínimo).
2. Ejecuta `docker compose up --build`.
3. Configura `NEXT_PUBLIC_API_URL=http://localhost:8080/api` y ejecuta el frontend con `npm run dev`.

## url de swagger 
   - http://localhost:8080/swagger/auth/
   - http://localhost:8080/swagger/products/
   - http://localhost:8080/swagger/orders/

El gateway queda en `http://localhost:8080`; por ejemplo, catálogo: `GET /api/products`; identidad: `POST /api/auth/register` y `POST /api/auth/login`; pedidos autenticados: `POST /api/orders`.

## La URL de conexión a SQL Server es:
- Server=localhost,1433;User Id=sa;Password=Use_A_Strong_SQL_Password_2026!;
## Seguridad y operación

- Contraseñas con PBKDF2-SHA512, 310 000 iteraciones y salt aleatorio; nunca se almacenan en texto plano.
- JWT de 30 minutos con validación de emisor, audiencia, firma y expiración. Usar un vault/secret manager en despliegues reales.
- Validación por Data Annotations, respuestas Problem Details para excepciones y CORS limitado al frontend local.
- `EnsureCreated` permite el arranque reproducible de este entorno inicial. Antes de producción, generar y aplicar migraciones EF Core versionadas (`dotnet ef migrations add InitialCreate`) mediante una etapa de despliegue controlada.
- CI construye frontend, ejecuta pruebas y detecta vulnerabilidades de paquetes. Añade análisis SAST, escaneo de imagen y secretos en el proveedor de CI antes de liberar.

No se debe confiar en precio ni disponibilidad enviados por el navegador: el siguiente incremento debe hacer que `OrderService` consulte un contrato interno de catálogo y calcule precios del lado servidor.
