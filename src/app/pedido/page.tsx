import { Suspense } from 'react';
import { OrderLookup } from '@/features/orders/order-lookup';
export default function Page() { return <Suspense fallback={null}><OrderLookup /></Suspense>; }
