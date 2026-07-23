import type { Metadata } from 'next';
import 'animate.css';
import { ReduxProvider } from '@/store/provider';
import { Header } from '@/components/header';
import './globals.css';
export const metadata: Metadata = { title: 'Lúmina · Tienda', description: 'Compra simple y segura.' };
export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) { return <html lang="es"><body><ReduxProvider><Header /><main style={{ paddingTop: 76 }}>{children}</main></ReduxProvider></body></html>; }
