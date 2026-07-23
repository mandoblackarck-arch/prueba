'use client';
import { FormEvent, useState } from 'react';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { addAddress, selectAddress } from '@/store/slices/address-slice';

const addressPattern = /^(?=.*[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]{3,})(?=.*\d+)(?=.*[,\s])[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9.,#\-\s]+$/;

export function AddressManager() {
  const items = useAppSelector(s => s.addresses.items);
  const selectedId = useAppSelector(s => s.addresses.selectedId);
  const dispatch = useAppDispatch();
  const [message, setMessage] = useState('');

  const submit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const data = new FormData(event.currentTarget);
    const address = {
      id: crypto.randomUUID(), recipient: String(data.get('recipient')),
      line1: String(data.get('line1')).trim(), city: String(data.get('city')),
      postalCode: String(data.get('postalCode')),
    };
    if (!/^\d{5}$/.test(address.postalCode)) return setMessage('El código postal debe tener 5 dígitos.');
    if (!addressPattern.test(address.line1)) return setMessage('Escribe calle, número y colonia. Ejemplo: Av. Reforma 123, Juárez.');
    dispatch(addAddress(address));
    setMessage('Dirección guardada y seleccionada para tu próxima compra.');
    event.currentTarget.reset();
  };

  return <section className="section page"><p className="eyebrow">MI CUENTA</p><h1>Direcciones de entrega</h1><div className="address-layout"><div>{items.length ? items.map(address => <label className="address-card" key={address.id}><input type="radio" checked={selectedId === address.id} onChange={() => dispatch(selectAddress(address.id))} /><span><b>{address.recipient}</b><br />{address.line1}<br />{address.city}, C.P. {address.postalCode}</span></label>) : <p>Aún no tienes direcciones guardadas.</p>}</div><form className="address-form" onSubmit={submit}><h2>Agregar dirección</h2><label>Nombre completo<input name="recipient" required /></label><label>Dirección<input name="line1" required minLength={10} placeholder="Calle, número y colonia" title="Incluye calle, número y colonia. Ejemplo: Av. Reforma 123, Juárez" /></label><div className="two-cols"><label>Ciudad<input name="city" required /></label><label>Código postal<input name="postalCode" required inputMode="numeric" pattern="\d{5}" placeholder="00000" /></label></div>{message && <p className="notice">{message}</p>}<button className="button">Guardar dirección</button></form></div></section>;
}
