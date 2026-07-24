'use client';
import { FormEvent, useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { apiClient } from '@/shared/api/api-client';
import { useAppDispatch } from '@/store/hooks';
import { login } from '@/store/slices/auth-slice';

type AuthResponse = { accessToken: string; user: { name: string; email: string } };

export function AuthForm() {
  const [register, setRegister] = useState(false); const [error, setError] = useState(''); const [pending, setPending] = useState(false);
  const router = useRouter(); const params = useSearchParams(); const dispatch = useAppDispatch();
  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault(); setError(''); const data = new FormData(event.currentTarget);
    const email = String(data.get('email')); const password = String(data.get('password')); const name = String(data.get('name') || email.split('@')[0]);
    if (password.length < 12) return setError('La contraseña debe tener al menos 12 caracteres.');
    setPending(true);
    try {
      const response = await apiClient<AuthResponse>(`/auth/${register ? 'register' : 'login'}`, { method: 'POST', body: JSON.stringify(register ? { name, email, password } : { email, password }) });
      sessionStorage.setItem('access_token', response.accessToken); localStorage.setItem('lumina_user', JSON.stringify(response.user)); document.cookie = 'lumina_session=1; path=/; max-age=1800; samesite=lax'; dispatch(login(response.user)); router.replace(params.get('regresar') || '/productos');
    } catch (cause) { setError(cause instanceof Error ? cause.message : 'No fue posible iniciar sesión.'); } finally { setPending(false); }
  };
  return <section className="form-page"><div className="form-card"><p className="eyebrow">CUENTA LÚMINA</p><h1>{register ? 'Crea tu cuenta' : 'Qué gusto verte'}</h1><p>{register ? 'Guarda tus datos y administra tus direcciones.' : 'Identifícate para hacer pedidos y pagar de forma segura.'}</p><form onSubmit={submit}><label>Correo electrónico<input name="email" required type="email" placeholder="tu@correo.com" /></label>{register && <label>Nombre completo<input name="name" required placeholder="Tu nombre" /></label>}<label>Contraseña<input name="password" required type="password" minLength={12} placeholder="Mínimo 12 caracteres" /></label>{error && <p className="field-error">{error}</p>}<button className="button full" disabled={pending}>{pending ? 'Procesando…' : register ? 'Crear cuenta' : 'Ingresar'}</button></form><button className="text-button" onClick={() => { setError(''); setRegister(x => !x); }}>{register ? 'Ya tengo una cuenta' : 'Quiero registrarme'}</button></div></section>;
}
