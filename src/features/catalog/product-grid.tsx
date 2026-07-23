'use client';
import { Product } from './product';
import { useAppDispatch } from '@/store/hooks';
import { addToCart } from '@/store/slices/cart-slice';
const money = new Intl.NumberFormat('es-MX', { style: 'currency', currency: 'MXN', maximumFractionDigits: 0 });
export function ProductGrid({ products }: { products: Product[] }) { const dispatch = useAppDispatch(); return <div className="product-grid">{products.map(product => <article className="product" key={product.id}><div className="product-image"><img src={product.image} alt={product.name} /></div><p className="eyebrow">{product.category}</p><div className="product-meta"><div><h3>{product.name}</h3><p>{money.format(product.price)}</p></div><button aria-label={'Agregar ' + product.name} onClick={() => dispatch(addToCart(product))}>+</button></div></article>)}</div>; }
