import { Component, Inject } from '@angular/core';
import {MdDialog, MdDialogRef, MD_DIALOG_DATA} from '@angular/material';

@Component({
	selector: 'preview-dialog',
	templateUrl: './templates/previewDialog.html',
	styleUrls: ['./styles/previewDialog.css']
})
export class PreviewDialog {

	constructor(
		public dialogRef: MdDialogRef<PreviewDialog>,
		@Inject(MD_DIALOG_DATA) public data: any) { }

	onNoClick(): void {
		this.dialogRef.close();
	}

}