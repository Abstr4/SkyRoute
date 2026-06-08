export const CABIN_CLASSES = [
  { label: 'Economy', value: 'Economy' },
  { label: 'Business', value: 'Business' },
  { label: 'First Class', value: 'FirstClass' },
] as const;

export type CabinClass =
  typeof CABIN_CLASSES[number]['value'];

export const AIRPORTS = [
  { code: 'EZE', name: 'Buenos Aires, Argentina' },
  { code: 'COR', name: 'Córdoba, Argentina' },
  { code: 'MDZ', name: 'Mendoza, Argentina' },
  { code: 'GRU', name: 'São Paulo, Brazil' },
  { code: 'GIG', name: 'Rio de Janeiro, Brazil' },
  { code: 'SCL', name: 'Santiago, Chile' },
  { code: 'AEP', name: 'Buenos Aires, Argentina' },
  { code: 'LIM', name: 'Lima, Peru' },
] as const;