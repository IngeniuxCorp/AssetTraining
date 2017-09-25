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

	get IsIFrame(): boolean {
		try {
			return window.self !== window.top;
		}
		catch (e) {
			return true;
		}
	}

	get IsPopUp(): boolean {
		return window.opener != null;
	}

	get SignInURL(): string {
		return this.BaseURL + "/SignIn/";
	}

	SignInPopUp() {
		window.open(this.SignInURL, '_blank');
	}

	constructor(elm: ElementRef, private signalRService: SignalRService) {
		this.BaseURL = elm.nativeElement.getAttribute('BaseURL');
		this.HasToken = elm.nativeElement.getAttribute('HasToken');
		signalRService.Message.subscribe(this._OnNewMessage.bind(this));
	}

	private _OnNewMessage(data: any) {
		if (data == null || data.Sender != "CustomTabMessage" || data.Message == null) {
			return;
		};

		if (data.Header == "TokenResponse") {
			if (data.Message.IsSuccess) {
				this.HasToken = true;
			}
		}
	}

	ngOnInit() {
	}
}
