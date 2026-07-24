const baseUrl = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:8080/api';

export class ApiError extends Error {
  constructor(public readonly status: number, message: string) { super(message); }
}

/** Cliente HTTP único: concentra URL, JSON, errores y bearer token. */
export async function apiClient<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = typeof window === 'undefined' ? null : sessionStorage.getItem('access_token');
  const response = await fetch(`${baseUrl}${path}`, { ...options, headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}), ...options.headers } });
  if (!response.ok) {
    const body = await response.json().catch(() => null) as { message?: string; detail?: string } | null;
    throw new ApiError(response.status, body?.message ?? body?.detail ?? `La solicitud falló (${response.status})`);
  }
  return response.status === 204 ? (undefined as T) : response.json() as Promise<T>;
}
