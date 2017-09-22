import { Component, Input, ElementRef } from '@angular/core';
import { SignalRService } from './signalR.service';

@Component({
	selector: 'custom-tab',
	templateUrl: './templates/customTab.html',
	styleUrls: ['./styles/customTab.css'],
	providers: [SignalRService]
})
export class CustomTab {
	@Input() BaseURL: string;
	@Input() HasToken: boolean;

	get SignInURL(): string {
		return this.BaseURL + "/SignIn/"
	}

	constructor(elm: ElementRef) {
		this.BaseURL = elm.nativeElement.getAttribute('BaseURL');
		this.HasToken = elm.nativeElement.getAttribute('HasToken');
	}

	ngOnInit() {
	}
}
