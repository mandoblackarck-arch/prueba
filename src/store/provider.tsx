'use client';
import { PropsWithChildren, useEffect } from 'react';
import { Provider } from 'react-redux';
import { store } from './store';
import { restoreSession } from './slices/auth-slice';
export function ReduxProvider({ children }: PropsWithChildren) { useEffect(() => { store.dispatch(restoreSession()); }, []); return <Provider store={store}>{children}</Provider>; }
