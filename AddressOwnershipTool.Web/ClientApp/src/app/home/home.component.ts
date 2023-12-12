import { ClaimGroup } from './../models/claim-group';
import { DistributionService } from './../shared/distribution.service';
import { EthService } from '../shared/eth.service';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../shared/auth.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { directoryExistsValidator } from '../shared/directory.validator';
import { DirectoryValidationService } from '../shared/directory.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {

  directoryForm: FormGroup;
  claimGroups: ClaimGroup[] = [];
  selectedClaimGroups: ClaimGroup[] = [];
  canProcess = false;
  busy = false;

  constructor(
    public authService: AuthService,
    private ethService: EthService,
    private directoryValidationService: DirectoryValidationService,
    private distributionService: DistributionService) {
    this.directoryForm = new FormGroup({
      directoryPath: new FormControl('C:\\Temp\\cirrus', {
        validators: [Validators.required],
        asyncValidators: [directoryExistsValidator(this.directoryValidationService)],
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
      console.log(this.directoryForm.value);
      const result = await this.distributionService.load(this.directoryForm.value.directoryPath);
      if (!!result.message) {
        alert(result.message);
      } else {
        this.claimGroups = result.result;
      }
    }
  }

  reset() {
    this.claimGroups.length = 0;
  }

  selectionUpdated(selectedItems: ClaimGroup[]) {
    console.log(selectedItems);
    this.selectedClaimGroups = selectedItems;
  }

  process() {

  }
}
