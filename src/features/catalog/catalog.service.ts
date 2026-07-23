import { Product } from './product';

const products: Product[] = [
  { id: 'vela-ambar', name: 'Vela Ámbar', price: 480, category: 'Aromas', image: 'https://images.unsplash.com/photo-1603006905003-be475563bc59?auto=format&fit=crop&w=700&q=85', description: 'Cera vegetal y notas cálidas de ámbar.' },
  { id: 'taza-arena', name: 'Taza Arena', price: 290, category: 'Mesa', image: 'https://images.unsplash.com/photo-1514228742587-6b1558fcca3d?auto=format&fit=crop&w=700&q=85', description: 'Cerámica esmaltada hecha para las pausas largas.' },
  { id: 'florero-oliva', name: 'Florero Oliva', price: 760, category: 'Decoración', image: 'https://images.unsplash.com/photo-1618220179428-22790b461013?auto=format&fit=crop&w=700&q=85', description: 'Vidrio soplado de silueta orgánica.' },
  { id: 'manta-lino', name: 'Manta de lino', price: 1250, category: 'Textiles', image: 'https://images.unsplash.com/photo-1584100936595-c0654b55a2e2?auto=format&fit=crop&w=700&q=85', description: 'Ligera, suave y tejida para durar.' },
];

// Único punto de acceso al catálogo. Sustituye la demo por fetch(`${API_URL}/products`).
export const catalogService = { getAll: async () => products, getFeatured: async () => products.slice(0, 4) };
