import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { Product } from '@/features/catalog/product';
type CartItem = Product & { quantity: number };
const cartSlice = createSlice({ name: 'cart', initialState: { items: [] as CartItem[] }, reducers: {
  addToCart: (state, action: PayloadAction<Product>) => { const item = state.items.find(x => x.id === action.payload.id); if (item) item.quantity += 1; else state.items.push({ ...action.payload, quantity: 1 }); },
  removeFromCart: (state, action: PayloadAction<string>) => { state.items = state.items.filter(x => x.id !== action.payload); },
  clearCart: state => { state.items = []; },
} });
export const { addToCart, removeFromCart, clearCart } = cartSlice.actions;
export default cartSlice.reducer;
