import { apiClient } from '@/shared/api/api-client';

export type UserEvent = 'product_added' | 'checkout_started' | 'order_created' | 'login_succeeded' | 'registration_succeeded';

/**
 * Auditoría de escritura solamente. La autorización y la consulta de la
 * bitácora pertenecen al backend; nunca se expone una ruta de lectura al cliente.
 */
export const eventTracker = {
  track: (event: UserEvent, metadata: Record<string, string | number> = {}) =>
    apiClient<void>('/events', { method: 'POST', body: JSON.stringify({ event, metadata, occurredAt: new Date().toISOString() }) }).catch(() => undefined),
};
