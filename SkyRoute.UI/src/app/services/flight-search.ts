import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FlightSearchRequest, FlightOffer } from '../models';

@Injectable({
  providedIn: 'root',
})
export class FlightSearchService {
  private apiUrl = 'https://localhost:7229/api/Flights';
  private http = inject(HttpClient);

  searchFlights(request: FlightSearchRequest): Observable<FlightOffer[]> {
    const params = new HttpParams()
      .set('originAirportCode', request.originAirportCode)
      .set('destinationAirportCode', request.destinationAirportCode)
      .set('departureDate', request.departureDate)
      .set('passengers', request.passengers)
      .set('cabinClass', request.cabinClass)
      .set('timeZone', request.timeZone);
    return this.http.get<FlightOffer[]>(this.apiUrl, { params });
  }
}
