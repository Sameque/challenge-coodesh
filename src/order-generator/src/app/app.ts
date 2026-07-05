import { Component, signal } from '@angular/core';
import { OrderFormComponent } from './features/orders/components/order-form/order-form.component';

@Component({
  selector: 'app-root',
  imports: [OrderFormComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('order-generator');
}
