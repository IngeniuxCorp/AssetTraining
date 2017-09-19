import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MdToolbarModule } from '@angular/material';
//import { CdkTableModule } from '@angular/cdk';
import { CdkTableModule} from '@angular/cdk/table';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';

import { SignalRService }  from './signalR.service';
import { CustomTab } from './customTab.component';


@NgModule({
	imports: [
		BrowserModule,
		BrowserAnimationsModule,
		CdkTableModule,
		MdToolbarModule,
		HttpModule,
		FormsModule
	],
	declarations: [CustomTab],
	bootstrap: [CustomTab]
})
export class CustomTabApp { }
