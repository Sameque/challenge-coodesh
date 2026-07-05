export interface Order {
  ticker: string;
  side: 'Compra' | 'Venda';
  quantity: number;
  price: number;
}

export interface OrderResponse {
  status: string;
  message: string;
  orderId?: string;
  rejectReason?: string;
}
