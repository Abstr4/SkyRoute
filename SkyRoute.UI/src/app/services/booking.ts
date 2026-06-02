import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateBookingRequest, CreateBookingResponse } from '../models';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private readonly apiUrl = `${environment.apiUrl}/api/Booking`;
  private readonly http = inject(HttpClient);

  public confirmBooking(request: CreateBookingRequest): Observable<CreateBookingResponse> {
    return this.http.post<CreateBookingResponse>(this.apiUrl, request);
  }
}
