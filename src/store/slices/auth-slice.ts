import { createSlice, PayloadAction } from '@reduxjs/toolkit';
export type User = { name: string; email: string };
type AuthState = { user: User | null; hydrated: boolean };
const authSlice = createSlice({ name: 'auth', initialState: { user: null, hydrated: false } as AuthState, reducers: {
  login: (state, action: PayloadAction<User>) => { state.user = action.payload; state.hydrated = true; },
  logout: state => { state.user = null; state.hydrated = true; if (typeof window !== 'undefined') localStorage.removeItem('lumina_user'); },
  restoreSession: state => { state.hydrated = true; if (typeof window !== 'undefined') { const saved = localStorage.getItem('lumina_user'); if (saved) state.user = JSON.parse(saved); } },
} });
export const { login, logout, restoreSession } = authSlice.actions;
export default authSlice.reducer;
