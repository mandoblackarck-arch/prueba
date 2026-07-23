import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';
export function middleware(request: NextRequest) { if (!request.cookies.get('lumina_session')) { const url = request.nextUrl.clone(); url.pathname = '/acceso'; url.searchParams.set('regresar', request.nextUrl.pathname); return NextResponse.redirect(url); } return NextResponse.next(); }
export const config = { matcher: ['/checkout/:path*', '/direcciones/:path*'] };
