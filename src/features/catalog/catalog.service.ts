import { apiClient } from '@/shared/api/api-client';
import { Product } from './product';

const products: Product[] = [
  { id: 'vela-ambar', name: 'Vela Ámbar', price: 480, existencia: 12, category: 'Aromas', image: 'https://images.unsplash.com/photo-1603006905003-be475563bc59?auto=format&fit=crop&w=700&q=85', description: 'Cera vegetal y notas cálidas de ámbar.' },
  { id: 'taza-arena', name: 'Taza Arena', price: 290, existencia: 8, category: 'Mesa', image: 'https://images.unsplash.com/photo-1514228742587-6b1558fcca3d?auto=format&fit=crop&w=700&q=85', description: 'Cerámica esmaltada hecha para las pausas largas.' },
];

export const catalogService = {
  getAll: async (): Promise<Product[]> => apiClient<Product[]>('/products').catch(() => products),
  getFeatured: async (): Promise<Product[]> => (await apiClient<Product[]>('/products').catch(() => products)).slice(0, 4),
};
