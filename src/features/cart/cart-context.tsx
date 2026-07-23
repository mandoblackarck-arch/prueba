'use client';
import { createContext, useContext, useMemo, useState } from 'react';
import { Product } from '@/features/catalog/product';
type CartItem = Product & { quantity: number };
type Cart = { items: CartItem[]; count: number; total: number; add: (p: Product) => void; remove: (id: string) => void; clear: () => void };
const CartContext = createContext<Cart | null>(null);
export function CartProvider({ children }: { children: React.ReactNode }) { const [items, setItems] = useState<CartItem[]>([]); const value = useMemo(() => ({ items, count: items.reduce((n, x) => n + x.quantity, 0), total: items.reduce((n, x) => n + x.price * x.quantity, 0), add: (p: Product) => setItems(old => { const exists = old.find(x => x.id === p.id); return exists ? old.map(x => x.id === p.id ? { ...x, quantity: x.quantity + 1 } : x) : [...old, { ...p, quantity: 1 }]; }), remove: (id: string) => setItems(old => old.filter(x => x.id !== id)), clear: () => setItems([]) }), [items]); return <CartContext.Provider value={value}>{children}</CartContext.Provider>; }
export function useCart() { const value = useContext(CartContext); if (!value) throw new Error('useCart debe usarse dentro de CartProvider'); return value; }
