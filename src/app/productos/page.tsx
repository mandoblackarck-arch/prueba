import { ProductGrid } from '@/features/catalog/product-grid';
import { catalogService } from '@/features/catalog/catalog.service';
export default async function ProductsPage() { return <section className="section page"><p className="eyebrow">CATÁLOGO</p><h1>Todo para disfrutar tu casa</h1><ProductGrid products={await catalogService.getAll()} /></section>; }
