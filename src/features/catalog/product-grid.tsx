'use client';
import { Product } from './product';
import { useAppDispatch } from '@/store/hooks';
import { addToCart } from '@/store/slices/cart-slice';
const money = new Intl.NumberFormat('es-MX', { style: 'currency', currency: 'MXN', maximumFractionDigits: 0 });

export function ProductGrid({ products }: { products: Product[] }) {
  const dispatch = useAppDispatch();

  return (
    <div className="product-grid">
      {products.map((product) => {
        const hasStock = product.existencia > 0;
        const stockLabel = hasStock ? `${product.existencia} disponibles` : 'Sin stock';

        return (
          <article className="product" key={product.id} style={{ opacity: hasStock ? 1 : 0.7 }}>
            <div className="product-image">
              <img src={product.image} alt={product.name} />
            </div>
            <p className="eyebrow">{product.category}</p>
            <div className="product-meta">
              <div>
                <h3>{product.name}</h3>
                <p>{money.format(product.price)}</p>
                <p style={{ fontSize: '0.9rem', color: hasStock ? '#2f6b3b' : '#8a1f1f', marginTop: '0.25rem' }}>{stockLabel}</p>
              </div>
              <button
                aria-label={'Agregar ' + product.name}
                onClick={() => hasStock && dispatch(addToCart(product))}
                disabled={!hasStock}
                style={{ opacity: hasStock ? 1 : 0.55, cursor: hasStock ? 'pointer' : 'not-allowed' }}
              >
                {hasStock ? '+' : '•'}
              </button>
            </div>
          </article>
        );
      })}
    </div>
  );
}
