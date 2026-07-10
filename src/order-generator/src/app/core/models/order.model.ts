export interface Order {
  ticker: string;
  side: 'BUY' | 'SELL';
  quantity: number;
  price: number;
}

export interface OrderResponse {
  orderId: string;
  symbol: string;
  side: string;
  quantity: number;
  price: number;
  status: string;
  brokerOrderId?: string;
  rejectReason?: string;
  createdAt: string;
}
