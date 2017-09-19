import { Component, Input, ElementRef } from '@angular/core';
import { SignalRService } from './signalR.service';
import { MdToolbarModule } from '@angular/material';

@Component({
	selector: 'custom-tab',
	templateUrl: './templates/customTab.html',
	styleUrls: ['app/styles/customTab.css'],
	providers: [SignalRService]
})
export class CustomTab {
	@Input() BaseURL: string;

	constructor(elm: ElementRef) {
		this.BaseURL = elm.nativeElement.getAttribute('BaseURL');
	}

	ngOnInit() {
	}
}
