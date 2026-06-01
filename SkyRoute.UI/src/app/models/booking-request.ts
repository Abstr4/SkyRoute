export interface BookingPassenger {
  fullName: string;
  email: string;
  documentType: string;
  documentNumber: string;
}

export interface CreateBookingRequest {
  provider: string;
  flightNumber: string;
  passengers: BookingPassenger[];
}
