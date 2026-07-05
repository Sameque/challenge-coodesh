import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Asset } from '../models/asset.model';

@Injectable({
  providedIn: 'root'
})
export class AssetService {
  private mockAssets: Asset[] = [
    { ticker: 'PETR4', name: 'Petrobras' },
    { ticker: 'VALE3', name: 'Vale' },
    { ticker: 'VIIA4', name: 'Via' }
  ];

  getAssets(): Observable<Asset[]> {
    return of(this.mockAssets);
  }
}
