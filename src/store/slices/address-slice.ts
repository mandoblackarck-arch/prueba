import { createSlice, PayloadAction } from '@reduxjs/toolkit';
export type Address = { id: string; recipient: string; line1: string; city: string; postalCode: string };
const addressSlice = createSlice({ name: 'addresses', initialState: { items: [] as Address[], selectedId: '' }, reducers: {
  addAddress: (state, action: PayloadAction<Address>) => { state.items.push(action.payload); state.selectedId = action.payload.id; },
  selectAddress: (state, action: PayloadAction<string>) => { state.selectedId = action.payload; },
} });
export const { addAddress, selectAddress } = addressSlice.actions;
export default addressSlice.reducer;
