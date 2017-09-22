import { Component, Input } from '@angular/core';
import { DriveDataSource } from './models/driveDataSource';
import { Http } from '@angular/http';
import {MdDialog, MdDialogRef, MD_DIALOG_DATA} from '@angular/material';
import { PreviewDialog }from './previewDialog.component' 
import {
	trigger,
	state,
	style,
	animate,
	transition
} from '@angular/animations';

@Component({
	selector: 'onedrive-table',
	templateUrl: './templates/oneDriveTable.html',
	styleUrls: ['./styles/oneDriveTable.css']
})
export class OneDriveTable {
	@Input() BaseURL: string;
	DataSource: DriveDataSource;
	DisplayedColumns = ['Type', 'Name', 'Preview', 'Import', 'Actions'];
	State: string = "loading";

	constructor(private http: Http, public dialog: MdDialog) {
	}

	PreviewItem(row: any) {
		if (row.Image != null) {
			let dialogRef = this.dialog.open(PreviewDialog, {
				width: '450px',
				data: { Name: row.Name, URL: row.WebUrl}
			});
			return;
		}

		if (row.WebUrl) {
			window.open(row.WebUrl, '_blank');
		}
	}

	ViewChildren(row: any) {
		if (row.Folder) {
			this.State = "loading";
			console.log(row);
			this.DataSource.NavigateToChild(row.Name);
			return;
		}
	}

	ImportAsset(row: any) {
		console.log(row);
		return false;
	}

	GetIcon(row: any): string {
		if (row.Folder != null) {
			return "folder";
		}

		if (row.Image != null) {
			return "image";
		}

		return "insert_drive_file";
	}

	ngOnInit() {
		this.DataSource = new DriveDataSource(this.BaseURL, this.http);
		this.DataSource.Message.subscribe(items => {
			this.State = "none";
		});
	}
}