'use client';
import Link from 'next/link';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { logout } from '@/store/slices/auth-slice';

export function Header() {
  const count = useAppSelector(s => s.cart.items.reduce((n, item) => n + item.quantity, 0));
  const user = useAppSelector(s => s.auth.user);
  const dispatch = useAppDispatch();
  const signOut = () => { dispatch(logout()); document.cookie = 'lumina_session=; path=/; max-age=0; samesite=lax'; };
  return <header style={{ position: 'fixed', top: 0, left: 0, zIndex: 10, width: '100%' }}><Link className="brand" href="/">LÚMINA</Link><nav><Link href="/productos">Productos</Link>{user && <Link href="/pedido">Rastrear pedido</Link>}{user ? <><Link href="/direcciones">Direcciones</Link><button className="nav-button" onClick={signOut}>Salir</button></> : <Link href="/acceso">Ingresar</Link>}<Link className="cart-link" href="/carrito">Bolsa <span>{count}</span></Link></nav></header>;
}
