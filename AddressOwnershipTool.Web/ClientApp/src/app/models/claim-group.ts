export interface ClaimGroup {
  destination: string;
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
  balance: number;
  originNetwork: string;
  type: string;
}

export interface SwappedTx {
  destination: string;
  amount: number;
  txHash: string;
  type: string;
}

export interface SwapRequest {
  destination: string;
  amount: number;
  txHash: string;
  path: string;
  type: string;
}
