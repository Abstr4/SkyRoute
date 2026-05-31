export interface FlightSearchRequest {
  originAirportCode: string;
  destinationAirportCode: string;
  departureDate: string; // ISO format date string (e.g., "2024-12-31")
  passengers: number;
  cabinClass: 'Economy' | 'Business' | 'FirstClass';
  timeZone: string;
}