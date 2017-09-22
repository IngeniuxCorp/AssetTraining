import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MdToolbarModule, MdDialogModule, MdButtonModule, MdTableModule, MdInputModule, MdIconModule } from '@angular/material';
import { CdkTableModule} from '@angular/cdk/table';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';
//import { MdTabsModule, MdInputModule, , MdListModule, MdCheckboxModule, MdProgressBarModule } from '@angular/material';
import { SignalRService }  from './signalR.service';
import { CustomTab } from './customTab.component';
import { PreviewDialog } from './previewDialog.component';
import { OneDriveTable } from './oneDriveTable.component';

@NgModule({
	imports: [
		BrowserModule,
		BrowserAnimationsModule,
		MdTableModule,
		CdkTableModule,
		MdToolbarModule,
		MdButtonModule,
		MdInputModule,
		MdIconModule,
		MdDialogModule,
		HttpModule,
		FormsModule
	],
	entryComponents: [
		PreviewDialog
	],
	declarations: [CustomTab, OneDriveTable, PreviewDialog],
	bootstrap: [CustomTab]
})
export class CustomTabApp { }
