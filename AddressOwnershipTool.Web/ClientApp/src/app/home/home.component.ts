import { ClaimGroup } from './../models/claim-group';
import { DistributionService } from './../shared/distribution.service';
import { EthService } from '../shared/eth.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService } from '../shared/auth.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { directoryExistsValidator } from '../shared/directory.validator';
import { DirectoryValidationService } from '../shared/directory.service';
import { ClaimGroupsTableComponent } from './claim-groups-table/claim-groups-table.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  @ViewChild(ClaimGroupsTableComponent, { static: false })
  private claimGroupTableComponent!: ClaimGroupsTableComponent;

  directoryForm: FormGroup;
  claimGroups: ClaimGroup[] = [];
  selectedClaimGroups: ClaimGroup[] = [];
  canProcess = false;
  busy = false;

  failed: ClaimGroup[] = [];
  successful: ClaimGroup[] = [];

  constructor(
    public authService: AuthService,
    private ethService: EthService,
    private directoryValidationService: DirectoryValidationService,
    private distributionService: DistributionService) {
    this.directoryForm = new FormGroup({
      directoryPath: new FormControl('', {
        validators: [Validators.required],
        asyncValidators: [directoryExistsValidator(this.directoryValidationService)],
        updateOn: 'blur'
      }),
      limit: new FormControl('', {
        validators: [Validators.required],
        updateOn: 'blur'
      })
    });
  }

  async ngOnInit() {
    await this.authService.connectToMetaMask();
    // await this.ethService.sendTransaction(sessionStorage.getItem('account') ?? '', sessionStorage.getItem('account') ?? '', 0.1)
  }

  async onSubmit() {
    if (this.directoryForm.valid) {
      this.busy = true;
      const result = await this.distributionService.load(this.directoryForm.value.directoryPath, this.directoryForm.value.limit);
      if (!!result.message) {
        alert(result.message);
      } else {
        this.claimGroups = result.result;
      }
      this.busy = false;
    }
  }

  reset() {
    this.claimGroups.length = 0;
  }

  selectionUpdated(selectedItems: ClaimGroup[]) {
    console.log(selectedItems);
    this.selectedClaimGroups = selectedItems;
  }

  async process() {
    this.busy = true;
    let filtered: ClaimGroup[] = this.claimGroups;
    for (const claim of this.selectedClaimGroups) {
      try {
        const txHash = await this.ethService.sendTransaction(
          sessionStorage.getItem('account') ?? '',
          claim.destination,
          claim.totalAmountToTransfer
        );
        this.successful.push(claim);
        filtered = filtered.filter(
          (cg) => cg.destination !== claim.destination
        );

        await this.distributionService.update({
          txHash: txHash,
          destination: claim.destination,
          amount: claim.totalAmountToTransfer,
          path: this.directoryForm.value.directoryPath,
          type: claim.type,
          origin: claim.origin
        });
      } catch {
        this.failed.push(claim);
      }
    };
    this.busy = false;

    this.claimGroups.length = 0;
    for (const claim of filtered) {
      this.claimGroups.push(claim);
    }

    this.claimGroupTableComponent?.refresh();
    this.claimGroupTableComponent?.clearSelection();
  }
}
