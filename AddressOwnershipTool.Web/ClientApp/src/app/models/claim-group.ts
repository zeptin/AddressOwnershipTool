export interface ClaimGroup {
  destination: string;
  origin: string;
  numberOfClaimedAddresses: number;
  totalAmountToTransfer: number;
  originalTotalBalance: number;
  claims: Claim[];
  type: string;
}

export interface ClaimGroupResponse {
  result: ClaimGroup[],
  message: string
}

export interface Claim {
  destination: string;
  origin: string;
  balance: number;
  originNetwork: string;
  type: string;
}

export interface SwappedTx {
  destination: string;
  origin: string;
  amount: number;
  txHash: string;
  type: string;
}

export interface SwapRequest {
  destination: string;
  origin: string;
  amount: number;
  txHash: string;
  path: string;
  type: string;
}
