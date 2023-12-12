export interface ClaimGroup {
  destination: string;
  numberOfClaimedAddresses: number;
  totalAmountToTransfer: number;
  originalTotalBalance: number;
  claims: Claim[]
}

export interface ClaimGroupResponse {
  result: ClaimGroup[],
  message: string
}

export interface Claim {
  destination: string;
  balance: number;
  originNetwork: string;
}
