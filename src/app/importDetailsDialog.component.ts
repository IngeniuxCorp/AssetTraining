import { Component, Inject } from '@angular/core';
import {MdDialog, MdDialogRef, MD_DIALOG_DATA} from '@angular/material';

@Component({
	selector: 'impost-details-dialog',
	templateUrl: './templates/importDetailsDialog.html',
	styleUrls: ['./styles/importDetailsDialog.css']
})
export class ImportDetailsDialog {

	constructor( public dialogRef: MdDialogRef<ImportDetailsDialog>,
		@Inject(MD_DIALOG_DATA) public data: any) {

	}

	onNoClick(): void {
		this.dialogRef.close();
	}

}