import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ClaimGroup } from 'src/app/models/claim-group';

@Component({
  selector: 'app-claim-group-table',
  templateUrl: './claim-groups-table.component.html'
})
export class ClaimGroupsTableComponent implements OnInit {
  @Input()
  claimGroups: ClaimGroup[] = [];

  @Input()
  failed: ClaimGroup[] = [];

  @Input()
  successful: ClaimGroup[] = [];

  @Output()
  selected = new EventEmitter<ClaimGroup[]>();

  paginatedClaimGroups: ClaimGroup[] = [];
  pageSize = 200;
  currentPage = 1;
  selectedClaimGroups: ClaimGroup[] = [];

  constructor() {}

  async ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.updatePaginator();
  }

  // Select all claim groups
  selectAll(event: any): void {
    if (event.target.checked) {
      this.selectedClaimGroups = [...this.claimGroups];
    } else {
      this.selectedClaimGroups = [];
    }

    this.selected.emit(this.selectedClaimGroups);
  }

  // Check if all claim groups are selected
  isAllSelected(): boolean {
    return (
      this.selectedClaimGroups.length === this.claimGroups.length &&
      this.claimGroups.length > 0
    );
  }

  // Select/deselect a single claim group
  selectClaimGroup(claimGroup: ClaimGroup, event: any): void {
    if (event.target.checked) {
      this.selectedClaimGroups.push(claimGroup);
    } else {
      this.selectedClaimGroups = this.selectedClaimGroups.filter(
        (cg) => cg !== claimGroup
      );
    }

    this.selected.emit(this.selectedClaimGroups);
  }

  // Check if a claim group is selected
  isSelected(claimGroup: ClaimGroup): boolean {
    return this.selectedClaimGroups.includes(claimGroup);
  }

  // Update the displayed claim groups when the paginator changes
  updatePaginator(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    this.paginatedClaimGroups = this.claimGroups.slice(
      startIndex,
      startIndex + this.pageSize
    );
  }

  hasFailed(claim: ClaimGroup) {
    return !!this.failed.find(f => f.destination === claim.destination);
  }

  hasSucceeded(claim: ClaimGroup) {
    return !!this.successful.find(f => f.destination === claim.destination);
  }

  clearSelection() {
    this.selectedClaimGroups.length = 0;
  }
}
