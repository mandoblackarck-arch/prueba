import Link from 'next/link';
import { ProductGrid } from '@/features/catalog/product-grid';
import { catalogService } from '@/features/catalog/catalog.service';

export default async function HomePage() {
  const products = await catalogService.getFeatured();
  return <><section className="hero"><div><p className="eyebrow">NUEVA COLECCIÓN</p><h1>Objetos que hacen<br />más tuyo tu espacio.</h1><p>Diseño cotidiano, materiales nobles y detalles que importan.</p><Link className="button" href="#productos">Ver productos</Link></div><div className="hero-art">✦</div></section><section id="productos" className="section"><div className="section-heading"><div><p className="eyebrow">SELECCIÓN</p><h2>Favoritos de la semana</h2></div><Link href="/productos">Ver todo →</Link></div><ProductGrid products={products} /></section><section className="benefits"><div><b>Envío sin costo</b><span>En compras mayores a $1,500</span></div><div><b>Pago protegido</b><span>Simulación segura para tu demo</span></div><div><b>Atención cercana</b><span>Estamos para ayudarte</span></div></section></>;
}
