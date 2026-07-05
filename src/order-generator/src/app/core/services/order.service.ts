import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Order, OrderResponse } from '../models/order.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private readonly apiUrl = `${environment.urlBase}/api/orders`;

  constructor(private http: HttpClient) {}

  sendOrder(order: Order): Observable<OrderResponse> {
    return this.http.post<OrderResponse>(this.apiUrl, order);
  }
}
